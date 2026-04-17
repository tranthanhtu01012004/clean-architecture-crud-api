using CleanCrudDemo.Application.Abstractions;
using CleanCrudDemo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanCrudDemo.Infrastructure.Persistence;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) // EF Core: inject options cho DbContext.
    {
    }

    public DbSet<Menu> Menus => Set<Menu>(); // EF Core: expose bảng Menus.
    public DbSet<News> News => Set<News>(); // EF Core: expose bảng News.
    public DbSet<MenuNews> MenuNews => Set<MenuNews>(); // EF Core: expose bảng nối n-n.

    protected override void OnModelCreating(ModelBuilder modelBuilder) // EF Core Fluent API: cấu hình model.
    {
        base.OnModelCreating(modelBuilder); // EF Core: gọi cấu hình mặc định của base.

        modelBuilder.Entity<Menu>(entity => // EF Core Fluent API: cấu hình bảng Menu.
        {
            entity.ToTable("Menus"); // EF Core: map entity Menu vào bảng Menus.
            entity.HasKey(x => x.Id); // EF Core: thiết lập khóa chính.
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired(); // EF Core: cột Name bắt buộc, max 200.
            entity.Property(x => x.Description).HasMaxLength(1000); // EF Core: cột Description max 1000.
        });

        modelBuilder.Entity<News>(entity => // EF Core Fluent API: cấu hình bảng News.
        {
            entity.ToTable("News"); // EF Core: map entity News vào bảng News.
            entity.HasKey(x => x.Id); // EF Core: thiết lập khóa chính.
            entity.Property(x => x.Title).HasMaxLength(200).IsRequired(); // EF Core: cột Title bắt buộc.
            entity.Property(x => x.Content).HasMaxLength(4000); // EF Core: cột Content max 4000.
        });

        modelBuilder.Entity<MenuNews>(entity => // EF Core Fluent API: cấu hình bảng nối n-n.
        {
            entity.ToTable("MenuNews"); // EF Core: map entity nối vào bảng MenuNews.
            entity.HasKey(x => new { x.MenuId, x.NewsId }); // EF Core: khóa chính kép cho bảng nối.

            entity.HasOne(x => x.Menu) // EF Core: cấu hình quan hệ từ MenuNews sang Menu.
                .WithMany(x => x.MenuNews) // EF Core: một Menu có nhiều MenuNews.
                .HasForeignKey(x => x.MenuId) // EF Core: foreign key MenuId.
                .OnDelete(DeleteBehavior.Cascade); // EF Core: xóa Menu thì xóa liên kết.

            entity.HasOne(x => x.News) // EF Core: cấu hình quan hệ từ MenuNews sang News.
                .WithMany(x => x.MenuNews) // EF Core: một News có nhiều MenuNews.
                .HasForeignKey(x => x.NewsId) // EF Core: foreign key NewsId.
                .OnDelete(DeleteBehavior.Cascade); // EF Core: xóa News thì xóa liên kết.
        });
    }
}
