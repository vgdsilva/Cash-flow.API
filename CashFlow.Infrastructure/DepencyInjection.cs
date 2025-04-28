using CashFlow.Infrastructure.Interface;
using CashFlow.Infrastructure.Persistence;
using CashFlow.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CashFlow;

public static class DepencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services
                .AddDatabaseContext(configuration)
                .AddRepositories();
    }

    private static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CashFlowDbContext>(options =>
        {
            string connection = configuration.GetConnectionString("DefaultConnection");
            options.UseNpgsql(connection);
        });

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped<CashFlowRepository>();
        //services.AddScoped<UserRepository>(); // Se você criar um UserRepository específico
        //services.AddScoped<TransactionRepository>();
        //services.AddScoped<CreditCardRepository>(); // Se você criar um CreditCardRepository específico
        //services.AddScoped<CashFlowUserRepository>(); // Se você criar um CashFlowUserRepository específico

        return services;
    }
}
