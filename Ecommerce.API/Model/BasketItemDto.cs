using Newtonsoft.Json;

namespace Ecommerce.API.Model
{
    public class BasketItemDto
    {
        [JsonProperty("productId")]
        public string ProductId { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }
    }
}