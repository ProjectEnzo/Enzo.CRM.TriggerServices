using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Vitol.Enzo.CRM.ServiceProviderInterface
{
    public interface ICRMServiceProvider
    {
        #region Methods
        /// <summary>
        ///provide CRM connection.
        /// </summary>
        Task<string> GetAccessTokenCrm(IHttpClientFactory clientFactory);
        /// <summary>
        ///provide CRM connection.
        /// </summary>

        /// <summary>
        ///  GetCRMId Get LookUp Tables Data against Query.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="accessToken"></param>
        Task<string> GetCRMId(string query, string accessToken, IHttpClientFactory clientFactory);
        #endregion
    }

}
