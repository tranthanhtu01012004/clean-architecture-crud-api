namespace CleanCrudDemo.Domain.Entities;

public class Menu
{
    public Guid Id { get; set; } // Domain Entity: khóa chính của Menu.
    public string Name { get; set; } = string.Empty; // Domain Entity: tên menu.
    public string Description { get; set; } = string.Empty; // Domain Entity: mô tả menu.
    public ICollection<MenuNews> MenuNews { get; set; } = new List<MenuNews>(); // EF Core: navigation property cho quan hệ n-n.
}
