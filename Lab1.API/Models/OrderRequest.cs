namespace Order.API.Models
{
    public class OrderRequest
    {
        public int userId { get; set; }
        public List<OrderItem> orderItems { get; set; }
    }
}
