using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Vitol.Enzo.CRM.Infrastructure.common
{
    public class AccessToken : BaseInfrastructure
    {
        public AccessToken(IConfiguration configuration, ILogger<AccessToken> logger) : base(configuration, logger)
        {
        }
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

    }
}
