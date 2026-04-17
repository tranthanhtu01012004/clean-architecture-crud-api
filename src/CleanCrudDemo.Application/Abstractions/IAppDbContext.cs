using Microsoft.EntityFrameworkCore;
using MenuEntity = CleanCrudDemo.Domain.Entities.Menu;
using NewsEntity = CleanCrudDemo.Domain.Entities.News;
using MenuNewsEntity = CleanCrudDemo.Domain.Entities.MenuNews;

namespace CleanCrudDemo.Application.Abstractions;

public interface IAppDbContext
{
    DbSet<MenuEntity> Menus { get; }
    DbSet<NewsEntity> News { get; }
    DbSet<MenuNewsEntity> MenuNews { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}