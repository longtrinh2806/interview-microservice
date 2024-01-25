namespace ProductServices.Dtos
{
    public class UpdateProductDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
    }
}