using BakeryOrderManagmentSystem.Models;
using System.ComponentModel.DataAnnotations;

public class Product
{
    [Key]
    public int ProductId { get; set; }
 
    [Required]
    public string? Name { get; set; }

    [Required]
    public string? Description { get; set; }

    [Required]
    public decimal Price { get; set; }

    public bool IsActive { get; set; }

    public ICollection<OrdersProducts>? OrdersProducts { get; set; }
}