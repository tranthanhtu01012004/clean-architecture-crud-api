namespace CleanCrudDemo.Domain.Entities;

public class MenuNews
{
    public Guid MenuId { get; set; } // EF Core: foreign key tới Menu.
    public Menu? Menu { get; set; } // EF Core: navigation property tới Menu.

    public Guid NewsId { get; set; } // EF Core: foreign key tới News.
    public News? News { get; set; } // EF Core: navigation property tới News.
}
