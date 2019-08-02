using System;
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
        Task<string> GetAccessTokenCrm();
        Task<string> GetCRMId(string query, string accessToken);

         #endregion
    }
}
