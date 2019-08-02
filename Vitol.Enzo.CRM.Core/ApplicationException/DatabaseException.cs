using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection.Metadata;
using System.Runtime.Serialization;
using System.Text;

namespace Vitol.Enzo.CRM.Core.ApplicationException
{
    /// <summary>
    /// DatabaseException inherits Exception class and holds the additional information like command, parameter details.
    /// </summary>
    public class DatabaseException : Exception
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the DatabaseException class.
        /// </summary>
        public DatabaseException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the DatabaseException class.
        /// </summary>
        /// <param name="message">The message to display for this exception.</param>
        public DatabaseException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DatabaseException class.
        /// </summary>
        /// <param name="info">
        /// The System.Runtime.Serialization.SerializationInfo that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The System.Runtime.Serialization.StreamingContext that contains contextual information about the source or destination.
        /// </param>
        public DatabaseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            info.AddValue(Constant.ExceptionData, this.ExceptionData);
        }

        /// <summary>
        /// Initializes a new instance of the DatabaseException class.
        /// </summary>
        /// <param name="message">The error message string.</param>
        /// <param name="innerException">The inner exception reference.</param>
        public DatabaseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DatabaseException class.
        /// </summary>
        /// <param name="message">The error message string.</param>
        /// <param name="parameters"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="innerException">The inner exception reference.</param>
        public DatabaseException(string message, Exception innerException, List<DbParameter> parameters, string commandText, CommandType commandType) : base(message, innerException)
        {
            this.ExceptionData = new Dictionary<string, string>
            {
                { Constant.CommandType, commandType.ToString() },
                { Constant.CommandText, commandText }
            };

            foreach (var parameter in parameters)
            {
                this.ExceptionData.Add(parameter.ParameterName, parameter.Value.ToString());
            }
        }
        #endregion

        #region Properties and Data Members
        public IDictionary<string, string> ExceptionData { get; }
        #endregion
    }
}
