using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Vitol.Enzo.CRM.Core.ApplicationException;
using System.Data.SqlClient;

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
        public BaseInfrastructure(IConfiguration configuration, ILogger logger, IHttpClientFactory clientFactory)
        {
            this.Configuration = configuration;
            this.Logger = logger;
            ClientId = this.Configuration["AzureCRM:clientId"];
            ClientSecret = this.Configuration["AzureCRM:clientSecret"];
            Resource = this.Configuration["AzureCRM:resource"];
            Authority = this.Configuration["AzureCRM:authority"];
            this.ConnectionString = this.Configuration.GetConnectionString("DefaultConnection");
            this.Logger?.LogEnterConstructor(this.GetType());
            _clientFactory = clientFactory;
        }
        protected readonly IHttpClientFactory _clientFactory;

        public IConfiguration Configuration { get; }
        public ILogger Logger { get; }

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Resource { get; set; }
        public string Authority { get; set; }
        public string ConnectionString { get; }
        protected async Task<DbDataReader> ExecuteReader(List<DbParameter> parameters, string commandText, CommandType commandType = CommandType.StoredProcedure)
        {
            DbDataReader ds;

            try
            {
                var connection = this.GetConnection();
                var cmd = this.GetCommand(connection, commandText, commandType, parameters);
                ds = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                var dbException = this.GetException(this.GetType().FullName, "ExecuteReader", ex, parameters, commandText, commandType);
                throw dbException;
            }

            return ds;

            #endregion
        }
        private DbConnection GetConnection()
        {
            DbConnection connection = new MySqlConnection(this.ConnectionString);
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            return connection;
        }
        private DbCommand GetCommand(DbConnection connection, string commandText, CommandType commandType, List<DbParameter> parameters)
        {
            var command = connection.CreateCommand();

            command.CommandText = commandText;
            command.CommandType = commandType;

            if (parameters != null && parameters.Count > 0)
            {
                command.Parameters.AddRange(parameters.ToArray());
            }

            return command;
        }

        private DatabaseException GetException(string className, string methodName, Exception exception, List<DbParameter> parameters, string commandText, CommandType commandType)
        {
            var message = $"Failed in {className}.{methodName}. {exception.Message}";
            var dbException = new DatabaseException(message, exception, parameters, commandText, commandType);

            return dbException;
        }
    }
}
