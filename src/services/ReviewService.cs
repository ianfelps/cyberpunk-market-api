using cyberpunk_market_api.src.constants;
using cyberpunk_market_api.src.contexts;
using cyberpunk_market_api.src.dtos.Review;
using cyberpunk_market_api.src.interfaces;
using cyberpunk_market_api.src.mappers;
using cyberpunk_market_api.src.responses;
using Microsoft.EntityFrameworkCore;

namespace cyberpunk_market_api.src.services;

public class ReviewService : IReviewService
{
    private readonly ApplicationDbContext _context;

    public ReviewService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<PagedResponse<ReviewResponse>>> GetByProductIdAsync(Guid productId, int page, int pageSize, int? minRating, int? maxRating)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = PaginationConstants.DefaultPageSize;
        if (pageSize > PaginationConstants.MaxPageSize) pageSize = PaginationConstants.MaxPageSize;
        var query = _context.Reviews.Where(r => r.ProductId == productId);
        if (minRating.HasValue)
            query = query.Where(r => r.Rating >= minRating.Value);
        if (maxRating.HasValue)
            query = query.Where(r => r.Rating <= maxRating.Value);
        var totalCount = await query.CountAsync();
        var reviews = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        var items = reviews.Select(ReviewMapper.ToResponse);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        var paged = new PagedResponse<ReviewResponse>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages
        };
        return ApiResponse<PagedResponse<ReviewResponse>>.Success(paged);
    }

    public async Task<ApiResponse<ReviewResponse?>> GetByIdAsync(Guid id)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review == null)
            return ApiResponse<ReviewResponse?>.Fail("Avaliação não encontrada.");
        return ApiResponse<ReviewResponse?>.Success(ReviewMapper.ToResponse(review));
    }

    public async Task<ApiResponse<ReviewResponse>> CreateAsync(Guid userId, CreateReviewDto dto)
    {
        if (dto.Rating < 1 || dto.Rating > 5)
            return ApiResponse<ReviewResponse>.Fail("Avaliação deve ser entre 1 e 5.");
        var productExists = await _context.Products.AnyAsync(p => p.Id == dto.ProductId);
        if (!productExists)
            return ApiResponse<ReviewResponse>.Fail("Produto não encontrado.");
        var alreadyReviewed = await _context.Reviews.AnyAsync(r => r.UserId == userId && r.ProductId == dto.ProductId);
        if (alreadyReviewed)
            return ApiResponse<ReviewResponse>.Fail("Você já avaliou este produto.");
        var review = ReviewMapper.ToEntity(dto, userId);
        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();
        return ApiResponse<ReviewResponse>.Success(ReviewMapper.ToResponse(review), "Avaliação registrada com sucesso.");
    }

    public async Task<ApiResponse<ReviewResponse?>> UpdateAsync(Guid userId, Guid id, UpdateReviewDto dto)
    {
        var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
        if (review == null)
            return ApiResponse<ReviewResponse?>.Fail("Avaliação não encontrada.");
        if (dto.Rating.HasValue)
        {
            if (dto.Rating.Value < 1 || dto.Rating.Value > 5)
                return ApiResponse<ReviewResponse?>.Fail("Avaliação deve ser entre 1 e 5.");
            review.Rating = dto.Rating.Value;
        }
        if (dto.Comment != null)
            review.Comment = dto.Comment;
        review.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return ApiResponse<ReviewResponse?>.Success(ReviewMapper.ToResponse(review), "Avaliação atualizada com sucesso.");
    }

    public async Task<ApiResponse<object>> DeleteAsync(Guid userId, Guid id)
    {
        var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
        if (review == null)
            return ApiResponse<object>.Fail("Avaliação não encontrada.");
        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();
        return ApiResponse<object>.Success(new { }, "Avaliação removida com sucesso.");
    }
}
