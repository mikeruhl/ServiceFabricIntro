using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Ecommerce.API.Model
{
    public class BasketDto
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }
        [JsonProperty("items")]
        public BasketItemDto[] ItemsDto { get; set; }

    }
}
