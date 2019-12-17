using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Vitol.Enzo.CRM.ServiceConnectorInterface
{
    public interface ICRMServiceConnector
    {
        #region Methods
        /// <summary>
        ///provide CRM connection.
        /// </summary>
       
        /// <returns></returns>
        Task<string> GetAccessTokenCrm(IHttpClientFactory clientFactory);
        Task<string> GetCRMId(string query, string accessToken, IHttpClientFactory clientFactory);

         #endregion
    }
}
