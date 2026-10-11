
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CVManagement.DAL;

public static class DependencyInjection
{
    public static IServiceCollection AddDAL(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<CVManagementDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")
            )
        );

        return services;
    }
}
