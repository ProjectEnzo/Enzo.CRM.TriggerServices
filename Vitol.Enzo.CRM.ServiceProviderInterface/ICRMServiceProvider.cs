using System;
using System.Threading.Tasks;

namespace Vitol.Enzo.CRM.ServiceProviderInterface
{
    public interface ICRMServiceProvider
    {
        #region Methods
        /// <summary>
        ///provide CRM connection.
        /// </summary>
        Task<string> GetAccessTokenCrm();
        /// <summary>
        ///provide CRM connection.
        /// </summary>

        /// <summary>
        ///  GetCRMId Get LookUp Tables Data against Query.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="accessToken"></param>
        Task<string> GetCRMId(string query, string accessToken);
        #endregion
    }

}
