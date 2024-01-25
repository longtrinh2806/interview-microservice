namespace OrderServices.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public int TotalPrice { get; set; }
        public DateTime Date { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
