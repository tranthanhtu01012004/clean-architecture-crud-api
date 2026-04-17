using CleanCrudDemo.Application.Abstractions;
using CleanCrudDemo.Application.DTOs;
using CleanCrudDemo.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanCrudDemo.Application.Menus.Commands;

public record CreateMenuCommand(Guid Id, string Name, string Description, List<Guid> NewsIds) : IRequest<MenuDto>;

public class CreateMenuCommandHandler : IRequestHandler<CreateMenuCommand, MenuDto>
{
    private readonly IAppDbContext _context;
    private readonly IEventPublisher _eventPublisher;

    public CreateMenuCommandHandler(IAppDbContext context, IEventPublisher eventPublisher)
    {
        _context = context;
        _eventPublisher = eventPublisher;
    }

    public async Task<MenuDto> Handle(CreateMenuCommand request, CancellationToken cancellationToken)
    {
        var menu = new Menu
        {
            Id = request.Id == Guid.Empty ? Guid.NewGuid() : request.Id,
            Name = request.Name,
            Description = request.Description
        };

        _context.Menus.Add(menu);

        if (request.NewsIds is not null && request.NewsIds.Count > 0)
        {
            var existingNewsIds = await _context.News
                .Where(x => request.NewsIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

            foreach (var newsId in existingNewsIds.Distinct())
            {
                _context.MenuNews.Add(new MenuNews
                {
                    MenuId = menu.Id,
                    NewsId = newsId
                });
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        var result = new MenuDto
        {
            Id = menu.Id,
            Name = menu.Name,
            Description = menu.Description,
            NewsIds = request.NewsIds?.Distinct().ToList() ?? []
        };

        await _eventPublisher.PublishAsync(
            "menu-events",
            new { Event = "MenuCreated", result.Id, result.Name },
            cancellationToken);

        return result;
    }
}