using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Vitol.Enzo.CRM.ServiceConnectorInterface;
using Vitol.Enzo.CRM.ServiceProviderInterface;

namespace Vitol.Enzo.CRM.ServiceConnector
{
    public class CRMServiceConnector : ICRMServiceConnector
    {
        #region Constructor
        /// <summary>
        ///  MakeController initializes class object.
        /// </summary>
        /// <param name="crmServiceProvider"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public CRMServiceConnector(ICRMServiceProvider crmServiceProvider, IConfiguration configuration, ILogger<CRMServiceConnector> logger)
        {
            this.CRMServiceProvider = crmServiceProvider;
        }
        #region Properties and Data Members
        public ICRMServiceProvider CRMServiceProvider { get; }
        #endregion

        public Task<string> GetAccessTokenCrm()
        {
          return  CRMServiceProvider.GetAccessTokenCrm();
        }

        public Task<string> GetCRMId(string query, string accessToken)
        {
            return CRMServiceProvider.GetCRMId(query,accessToken);
        }

        #endregion

    }
}
