namespace CleanCrudDemo.Application.DTOs;

public class MenuNewsDto
{
    public Guid MenuId { get; set; }
    public string MenuName { get; set; } = string.Empty;

    public Guid NewsId { get; set; }
    public string NewsTitle { get; set; } = string.Empty;
}