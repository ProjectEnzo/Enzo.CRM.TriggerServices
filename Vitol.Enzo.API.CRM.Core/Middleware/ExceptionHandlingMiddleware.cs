using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Vitol.Enzo.CRM.Core;
using Vitol.Enzo.CRM.Core.ApplicationException;
using Vitol.Enzo.CRM.Core.Interface;

namespace Vitol.Enzo.API.CRM.Core.Middleware
{
    /// <summary>
    /// HeaderValueMiddleware is used to parse header values before requested controller has been invoked.
    /// </summary>
    public class ExceptionHandlingMiddleware : BaseMiddleware
    {
        #region Constructor
        /// <summary>
        /// ErrorHandlingMiddleware initializes class object.
        /// </summary>
        /// <param name="next"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public ExceptionHandlingMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<ExceptionHandlingMiddleware> logger) : base(next, configuration, logger)
        {
        }
        #endregion

        #region Properties and Data Members

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
                await this.Next(context);
            }
            catch (Exception ex)
            {
                await this.HandleExceptionAsync(context, ex, headerValue);
            }
        }

        /// <summary>
        /// HandleExceptionAsync creates response in case of exception.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        /// <param name="headerValue"></param>
        /// <returns></returns>
        private Task HandleExceptionAsync(HttpContext context, Exception exception, IHeaderValue headerValue)
        {
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected

            if (exception is EntryPointNotFoundException)
            {
                code = HttpStatusCode.NotFound;
            }
            else if (exception is AccessViolationException)
            {
                code = HttpStatusCode.Forbidden;
            }
            else if (exception is UnauthorizedAccessException)
            {
                code = HttpStatusCode.Unauthorized;
            }
            else if (exception is DatabaseException)
            {
                code = HttpStatusCode.BadRequest;
            }

            var result = JsonConvert.SerializeObject(new { error = exception.Message });

            context.Response.ContentType = Constant.JsonContentType;
            context.Response.StatusCode = (int)code;

            base.Logger?.LogExeception(exception, headerValue);

            return context.Response.WriteAsync(result);
        }
        #endregion
    }
}
