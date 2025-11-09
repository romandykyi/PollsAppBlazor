using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json.Serialization;

namespace PollsAppBlazor.Server.Extensions;

public static class APIServiceCollectionExtensions
{
    public static IServiceCollection ConfigureControllers(this IServiceCollection services, bool addAntiForgery)
    {
        services
            .AddControllers(options =>
            {
                if (addAntiForgery)
                {
                    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                }
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            })
            .ConfigureApiBehaviorOptions(x => { x.SuppressMapClientErrors = true; });

        return services;
    }

    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please, enter access token (JWT)",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });

            OpenApiSecurityScheme scheme = new()
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };

            options.AddSecurityRequirement(new OpenApiSecurityRequirement { { scheme, Array.Empty<string>() } });

            options.SwaggerDoc("v1", new()
            {
                Title = "Polls App",
                Version = "v1"
            });

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

        return services;
    }
}
