using System.ComponentModel.DataAnnotations;

namespace BakeryOrderManagmentSystem.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
       
        [Required]
        public int CustomerId { get; set; }
       
        [Required]
        public DateTime OrderDate { get; set; }
        
        [Required]
        public OrderStatus Status { get; set; }

        public ICollection<OrdersProducts>? OrdersProducts { get; set; }
    }
}
