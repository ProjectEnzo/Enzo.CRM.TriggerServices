using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Vitol.Enzo.CRM.Core.ApplicationException;

namespace Vitol.Enzo.CRM.Infrastructure
{
    public abstract class BaseInfrastructure
    {
        #region Constructor
        /// <summary>
        /// BaseApplication initailizes object instance.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public BaseInfrastructure(IConfiguration configuration, ILogger logger)
        {
            this.Configuration = configuration;
            this.Logger = logger;
            ClientId = this.Configuration["AzureCRM:clientId"];
            ClientSecret = this.Configuration["AzureCRM:clientSecret"];
            Resource = this.Configuration["AzureCRM:resource"];
            Authority = this.Configuration["AzureCRM:authority"];

            this.Logger?.LogEnterConstructor(this.GetType());
        }

        public IConfiguration Configuration { get; }
        public ILogger Logger { get; }

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Resource { get; set; }
        public string Authority { get; set; }

        #endregion
    }
}
