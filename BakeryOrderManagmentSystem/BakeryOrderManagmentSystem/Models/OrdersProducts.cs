using BakeryOrderManagmentSystem.Models;
using System.ComponentModel.DataAnnotations;

public class OrdersProducts
{
    [Key]
    public int OrderProductId { get; set; }
    
    [Required]
    public int OrderId { get; set; }
    
    [Required]
    public int ProductId { get; set; }
    
    [Required]
    public int Quantity { get; set; }
    public Order? Order { get; set; }
    public Product? Product { get; set; }
}