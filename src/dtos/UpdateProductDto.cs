namespace cyberpunk_market_api.src.dtos;

public class UpdateProductDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public int? StockQuantity { get; set; }
    public bool? IsActive { get; set; }
    public Guid? CategoryId { get; set; }
}
