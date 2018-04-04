using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.CheckoutService.Model;
using Ecommerce.ProductCatalog.Model;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using UserActor.Interfaces;

namespace Ecommerce.CheckoutService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class CheckoutService : StatefulService, ICheckoutService
    {
        public CheckoutService(StatefulServiceContext context)
            : base(context)
        { }

        
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[]
            {
                new ServiceReplicaListener(this.CreateServiceRemotingListener)
            };
        }

        public async Task<CheckoutSummary> Checkout(string userId)
        {
            var result = new CheckoutSummary();
            result.Date = DateTime.UtcNow;
            result.Products = new List<CheckoutProduct>();

            //call user actor to get the basket
            var userActor = GetUserActor(userId);
            var basket = await userActor.GetBasket();

            //get catalog client
            IProductCatalogService catalogService = GetProductCatalogService();

            //constuct CheckoutProduct items by calling to the catalog
            foreach (var basketLine in basket)
            {
                var product = await catalogService.GetProduct(basketLine.Key);
                if (product != null)
                {
                    var checkoutProduct = new CheckoutProduct
                    {
                        Product = product,
                        Price = product.Price,
                        Quantity = basketLine.Value
                    };
                    result.Products.Add(checkoutProduct);
                }
            }

            //generate total price
            result.TotalPrice = result.Products.Select(p=>p.Quantity * p.Price).Sum();

            //clear user basket
            await userActor.ClearBasket();

            await AddToHistory(result);

            return result;
        }

        public async Task<IEnumerable<CheckoutSummary>> GetOrderHitory(string userId)
        {
            var result = new List<CheckoutSummary>();
            var history = await StateManager.GetOrAddAsync<IReliableDictionary<DateTime, CheckoutSummary>>("history");

            using (var tx = StateManager.CreateTransaction())
            {
                var allProducts = await history.CreateEnumerableAsync(tx, EnumerationMode.Unordered);
                using (var enumerator = allProducts.GetAsyncEnumerator())
                {
                    while (await enumerator.MoveNextAsync(CancellationToken.None))
                    {
                        var current = enumerator.Current;

                        result.Add(current.Value);
                    }
                }
            }

            return result;
        }

        private async Task AddToHistory(CheckoutSummary checkout)
        {
            var history = await StateManager.GetOrAddAsync<IReliableDictionary<DateTime, CheckoutSummary>>("history");

            using (var tx = StateManager.CreateTransaction())
            {
                await history.AddAsync(tx, checkout.Date, checkout);

                await tx.CommitAsync();
            }
        }

        private IUserActor GetUserActor(string userId)
        {
            return ActorProxy.Create<IUserActor>(new ActorId(userId), new Uri("fabric:/Ecommerce/UserActorService"));
        }

        private IProductCatalogService GetProductCatalogService()
        {
            return ServiceProxy.Create<IProductCatalogService>(
                new Uri("fabric:/Ecommerce/ECommerce.ProductCatalog"),
                new ServicePartitionKey(0));
        }
    }
}
