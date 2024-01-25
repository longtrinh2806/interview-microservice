namespace ProductServices.Dtos
{
    public class CreateProductDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
    }
}