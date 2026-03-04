namespace cyberpunk_market_api.src.dtos.Review;

public class CreateReviewDto
{
    public Guid ProductId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
