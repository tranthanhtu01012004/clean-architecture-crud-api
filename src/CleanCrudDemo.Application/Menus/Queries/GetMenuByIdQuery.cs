using CleanCrudDemo.Application.Abstractions;
using CleanCrudDemo.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanCrudDemo.Application.Menus.Queries;

public record GetMenuByIdQuery(Guid Id) : IRequest<MenuDto?>; // CQRS + MediatR: query lấy chi tiết Menu.

public class GetMenuByIdQueryHandler : IRequestHandler<GetMenuByIdQuery, MenuDto?>
{
    private readonly IAppDbContext _context; // Clean Architecture: abstraction DbContext.

    public GetMenuByIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<MenuDto?> Handle(GetMenuByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Menus // EF Core: truy cập bảng Menu.
            .Include(x => x.MenuNews) // EF Core: load bảng nối n-n.
            .Where(x => x.Id == request.Id) // LINQ: lọc theo id.
            .Select(x => new MenuDto // LINQ: projection sang DTO.
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                NewsIds = x.MenuNews.Select(mn => mn.NewsId).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken); // EF Core: lấy record đầu tiên hoặc null.
    }
}
