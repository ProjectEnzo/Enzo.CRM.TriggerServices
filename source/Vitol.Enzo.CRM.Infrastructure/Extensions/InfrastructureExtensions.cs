using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Vitol.Enzo.CRM.InfrastructureInterface;
using Vitol.Enzo.CRM.ServiceConnector;
using Vitol.Enzo.CRM.ServiceConnectorInterface;
using Vitol.Enzo.CRM.ServiceProviderInterface;
using Vitol.Enzo.ServiceProvider;

namespace Vitol.Enzo.CRM.Infrastructure.Extensions
{
  public static class InfrastructureExtensions
    {
        /// RegisterCustomerInfrastructure registers Infrastructure components for Customer.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterCustomerInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<ICustomerInfrastructure, CustomerInfrastructure>();
           
           
            return services;
        }
        /// RegisterCustomerInfrastructure registers Infrastructure components for Customer.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterLeadInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<ILeadInfrastructure, LeadInfrastructure>();


            return services;
        }
        public static IServiceCollection RegisterOpportunityInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<IOpportunityInfrastructure, OpportunityInfrastructure>();


            return services;
        }
        public static IServiceCollection RegisterProspectInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<IProspectInfrastructure, ProspectInfrastructure>();


            return services;
        }
        public static IServiceCollection RegisterAuctionInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<IAuctionInfrastructure, AuctionInfrastructure>();


            return services;
        }

    }
}
