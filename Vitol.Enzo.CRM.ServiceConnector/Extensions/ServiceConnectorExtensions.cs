using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Vitol.Enzo.CRM.ServiceConnectorInterface;
using Vitol.Enzo.CRM.ServiceProviderInterface;
using Vitol.Enzo.ServiceProvider;

namespace Vitol.Enzo.CRM.ServiceConnector.Extensions
{
    public static class ServiceConnectorExtensions
    {
        /// <summary>
        /// RegisterCommonServiceConnector registers Common Service Connector components.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>

        /// <summary>
        /// RegisterCustomerServiceConnector registers Customer Service Connector components.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        //public static IServiceCollection RegisterCustomerServiceConnector(this IServiceCollection services)
        //{
        ////    services.AddSingleton<ICRMServiceConnector, CRMServiceConnector>();

        //  //  return services;
        //}

        public static IServiceCollection RegisterCommonServiceConnector(this IServiceCollection services)
        {
            services.AddSingleton<ICRMServiceConnector, CRMServiceConnector>();
            services.AddSingleton<ICRMServiceProvider, CRMServiceProvider>();
            return services;
        }

    }
}
