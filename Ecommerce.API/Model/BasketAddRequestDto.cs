using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Ecommerce.API.Model
{
    public class BasketAddRequestDto
    {
        [JsonProperty("productId")]
        public Guid ProductId { get; set; }
        [JsonProperty("quantity")]
        public int Quantity { get; set; }

    }
}
