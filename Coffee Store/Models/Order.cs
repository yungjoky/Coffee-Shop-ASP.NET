namespace Coffee_Store.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }

        public decimal Price { get; set; }
        public Orderstatus status { get; set; }

        public User User { get; set; }
    }
}
