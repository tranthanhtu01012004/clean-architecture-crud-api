using CleanCrudDemo.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanCrudDemo.Application.MenuNewsRelations.Commands;

public record UnlinkMenuNewsCommand(Guid MenuId, Guid NewsId) : IRequest<bool>;

public class UnlinkMenuNewsCommandHandler : IRequestHandler<UnlinkMenuNewsCommand, bool>
{
    private readonly IAppDbContext _context;

    public UnlinkMenuNewsCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UnlinkMenuNewsCommand request, CancellationToken cancellationToken)
    {
        var relation = await _context.MenuNews
            .FirstOrDefaultAsync(x => x.MenuId == request.MenuId && x.NewsId == request.NewsId, cancellationToken);

        if (relation is null)
            return false;

        _context.MenuNews.Remove(relation);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}