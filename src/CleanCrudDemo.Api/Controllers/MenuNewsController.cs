using CleanCrudDemo.Api.Models;
using CleanCrudDemo.Application.MenuNewsRelations.Commands;
using CleanCrudDemo.Application.MenuNewsRelations.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanCrudDemo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuNewsController : ControllerBase
{
    private readonly IMediator _mediator;

    public MenuNewsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllMenuNewsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("menu/{menuId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByMenuId(Guid menuId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetMenuNewsByMenuIdQuery(menuId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("news/{newsId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByNewsId(Guid newsId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetMenuNewsByNewsIdQuery(newsId), cancellationToken);
        return Ok(result);
    }

    [HttpPost("link")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Link([FromBody] MenuNewsRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new LinkMenuNewsCommand(request.MenuId, request.NewsId),
            cancellationToken);

        return result
            ? Ok(new { message = "Liên kết thành công", request.MenuId, request.NewsId })
            : BadRequest("Không thể tạo liên kết. Kiểm tra MenuId, NewsId hoặc quan hệ đã tồn tại.");
    }

    [HttpDelete("unlink")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Unlink([FromBody] MenuNewsRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UnlinkMenuNewsCommand(request.MenuId, request.NewsId),
            cancellationToken);

        return result
            ? Ok(new { message = "Huỷ liên kết thành công", request.MenuId, request.NewsId })
            : NotFound("Không tìm thấy quan hệ cần xoá.");
    }
}