using {{SolutionName}}.Domain.Interfaces;
using {{SolutionName}}.Infrastructure.Data;
using {{SolutionName}}.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace {{SolutionName}}.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<SqlConnectionFactory>();
        services.AddScoped<I{{EntityName}}Repository, {{EntityName}}Repository>();

        return services;
    }
}
