namespace OrderItemsReserver.Models
{
    public class OrderItem
    {
        public ItemOrdered? ItemOrdered { get; set; }
        public double UnitPrice { get; set; }
        public int Units { get; set; }
        public int Id { get; set; }
    }
}