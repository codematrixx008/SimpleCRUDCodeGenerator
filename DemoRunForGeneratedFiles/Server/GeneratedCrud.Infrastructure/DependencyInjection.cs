using GeneratedCrud.Domain.Interfaces;
using GeneratedCrud.Infrastructure.Data;
using GeneratedCrud.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GeneratedCrud.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IEmployeeRepository, EmployeeRepository>();

        return services;
    }
}
