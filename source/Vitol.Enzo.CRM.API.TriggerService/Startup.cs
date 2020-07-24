using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vitol.Enzo.API.CRM.Core.ServiceExtensions;
using Vitol.Enzo.CRM.Application.Extensions;
using Vitol.Enzo.CRM.Infrastructure.Extensions;
using Vitol.Enzo.CRM.ServiceConnector.Extensions;

namespace Vitol.Enzo.API.Customer
{
    public class Startup
    {
        #region Constructor
        /// <summary>
        /// Startup initializes class object.
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }
        #endregion
        public IConfiguration Configuration { get; }

        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        #region Methods
        /// <summary>
        /// ConfigureServices method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            //string AllowSpecificOrigins = "allowVavaCars";



            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddHttpClient("NameClientFactory", c =>
            {
                //c.BaseAddress = new Uri("https://api.github.com/");
            });
            //services.AddCors();

            #region Custom Service Registration

            services.AddSingleton(c => this.Configuration);
            
            services.RegisterCustomerApplication();
            services.RegisterLeadApplication();
            services.RegisterOpportunityApplication();
            services.RegisterProspectApplication();
            services.RegisterAuctionApplication();

 //           services.RegisterMakeApplication();

            services.RegisterCustomerInfrastructure();
            services.RegisterLeadInfrastructure();
            services.RegisterOpportunityInfrastructure();
            services.RegisterProspectInfrastructure();
            services.RegisterAuctionInfrastructure();

            services.RegisterCommonServiceConnector();

            services.RegisterCommonService();

            services.AddTransient<SqlConnection>(_ => new SqlConnection(Configuration["ConnectionStrings:Default"]));
            //services.AddCors(options =>
            //{
            //    options.AddPolicy(MyAllowSpecificOrigins,
            //    builder =>
            //    {
            //        builder.WithOrigins("https://localhost:5005/");
            //    });
            //});

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            //services.AddCors(options =>
            //{
            //    options.AddPolicy(MyAllowSpecificOrigins,
            //    builder =>
            //    {
            //        builder.WithOrigins("https://localhost:5005/");
            //    });
            //});

            //app.UseCors(builder => builder
            //        .WithOrigins("https://localhost:5005/")
            //        .AllowAnyHeader()
            //        .AllowAnyMethod()
            //);


            app.UseCors(MyAllowSpecificOrigins);

            //app.UseCors();
            app.UseHttpsRedirection();
            app.UseMvc();
           app.UseExceptionHandlingMiddleware();
        }

        #endregion
    }
}
