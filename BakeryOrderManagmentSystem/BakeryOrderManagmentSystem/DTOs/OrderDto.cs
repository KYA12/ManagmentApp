using System.ComponentModel.DataAnnotations;

public class OrderDto
{
    public int OrderId { get; set; }

    [Required]
    public int CustomerId { get; set; }

    [Required]
    public DateTime OrderDate { get; set; }

    [Required]
    public string? Status { get; set; }

    public List<OrderProductDto>? OrdersProducts { get; set; }
}
