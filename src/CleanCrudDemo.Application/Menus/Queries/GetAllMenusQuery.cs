using CleanCrudDemo.Application.Abstractions;
using CleanCrudDemo.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanCrudDemo.Application.Menus.Queries;

public record GetAllMenusQuery() : IRequest<List<MenuDto>>; // CQRS + MediatR: query lấy toàn bộ Menu.

public class GetAllMenusQueryHandler : IRequestHandler<GetAllMenusQuery, List<MenuDto>>
{
    private readonly IAppDbContext _context; // Clean Architecture: abstraction DbContext.

    public GetAllMenusQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<MenuDto>> Handle(GetAllMenusQuery request, CancellationToken cancellationToken)
    {
        return await _context.Menus // EF Core: truy cập bảng Menus.
            .Include(x => x.MenuNews) // EF Core: load bảng nối để lấy NewsIds.
            .Select(x => new MenuDto // LINQ Projection: map trực tiếp từ query sang DTO.
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                NewsIds = x.MenuNews.Select(mn => mn.NewsId).ToList() // LINQ: lấy các NewsId liên kết.
            })
            .ToListAsync(cancellationToken); // EF Core Async: chạy query và trả list.
    }
}
