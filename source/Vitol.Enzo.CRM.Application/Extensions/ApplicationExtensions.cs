using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Vitol.Enzo.CRM.ApplicationInterface;
using Vitol.Enzo.CRM.InfrastructureInterface;
using Vitol.Enzo.CRM.ServiceConnector;
using Vitol.Enzo.CRM.ServiceConnectorInterface;
using Vitol.Enzo.CRM.ServiceProviderInterface;
using Vitol.Enzo.ServiceProvider;

namespace Vitol.Enzo.CRM.Application.Extensions
{
    public static class ApplicationExtensions
    {
        /// <summary>
        /// RegisterCustomerApplication registers Application components for Customer.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterCustomerApplication(this IServiceCollection services)
        {
            services.AddSingleton<ICustomerApplication, CustomerApplication>();
         
            return services;
        }
        /// <summary>
        /// RegisterLeadApplication registers Application components for Lead.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterLeadApplication(this IServiceCollection services)
        {
            services.AddSingleton<ILeadApplication, LeadApplication>();
           
            return services;
        }
        public static IServiceCollection RegisterOpportunityApplication(this IServiceCollection services)
        {
            services.AddSingleton<IOpportunityApplication, OpportunityApplication>();

            return services;
        }
        public static IServiceCollection RegisterProspectApplication(this IServiceCollection services)
        {
            services.AddSingleton<IProspectApplication, ProspectApplication>();

            return services;
        }
        public static IServiceCollection RegisterAuctionApplication(this IServiceCollection services)
        {
            services.AddSingleton<IAuctionApplication, AuctionApplication>();

            return services;
        }


    }
}
