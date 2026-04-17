using CleanCrudDemo.Application.Abstractions;
using CleanCrudDemo.Application.DTOs;
using CleanCrudDemo.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanCrudDemo.Application.News.Commands;

public record CreateNewsCommand(string Title, string Content, List<Guid> MenuIds) : IRequest<NewsDto>;

public class CreateNewsCommandHandler : IRequestHandler<CreateNewsCommand, NewsDto>
{
    private readonly IAppDbContext _context;
    private readonly IEventPublisher _eventPublisher;

    public CreateNewsCommandHandler(IAppDbContext context, IEventPublisher eventPublisher)
    {
        _context = context;
        _eventPublisher = eventPublisher;
    }

    public async Task<NewsDto> Handle(CreateNewsCommand request, CancellationToken cancellationToken)
    {
        var news = new Domain.Entities.News
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Content = request.Content
        };

        _context.News.Add(news);

        if (request.MenuIds is not null && request.MenuIds.Count > 0)
        {
            var existingMenuIds = await _context.Menus
                .Where(x => request.MenuIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

            foreach (var menuId in existingMenuIds.Distinct())
            {
                _context.MenuNews.Add(new MenuNews
                {
                    MenuId = menuId,
                    NewsId = news.Id
                });
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        var result = new NewsDto
        {
            Id = news.Id,
            Title = news.Title,
            Content = news.Content,
            MenuIds = request.MenuIds?.Distinct().ToList() ?? []
        };

        await _eventPublisher.PublishAsync(
            "news-events",
            new { Event = "NewsCreated", result.Id, result.Title },
            cancellationToken);

        return result;
    }
}