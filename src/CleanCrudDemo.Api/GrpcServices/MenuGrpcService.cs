using CleanCrudDemo.Application.Menus.Queries;
using Grpc.Core;
using MediatR;

namespace CleanCrudDemo.Api.GrpcServices;

public class MenuGrpcService : MenuGrpc.MenuGrpcBase
{
    private readonly IMediator _mediator;

    public MenuGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<GetMenuByIdReply> GetMenuById(
        GetMenuByIdRequest request,
        ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var menuId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid menu id"));
        }

        var result = await _mediator.Send(
            new GetMenuByIdQuery(menuId),
            context.CancellationToken);

        if (result is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Menu not found"));
        }

        var reply = new GetMenuByIdReply
        {
            Id = result.Id.ToString(),
            Name = result.Name,
            Description = result.Description
        };

        reply.NewsIds.AddRange(result.NewsIds.Select(x => x.ToString()));
        return reply;
    }
}