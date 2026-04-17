namespace CleanCrudDemo.Application.DTOs;

public class MenuDto
{
    public Guid Id { get; set; } // DTO: id trả về client.
    public string Name { get; set; } = string.Empty; // DTO: tên menu.
    public string Description { get; set; } = string.Empty; // DTO: mô tả menu.
    public List<Guid> NewsIds { get; set; } = []; // DTO: danh sách News liên kết.
}
