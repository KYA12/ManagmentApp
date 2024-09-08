public class OrderProductDto
{
    public int OrderProductId { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public string? ProductName { get; set; }
}