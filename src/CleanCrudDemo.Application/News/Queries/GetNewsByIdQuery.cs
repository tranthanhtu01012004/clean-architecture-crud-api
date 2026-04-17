using CleanCrudDemo.Application.Abstractions;
using CleanCrudDemo.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanCrudDemo.Application.News.Queries;

public record GetNewsByIdQuery(Guid Id) : IRequest<NewsDto?>; // CQRS + MediatR: query lấy chi tiết News.

public class GetNewsByIdQueryHandler : IRequestHandler<GetNewsByIdQuery, NewsDto?>
{
    private readonly IAppDbContext _context; // Clean Architecture: DbContext abstraction.

    public GetNewsByIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<NewsDto?> Handle(GetNewsByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.News // EF Core: truy cập DbSet News.
            .Include(x => x.MenuNews) // EF Core: include bảng nối.
            .Where(x => x.Id == request.Id) // LINQ: lọc theo id.
            .Select(x => new NewsDto // LINQ: map sang DTO.
            {
                Id = x.Id,
                Title = x.Title,
                Content = x.Content,
                MenuIds = x.MenuNews.Select(mn => mn.MenuId).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken); // EF Core: lấy bản ghi đầu hoặc null.
    }
}
