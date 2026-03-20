using Asisya.Products.Application.Interfaces;
using Asisya.Products.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Asisya.Products.API.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}
