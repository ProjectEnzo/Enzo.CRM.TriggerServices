using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Vitol.Enzo.CRM.ServiceConnector
{
    /// <summary>
    /// BaseConectorService class serves as base class for all Service Connectors.
    /// </summary>
    public class BaseServiceConnector
    {
        #region Constructor
        /// <summary>
        /// APIBaseController initializes class object.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public BaseServiceConnector(IConfiguration configuration, ILogger logger)
        {
            this.Configuration = configuration;

            this.Logger = logger;

          //  this.Logger?.LogEnterConstructor(this.GetType());
        }

        #endregion

        #region Properties and Data Members
        public IConfiguration Configuration { get; }

        protected ILogger Logger { get; }
        #endregion
    }
}
