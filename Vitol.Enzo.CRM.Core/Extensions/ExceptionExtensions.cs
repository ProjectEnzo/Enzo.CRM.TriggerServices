using System;
using System.Collections.Generic;
using System.Text;
using Vitol.Enzo.CRM.Core.ApplicationException;

namespace Vitol.Enzo.CRM.Core.Extensions
{
    /// <summary>
    /// ExceptionExtensions contains extensions for exception handling.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// ExeceptionMessage create exception Message from given data.
        /// </summary>
        /// <param name="exceptionData"></param>
        /// <returns></returns>
        public static string ExeceptionMessage(this IDictionary<string, string> exceptionData)
        {
            var message = new StringBuilder();

            if (exceptionData != null)
            {
                message.AppendLine(Constant.ExceptionData);

                foreach (var item in exceptionData)
                {
                    message.AppendLine($"{item.Key}: {item.Value}");
                }
            }

            return message.ToString();
        }

        /// <summary>
        /// ExeceptionMessage create exception Message from given data.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="isInnerException"></param>
        /// <returns></returns>
        public static string ExeceptionMessage(this Exception ex, bool isInnerException = false)
        {
            var message = new StringBuilder();

            if (isInnerException)
            {
                message.Append(Constant.InnerExceptionMessage);
            }
            else
            {
                message.Append(Constant.ExceptionMessage);
            }

            message.AppendLine(ex.Message);

            if (isInnerException)
            {
                message.Append(Constant.InnerExceptionStackTrace);
            }
            else
            {
                message.Append(Constant.ExceptionStackTrace);
            }

            message.AppendLine(ex.StackTrace);

            var dbException = ex as DatabaseException;

            if (dbException != null && dbException.ExceptionData != null)
            {
                message.AppendLine(dbException.ExceptionData.ExeceptionMessage());
            }

            message.AppendLine(ex.StackTrace);

            if (ex.InnerException != null)
            {
                message.AppendLine(ex.InnerException.ExeceptionMessage(true));
            }

            return message.ToString();
        }
    }
}
