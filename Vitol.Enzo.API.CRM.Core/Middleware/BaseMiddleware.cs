using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Vitol.Enzo.API.CRM.Core.Middleware
{
    /// <summary>
    /// BaseMiddleware is base class for middleware implementations.
    /// </summary>
    public abstract class BaseMiddleware
    {
        #region Constructor
        /// <summary>
        /// BaseMiddleware initializes class object.
        /// </summary>
        /// <param name="next"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        protected BaseMiddleware(RequestDelegate next, IConfiguration configuration, ILogger logger)
        {
            this.Next = next;
            this.Configuration = configuration;
            this.Logger = logger;
        }
        #endregion

        #region Properties and Data Members
        protected RequestDelegate Next { get; }
        public IConfiguration Configuration { get; }
        protected ILogger Logger { get; }
        #endregion

        #region Methods

        #endregion
    }
}
