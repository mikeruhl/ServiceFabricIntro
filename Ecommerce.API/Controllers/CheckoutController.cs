using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using Ecommerce.API.Model;
using Ecommerce.CheckoutService.Model;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Ecommerce.API.Controllers
{
    public class CheckoutController : ApiController
    {
        private static readonly Random rnd = new Random(DateTime.UtcNow.Second);

        [HttpGet]
        [Route("checkout/{userId}")]
        public async Task<CheckoutSummaryDto> Checkout(string userId)
        {
            var summary = await GetCheckoutService().Checkout(userId);

            return Mapper.Map<CheckoutSummaryDto>(summary);
        }

        [HttpGet]
        [Route("history/{userId}")]
        public async Task<IEnumerable<CheckoutSummaryDto>> GetHistory(string userId)
        {
            var history = await GetCheckoutService().GetOrderHitory(userId);

            return Mapper.Map<IEnumerable<CheckoutSummaryDto>>(history);
        }

        private ICheckoutService GetCheckoutService()
        {
            var key = LongRandom();

            return ServiceProxy.Create<ICheckoutService>(
                new Uri("fabric:/Ecommerce/Ecommerce.CheckoutService"),
                new ServicePartitionKey(key));
        }

        private long LongRandom()
        {
            var buf = new byte[8];
            rnd.NextBytes(buf);
            var longRand = BitConverter.ToInt64(buf, 0);
            return longRand;
        }
    }
}
