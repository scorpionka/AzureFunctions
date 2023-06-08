namespace OrderItemsReserver.Models
{
    public class Order
    {
        public string? BuyerId { get; set; }
        public DateTime OrderDate { get; set; }
        public ShipToAddress? ShipToAddress { get; set; }
        public List<OrderItem>? OrderItems { get; set; }
        public int Id { get; set; }
        public double TotalSum { get; set; }

        public void CalculateTotalSum()
        {
            foreach (var item in OrderItems!)
            {
                TotalSum += item.UnitPrice * item.Units;
            }
        }
    }
}
