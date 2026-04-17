namespace CleanCrudDemo.Application.DTOs;

public class NewsDto
{
    public Guid Id { get; set; } // DTO: id trả về client.
    public string Title { get; set; } = string.Empty; // DTO: tiêu đề news.
    public string Content { get; set; } = string.Empty; // DTO: nội dung news.
    public List<Guid> MenuIds { get; set; } = []; // DTO: danh sách Menu liên kết.
}
