using CleanCrudDemo.Application.Abstractions;
using CleanCrudDemo.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanCrudDemo.Application.MenuNewsRelations.Queries;

public record GetMenuNewsByNewsIdQuery(Guid NewsId) : IRequest<List<MenuNewsDto>>;

public class GetMenuNewsByNewsIdQueryHandler : IRequestHandler<GetMenuNewsByNewsIdQuery, List<MenuNewsDto>>
{
    private readonly IAppDbContext _context;

    public GetMenuNewsByNewsIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<MenuNewsDto>> Handle(GetMenuNewsByNewsIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.MenuNews
            .Where(x => x.NewsId == request.NewsId)
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