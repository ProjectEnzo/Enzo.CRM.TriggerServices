using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Vitol.Enzo.CRM.Core.ApplicationException;

namespace Vitol.Enzo.CRM.Application
{
    public abstract class BaseApplication
    {
        #region Constructor
        /// <summary>
        /// BaseApplication initailizes object instance.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public BaseApplication(IConfiguration configuration, ILogger logger)
        {
            this.Configuration = configuration;
            this.Logger = logger;

            this.Logger?.LogEnterConstructor(this.GetType());
        }

        public IConfiguration Configuration { get; }
        public ILogger Logger { get; }
        #endregion
    }
}
