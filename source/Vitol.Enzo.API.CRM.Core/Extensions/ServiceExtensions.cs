using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.Text;
using Vitol.Enzo.API.CRM.Core.Middleware;
using Vitol.Enzo.CRM.Core.Interface;
using Vitol.Enzo.CRM.Core.Model;
namespace Vitol.Enzo.API.CRM.Core.ServiceExtensions
{
    /// <summary>
    /// ServiceExtensions class provides implementation for extension methods.
    /// </summary>
    public static class ServiceExtensions
    {
        private const string SwaggerPath = "/swagger/v1/swagger.json";

        /// <summary>
        /// RegisterCommonService registers common components.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterCommonService(this IServiceCollection services)
        {
            services.AddScoped<IHeaderValue, HeaderValue>();
           // services.AddScoped<IUser, User>();

            return services;
        }

        ///// <summary>
        ///// RegisterJWTAuthenticationService registers common Authentication for JWT.
        ///// </summary>
        ///// <param name="services"></param>
        ///// <returns></returns>
        //public static IServiceCollection RegisterJWTAuthenticationService(this IServiceCollection services, IConfiguration configuration)
        //{

        //    services.AddAuthentication(x =>
        //    {
        //        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        //    })
        //     .AddJwtBearer(cfg =>
        //     {
        //         cfg.RequireHttpsMetadata = false;
        //         cfg.SaveToken = true;
        //         cfg.TokenValidationParameters = new TokenValidationParameters()
        //         {
        //             //ValidateIssuerSigningKey = true,
        //             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Encryptionkey"])),
        //             ValidateAudience = false,
        //             ValidateLifetime = true,
        //             ValidIssuer = configuration["Jwt:Issuer"],
        //             //ValidAudience = Configuration["Jwt:Audience"],
        //             //IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Key"])),
        //         };
        //     });

        //    return services;
        //}

        /// <summary>
        /// UseHeaderValueMiddleware registers HeaderValueMiddleware.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseHeaderValueMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<HeaderValueMiddleware>();
        }

        /// <summary>
        /// UseExceptionHandlingMiddleware registers ExceptionHandlingMiddleware.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }

        /// <summary>
        /// Adds a delegate for configuring the provided Microsoft.Extensions.Logging.ILoggingBuilder. This may be called multiple times.
        /// </summary>
        /// <param name="hostBuilder">The Microsoft.AspNetCore.Hosting.IWebHostBuilder to configure.</param>
        /// <returns>The Microsoft.AspNetCore.Hosting.IWebHostBuilder.</returns>
        public static IWebHostBuilder ConfigureLogging(this IWebHostBuilder hostBuilder)
        {
            //Default settings
            //hostBuilder.ConfigureLogging((hostingContext, logging) =>
            // {
            //     logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
            //     logging.AddConsole();
            //     logging.AddDebug();
            // });

            hostBuilder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.SetMinimumLevel(LogLevel.Error);
            })
            .UseNLog();

            return hostBuilder;
        }


        /// <summary>
        /// UseCorsMiddleware registers UseCors.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCorsMiddleware(this IApplicationBuilder app)
        {
            return app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().AllowCredentials());
        }

        /// <summary>
        /// UseTokenValidatorMiddleware registers Token Parser
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        
        
    }
}
