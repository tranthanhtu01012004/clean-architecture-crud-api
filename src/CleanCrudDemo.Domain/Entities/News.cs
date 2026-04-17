namespace CleanCrudDemo.Domain.Entities;

public class News
{
    public Guid Id { get; set; } // Domain Entity: khóa chính của News.
    public string Title { get; set; } = string.Empty; // Domain Entity: tiêu đề news.
    public string Content { get; set; } = string.Empty; // Domain Entity: nội dung news.
    public ICollection<MenuNews> MenuNews { get; set; } = new List<MenuNews>(); // EF Core: navigation property cho quan hệ n-n.
}
