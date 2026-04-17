using CleanCrudDemo.Application.Abstractions;
using CleanCrudDemo.Application.DTOs;
using CleanCrudDemo.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanCrudDemo.Application.News.Commands;

public record UpdateNewsCommand(Guid Id, string Title, string Content, List<Guid> MenuIds) : IRequest<NewsDto?>;

public class UpdateNewsCommandHandler : IRequestHandler<UpdateNewsCommand, NewsDto?>
{
    private readonly IAppDbContext _context;
    private readonly IEventPublisher _eventPublisher;

    public UpdateNewsCommandHandler(IAppDbContext context, IEventPublisher eventPublisher)
    {
        _context = context;
        _eventPublisher = eventPublisher;
    }

    public async Task<NewsDto?> Handle(UpdateNewsCommand request, CancellationToken cancellationToken)
    {
        var news = await _context.News
            .Include(x => x.MenuNews)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (news is null) return null;

        news.Title = request.Title;
        news.Content = request.Content;

        news.MenuNews.Clear();

        foreach (var menuId in request.MenuIds.Distinct())
        {
            news.MenuNews.Add(new MenuNews
            {
                MenuId = menuId,
                NewsId = news.Id
            });
        }

        await _context.SaveChangesAsync(cancellationToken);

        var result = new NewsDto
        {
            Id = news.Id,
            Title = news.Title,
            Content = news.Content,
            MenuIds = news.MenuNews.Select(x => x.MenuId).ToList()
        };

        await _eventPublisher.PublishAsync(
            "news-events",
            new { Event = "NewsUpdated", result.Id, result.Title },
            cancellationToken);

        return result;
    }
}