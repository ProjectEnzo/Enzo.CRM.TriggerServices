using System;
using System.Collections.Generic;
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

namespace Vitol.Enzo.CRM.API.InspectionCentre
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //To Be Tested 
            services.AddCors();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.RegisterInspectorApplication();
            services.RegisterInspectorInfrastructure();

            services.RegisterCentreApplication();
            services.RegisterCentreInfrastructure();

            services.RegisterInspectionStatusApplication();
            services.RegisterInspectionStatusInfrastructure();

            services.RegisterInspectionApplication();
            services.RegisterInspectionInfrastructure();

            services.RegisterAppointmentApplication();
            services.RegisterAppointmentInfrastructure();

            services.RegisterCommonServiceConnector();
            services.RegisterCommonService();
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
            //To Be Tested
            app.UseCors(
                builder=>builder.WithOrigins("https://whatmobile.com"));
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
