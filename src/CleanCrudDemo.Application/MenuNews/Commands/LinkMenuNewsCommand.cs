using CleanCrudDemo.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanCrudDemo.Application.MenuNewsRelations.Commands;

public record LinkMenuNewsCommand(Guid MenuId, Guid NewsId) : IRequest<bool>;

public class LinkMenuNewsCommandHandler : IRequestHandler<LinkMenuNewsCommand, bool>
{
    private readonly IAppDbContext _context;

    public LinkMenuNewsCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(LinkMenuNewsCommand request, CancellationToken cancellationToken)
    {
        var menuExists = await _context.Menus
            .AnyAsync(x => x.Id == request.MenuId, cancellationToken);

        var newsExists = await _context.News
            .AnyAsync(x => x.Id == request.NewsId, cancellationToken);

        if (!menuExists || !newsExists)
            return false;

        var relationExists = await _context.MenuNews
            .AnyAsync(x => x.MenuId == request.MenuId && x.NewsId == request.NewsId, cancellationToken);

        if (relationExists)
            return false;

        _context.MenuNews.Add(new CleanCrudDemo.Domain.Entities.MenuNews
        {
            MenuId = request.MenuId,
            NewsId = request.NewsId
        });

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}