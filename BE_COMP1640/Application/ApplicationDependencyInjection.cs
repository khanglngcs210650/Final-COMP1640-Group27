using Application.Common.Behaviours;
using Application.Features.Dashboards.AdminDashboardService;
using Application.Features.Dashboards.CoordinatorDashboardService;
using Application.Features.Dashboards.ManagerDashboardService;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {


        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddScoped<IManagerDashboardService, ManagerDashboardService>();
        services.AddScoped<ICoordinatorDashboardService, CoordinatorDashboardService>();

        return services;
    }



}