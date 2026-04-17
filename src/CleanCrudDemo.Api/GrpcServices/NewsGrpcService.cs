using CleanCrudDemo.Application.News.Queries;
using Grpc.Core;
using MediatR;

namespace CleanCrudDemo.Api.GrpcServices;

public class NewsGrpcService : NewsGrpc.NewsGrpcBase
{
    private readonly IMediator _mediator;

    public NewsGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<GetNewsByIdReply> GetNewsById(
        GetNewsByIdRequest request,
        ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var newsId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid news id"));
        }

        var result = await _mediator.Send(
            new GetNewsByIdQuery(newsId),
            context.CancellationToken);

        if (result is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "News not found"));
        }

        return new GetNewsByIdReply
        {
            Id = result.Id.ToString(),
            Title = result.Title,
            Content = result.Content
        };
    }
}