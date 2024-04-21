using Application.Common.Interfaces;
using Domain.Entities;
using Hangfire;
using Infrastructure.Email;
using Infrastructure.Identity.Services;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Interceptors;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Infrastructure;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddPersistence(configuration);
        services.AddIdentity(configuration);
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        return services;
    }

    private static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

            //options.UseSqlServer(configuration.GetConnectionString("SQLServerConnection"))
            //    .LogTo(s => Debug.WriteLine(s))
            //    .EnableDetailedErrors()
            //    .EnableSensitiveDataLogging();


            options.UseSqlite(configuration.GetConnectionString("SQLiteConnection"))
                .LogTo(s => Debug.WriteLine(s))
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging();
        });

        services.AddHangfire(cfg =>
        {
            cfg.UseInMemoryStorage();
            cfg.UseSerializerSettings(new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
        });

        services.AddHangfireServer();
    }

    private static void AddIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        //services.AddIdentityCore<ApplicationUser>()
        //     .AddRoles<IdentityRole>()
        //     .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("COMP1640")
        //     .AddEntityFrameworkStores<ApplicationDbContext>()
        //     .AddDefaultTokenProviders();


        services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>("COMP1640")
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<DataProtectionTokenProviderOptions>(opt =>
        {
            opt.TokenLifespan = TimeSpan.FromDays(2);
        });

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Append("Token-Expired", "true");
                        }

                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        var serializedError = JsonSerializer.SerializeToUtf8Bytes(new { title = "You are not authorized" });
                        var errorJson = Encoding.UTF8.GetString(serializedError);
                        context.Response.WriteAsync(errorJson);
                        return Task.CompletedTask;
                    },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = 403;
                        context.Response.ContentType = "application/json";
                        var serializedError = JsonSerializer.SerializeToUtf8Bytes(new { title = "You are not authorized to access this resource" });
                        var errorJson = Encoding.UTF8.GetString(serializedError);
                        context.Response.WriteAsync(errorJson);
                        return Task.CompletedTask;
                    }

                };
            });

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ILinkGenerator, LinkGenerator>();
        services.AddScoped<IFileService, FileService.FileService>();
        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
    }


}