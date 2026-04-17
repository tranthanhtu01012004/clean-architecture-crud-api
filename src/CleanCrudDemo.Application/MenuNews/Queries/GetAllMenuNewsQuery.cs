using CleanCrudDemo.Application.Abstractions;
using CleanCrudDemo.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanCrudDemo.Application.MenuNewsRelations.Queries;

public record GetAllMenuNewsQuery() : IRequest<List<MenuNewsDto>>;

public class GetAllMenuNewsQueryHandler : IRequestHandler<GetAllMenuNewsQuery, List<MenuNewsDto>>
{
    private readonly IAppDbContext _context;

    public GetAllMenuNewsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<MenuNewsDto>> Handle(GetAllMenuNewsQuery request, CancellationToken cancellationToken)
    {
        return await _context.MenuNews
            .Include(x => x.Menu)
            .Include(x => x.News)
            .Select(x => new MenuNewsDto
            {
                MenuId = x.MenuId,
                MenuName = x.Menu != null ? x.Menu.Name : string.Empty,
                NewsId = x.NewsId,
                NewsTitle = x.News != null ? x.News.Title : string.Empty
            })
            .ToListAsync(cancellationToken);
    }
}