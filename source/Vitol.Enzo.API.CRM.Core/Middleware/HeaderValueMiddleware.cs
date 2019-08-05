using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Vitol.Enzo.CRM.Core.Interface;

namespace Vitol.Enzo.API.CRM.Core.Middleware
{
    /// <summary>
    /// HeaderValueMiddleware is used to parse header values before requested controller has been invoked.
    /// </summary>
    public class HeaderValueMiddleware : BaseMiddleware
    {
        #region Constructor
        /// <summary>
        /// HeaderValueMiddleware initializes class object.
        /// </summary>
        /// <param name="next"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public HeaderValueMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<HeaderValueMiddleware> logger) : base(next, configuration, logger)
        {
        }
        #endregion

        #region Properties and Data Members
        private const string ApplicationKeyName = "x-application-key";
        private const string AcceptLangauageKey = "Accept-Language";
        #endregion

        #region Methods
        /// <summary>
        /// Invoke method is called when middleware has been called.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="headerValue"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context, IHeaderValue headerValue)
        {
            try
            {
                if (context != null && context.Request != null && context.Request.Headers != null)
                {
                    headerValue.ApplicationKey = this.GetHeaderValue(context.Request.Headers, HeaderValueMiddleware.ApplicationKeyName);
                    headerValue.AcceptLangauage = this.GetAcceptLanguage(context.Request.Headers);
                }
            }
            catch
            {
                //TODO: only acceptable if header values can be optional.
            }

            await this.Next(context);
        }

        /// <summary>
        /// GetHeaderValue returns the value of header item by provided key.
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetHeaderValue(IHeaderDictionary headers, string key)
        {
            return headers[key];
        }

        /// <summary>
        /// GetAcceptLanguage parse and initializes the Language property with custom checks.
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        private string GetAcceptLanguage(IHeaderDictionary headers)
        {
            var language = this.GetHeaderValue(headers, HeaderValueMiddleware.AcceptLangauageKey);

            if (string.IsNullOrWhiteSpace(language) || !language.Equals(this.Configuration["General:SecondaryLanguage"], StringComparison.CurrentCultureIgnoreCase))
            {
                language = this.Configuration["General:DefaultLanguage"];
            }
            else
            {
                language = this.Configuration["General:SecondaryLanguage"];
            }

            return language;
        }
        #endregion
    }
}
