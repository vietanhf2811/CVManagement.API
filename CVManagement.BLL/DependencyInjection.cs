
using CVManagement.DAL;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CVManagement.BLL;

public static class DependencyInjection
{
    public static IServiceCollection AddBLL(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDAL(configuration);

        // registy services for BLL layer here
        // example:
        // services.AddScoped<ICandidateService, CandidateService>();

        return services;
    }
}
