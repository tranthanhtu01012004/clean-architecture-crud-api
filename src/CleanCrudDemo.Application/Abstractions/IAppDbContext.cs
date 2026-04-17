using CleanCrudDemo.Domain.Entities; // Import entity từ Domain
using Microsoft.EntityFrameworkCore; // Dùng DbSet<> của EF Core
using NewsEntity = CleanCrudDemo.Domain.Entities.News; // Đặt alias để tránh trùng với namespace News

namespace CleanCrudDemo.Application.Abstractions; // Namespace của layer Application

public interface IAppDbContext // Interface để Application làm việc với DbContext
{
    DbSet<Menu> Menus { get; } // EF Core: bảng Menus
    DbSet<NewsEntity> News { get; } // EF Core: bảng News, dùng alias để tránh lỗi trùng tên
    DbSet<MenuNews> MenuNews { get; } // EF Core: bảng nối many-to-many
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default); // EF Core: lưu thay đổi xuống DB
}