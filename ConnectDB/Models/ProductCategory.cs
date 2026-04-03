using System.Text.Json.Serialization;

namespace ConnectDB.Models
{
    public class ProductCategory
    {
        public long ProductId { get; set; }
        [JsonIgnore]
        public Product Product { get; set; }
        public long CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
