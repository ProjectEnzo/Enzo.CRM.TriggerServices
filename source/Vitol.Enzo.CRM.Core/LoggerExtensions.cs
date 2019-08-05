using System;

namespace Vitol.Enzo.CRM.Core
{
    public static class LoggerExtensions
    {
        #region Constants
        private const string EnterConstructorMessage = "Entering in {0} Constructor with ApplicationKey: {1}.";
        private const string EnterMethodMessage = "Entering in {0}.{1} with ApplicationKey: {1}.\nMethod Data: {2}.";
        private const string ExitMethodMessage = "Exiting from {0}.{1} with ApplicationKey: {1}.\nMethod Data: {2}.";
        private const string ExceptionMethodMessage = "Exception with ApplicationKey: {0}.";
        #endregion

        /// <summary>
        /// LogEnterConstructor logs message as information.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="callerClass"></param>
        /// <param name="headerValue"></param>
        //public static void LogEnterConstructor(this ILogger logger, Type callerClass, IHeaderValue headerValue = null)
        //{
        //    var applicationKey = headerValue != null ? headerValue.ApplicationKey : String.Empty;
        //    var message = string.Format(LoggerExtensions.EnterConstructorMessage, callerClass.FullName, applicationKey);

        //    logger?.LogInformation(message);
        //}

        ///// <summary>
        /////  LogEnterMethod logs message as information.
        ///// </summary>
        ///// <param name="logger"></param>
        ///// <param name="callerClass"></param>
        ///// <param name="callerMethod"></param>
        ///// <param name="headerValue"></param>
        ///// <param name="dataItems"></param>
        //public static void LogEnterMethod(this ILogger logger, Type callerClass, MethodBase callerMethod, IHeaderValue headerValue, params object[] dataItems)
        //{
        //    var applicationKey = headerValue != null ? headerValue.ApplicationKey : String.Empty;
        //    var data = JsonConvert.SerializeObject(dataItems);
        //    var message = string.Format(LoggerExtensions.EnterMethodMessage, callerClass.FullName, callerMethod.Name, applicationKey, data);

        //    logger?.LogInformation(message);
        //}

        ///// <summary>
        /////  LogExitMethod logs message as information.
        ///// </summary>
        ///// <param name="logger"></param>
        ///// <param name="callerClass"></param>
        ///// <param name="callerMethod"></param>
        ///// <param name="headerValue"></param>
        ///// <param name="dataItems"></param>
        //public static void LogExitMethod(this ILogger logger, Type callerClass, MethodBase callerMethod, IHeaderValue headerValue, params object[] dataItems)
        //{
        //    var applicationKey = headerValue != null ? headerValue.ApplicationKey : String.Empty;
        //    var data = JsonConvert.SerializeObject(dataItems);
        //    var message = string.Format(LoggerExtensions.ExitMethodMessage, callerClass.FullName, callerMethod.Name, applicationKey, data);

        //    logger?.LogInformation(message);
        //}

        ///// <summary>
        ///// LogExeception logs exception details as Error.
        ///// </summary>
        ///// <param name="logger"></param>
        ///// <param name="ex"></param>
        ///// <param name="headerValue"></param>
        //public static void LogExeception(this ILogger logger, Exception ex, IHeaderValue headerValue)
        //{
        //    var applicationKey = headerValue != null ? headerValue.ApplicationKey : String.Empty;
        //    var message = string.Format(LoggerExtensions.ExceptionMethodMessage, applicationKey) + ex.ExeceptionMessage();

        //    logger?.LogError(message);
        //}
    }
}
