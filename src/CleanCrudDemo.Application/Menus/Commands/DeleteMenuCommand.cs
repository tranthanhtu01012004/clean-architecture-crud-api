using CleanCrudDemo.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanCrudDemo.Application.Menus.Commands;

public record DeleteMenuCommand(Guid Id) : IRequest<bool>;

public class DeleteMenuCommandHandler : IRequestHandler<DeleteMenuCommand, bool>
{
    private readonly IAppDbContext _context;
    private readonly IEventPublisher _eventPublisher;

    public DeleteMenuCommandHandler(IAppDbContext context, IEventPublisher eventPublisher)
    {
        _context = context;
        _eventPublisher = eventPublisher;
    }

    public async Task<bool> Handle(DeleteMenuCommand request, CancellationToken cancellationToken)
    {
        var menu = await _context.Menus
            .Include(x => x.MenuNews)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (menu is null) return false;

        menu.MenuNews.Clear();
        _context.Menus.Remove(menu);

        await _context.SaveChangesAsync(cancellationToken);

        await _eventPublisher.PublishAsync(
            "menu-events",
            new { Event = "MenuDeleted", Id = request.Id },
            cancellationToken);

        return true;
    }
}