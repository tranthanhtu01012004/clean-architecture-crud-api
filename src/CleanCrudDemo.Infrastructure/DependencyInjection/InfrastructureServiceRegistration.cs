using CleanCrudDemo.Application.Abstractions;
using CleanCrudDemo.Infrastructure.Messaging;
using CleanCrudDemo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanCrudDemo.Infrastructure.DependencyInjection;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("SqlServer")));

        services.AddScoped<IAppDbContext>(provider =>
            provider.GetRequiredService<AppDbContext>());

        services.AddScoped<IEventPublisher>(_ => new RabbitMqPublisher(
            configuration["RabbitMq:HostName"] ?? "localhost",
            configuration["RabbitMq:UserName"] ?? "guest",
            configuration["RabbitMq:Password"] ?? "guest"));

        return services;
    }
}