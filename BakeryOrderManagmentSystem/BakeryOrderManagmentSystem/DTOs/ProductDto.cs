using System.ComponentModel.DataAnnotations;

public class ProductDto
{
    public int ProductId { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string? Name { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string? Description { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
    public decimal Price { get; set; }

    public bool IsActive { get; set; }
}