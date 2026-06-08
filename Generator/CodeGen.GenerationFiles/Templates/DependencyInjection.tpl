using {{SolutionName}}.Domain.Interfaces;
using {{SolutionName}}.Infrastructure.Data;
using {{SolutionName}}.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace {{SolutionName}}.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<I{{EntityName}}Repository, {{EntityName}}Repository>();

        return services;
    }
}
