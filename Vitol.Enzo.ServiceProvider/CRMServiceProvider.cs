using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Vitol.Enzo.CRM.ServiceProvider;
using Vitol.Enzo.CRM.ServiceProviderInterface;

namespace Vitol.Enzo.ServiceProvider
{
    public class CRMServiceProvider: BaseServiceProvider, ICRMServiceProvider
    {
        #region Constructor
        /// <summary>
        ///  MakeController initializes class object.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public CRMServiceProvider(IConfiguration configuration, ILogger<CRMServiceProvider> logger) : base(configuration, logger)
        {

        }
        #endregion

        #region Methods
        /// <summary>
        ///  GetAccessTokenCrm get CRM Access Token.
        /// </summary>
        public async Task<string> GetAccessTokenCrm()
        {
            Dictionary<string, string> values = new Dictionary<string, string>
           {
               {"grant_type", "client_credentials"},
               {"client_id", base.ClientId},
               {"client_secret",base.ClientSecret},
               {"resource", this.Resource}
           };

            FormUrlEncodedContent content = new FormUrlEncodedContent(values);
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.PostAsync(base.Authority, content);

            string responseJson = await response.Content.ReadAsStringAsync();
            dynamic responseObj = JsonConvert.DeserializeObject(responseJson);
            return responseObj.access_token;
        }
        /// <summary>
        ///  GetCRMId Get LookUp Tables Data against Query.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="accessToken"></param>
        public async Task<string> GetCRMId(string query, string accessToken)
        {
            HttpClient httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
            httpClient.DefaultRequestHeaders.Add("Prefer", "return=representation");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            HttpResponseMessage response = await httpClient.GetAsync(base.Resource + "api/data/v9.1/" + query);

            string responseJson = await response.Content.ReadAsStringAsync();
            return responseJson;
        }

    }

    #endregion
}
