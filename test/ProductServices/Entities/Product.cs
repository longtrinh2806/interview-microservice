using System.ComponentModel.DataAnnotations;

namespace ProductServices.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        [ConcurrencyCheck]
        public string Name { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
    }
}
