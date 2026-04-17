using CleanCrudDemo.Application.Abstractions;
using CleanCrudDemo.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanCrudDemo.Application.News.Queries;

public record GetAllNewsQuery() : IRequest<List<NewsDto>>; // CQRS + MediatR: query lấy tất cả News.

public class GetAllNewsQueryHandler : IRequestHandler<GetAllNewsQuery, List<NewsDto>>
{
    private readonly IAppDbContext _context; // Clean Architecture: DbContext abstraction.

    public GetAllNewsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<NewsDto>> Handle(GetAllNewsQuery request, CancellationToken cancellationToken)
    {
        return await _context.News // EF Core: truy cập DbSet News.
            .Include(x => x.MenuNews) // EF Core: load bảng nối n-n.
            .Select(x => new NewsDto // LINQ Projection: map sang DTO.
            {
                Id = x.Id,
                Title = x.Title,
                Content = x.Content,
                MenuIds = x.MenuNews.Select(mn => mn.MenuId).ToList() // LINQ: lấy danh sách MenuId.
            })
            .ToListAsync(cancellationToken); // EF Core Async: chạy query.
    }
}
