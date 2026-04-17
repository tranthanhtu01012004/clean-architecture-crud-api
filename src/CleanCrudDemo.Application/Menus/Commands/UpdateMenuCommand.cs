using CleanCrudDemo.Application.Abstractions;
using CleanCrudDemo.Application.DTOs;
using CleanCrudDemo.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanCrudDemo.Application.Menus.Commands;

public record UpdateMenuCommand(Guid Id, string Name, string Description, List<Guid> NewsIds) : IRequest<MenuDto?>;

public class UpdateMenuCommandHandler : IRequestHandler<UpdateMenuCommand, MenuDto?>
{
    private readonly IAppDbContext _context; // Clean Architecture: abstraction DbContext.
    private readonly IEventPublisher _eventPublisher; // RabbitMQ: publisher.

    public UpdateMenuCommandHandler(IAppDbContext context, IEventPublisher eventPublisher)
    {
        _context = context;
        _eventPublisher = eventPublisher;
    }

    public async Task<MenuDto?> Handle(UpdateMenuCommand request, CancellationToken cancellationToken)
    {
        var menu = await _context.Menus
            .Include(x => x.MenuNews)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (menu is null) return null;

        menu.Name = request.Name;
        menu.Description = request.Description;

        menu.MenuNews.Clear();

        foreach (var newsId in request.NewsIds.Distinct())
        {
            menu.MenuNews.Add(new MenuNews
            {
                MenuId = menu.Id,
                NewsId = newsId
            });
        }

        await _context.SaveChangesAsync(cancellationToken);

        var result = new MenuDto
        {
            Id = menu.Id,
            Name = menu.Name,
            Description = menu.Description,
            NewsIds = menu.MenuNews.Select(x => x.NewsId).ToList()
        };

        await _eventPublisher.PublishAsync(
            "menu-events",
            new { Event = "MenuUpdated", result.Id, result.Name },
            cancellationToken);

        return result;
    }
}