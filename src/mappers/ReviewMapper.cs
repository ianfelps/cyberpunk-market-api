using CyberpunkMarket.Models;
using cyberpunk_market_api.src.dtos.Review;
using cyberpunk_market_api.src.responses;

namespace cyberpunk_market_api.src.mappers;

public static class ReviewMapper
{
    public static ReviewResponse ToResponse(Review review)
    {
        return new ReviewResponse
        {
            Id = review.Id,
            UserId = review.UserId,
            ProductId = review.ProductId,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt,
            UpdatedAt = review.UpdatedAt
        };
    }

    public static Review ToEntity(CreateReviewDto dto, Guid userId)
    {
        return new Review
        {
            UserId = userId,
            ProductId = dto.ProductId,
            Rating = dto.Rating,
            Comment = dto.Comment
        };
    }
}
