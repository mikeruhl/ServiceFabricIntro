using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Ecommerce.API.Model;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using UserActor.Interfaces;

namespace Ecommerce.API.Controllers
{
    public class BasketController : ApiController
    {
        [HttpGet]
        [Route("basket/{userId}")]
        public async Task<BasketDto> Get(string userId)
        {
            var actor = GetActor(userId);
            var products = await actor.GetBasket();

            return new BasketDto()
            {
                UserId = userId,
                ItemsDto = products.Select(
                    p=>new BasketItemDto{ProductId = p.Key.ToString(), Quantity = p.Value})
                .ToArray()
            };
        }

        [HttpPost]
        [Route("basket/{userId}")]
        public async Task Add(string userId, [FromBody] BasketAddRequestDto requestDto)
        {
            var actor = GetActor(userId);
            await actor.AddToBasket(requestDto.ProductId, requestDto.Quantity);
        }

        [HttpDelete]
        [Route("basket/{userId}")]
        public async Task Delete(string userId)
        {
            var actor = GetActor(userId);
            await actor.ClearBasket();
        }

        private IUserActor GetActor(string userId)
        {
            return ActorProxy.Create<IUserActor>(new ActorId(userId),
                new Uri("fabric:/Ecommerce/UserActorService"));
        }
    }
}
