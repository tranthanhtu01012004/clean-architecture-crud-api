using CleanCrudDemo.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanCrudDemo.Application.News.Commands;

public record DeleteNewsCommand(Guid Id) : IRequest<bool>;

public class DeleteNewsCommandHandler : IRequestHandler<DeleteNewsCommand, bool>
{
    private readonly IAppDbContext _context;
    private readonly IEventPublisher _eventPublisher;

    public DeleteNewsCommandHandler(IAppDbContext context, IEventPublisher eventPublisher)
    {
        _context = context;
        _eventPublisher = eventPublisher;
    }

    public async Task<bool> Handle(DeleteNewsCommand request, CancellationToken cancellationToken)
    {
        var news = await _context.News
            .Include(x => x.MenuNews)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (news is null) return false;

        news.MenuNews.Clear();
        _context.News.Remove(news);

        await _context.SaveChangesAsync(cancellationToken);

        await _eventPublisher.PublishAsync(
            "news-events",
            new { Event = "NewsDeleted", Id = request.Id },
            cancellationToken);

        return true;
    }
}