using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Vitol.Enzo.CRM.ApplicationInterface;
using Vitol.Enzo.CRM.Core.Interface;
using Vitol.Enzo.CRM.Domain;
using Vitol.Enzo.CRM.InfrastructureInterface;

namespace Vitol.Enzo.CRM.API.Lead.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeadController : ControllerBase
    {
        #region Constructor
        /// <summary>
        /// CustomerController initializes class object.
        /// </summary>
        /// <param name="customerApplication"></param> 
        /// <param name="quotationApplication"></param>
        /// <param name="headerValue"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public LeadController(ILeadApplication leadApplication,  IHeaderValue headerValue, IConfiguration configuration, ILogger<LeadController> logger)
        {
            this.LeadApplication = leadApplication;
            this.Configuration = configuration;

        }
        #endregion

        #region Properties and Data Members

         public ILeadApplication LeadApplication { get; }
         public IConfiguration Configuration { get; }


        #endregion

        #region API Methods

        [HttpPost]
        [Route("LeadUtilityService")]
        public async Task<string> LeadUtilityService(string str)
        {
            string secretKey = Configuration.GetSection("Keys:EncryptionkeyLead").Value;
            var response = "";
            if (Request.Headers["Token"].ToString() == secretKey)
            {
                var response1 =   this.LeadApplication.LeadUtilityService(str);
            }
            else
            {
                response = HttpStatusCode.Unauthorized.ToString();
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            return response;
        }

        [HttpPost]
        [Route("LeadUtilitySMS")]
        public async Task <string>  LeadUtilitySms([FromBody]SMSEnvelope envelope)
        {
            string secretKey = Configuration.GetSection("Keys:EncryptionkeySMS").Value;
            var  response = "";
            if (Request.Headers["Token"].ToString() == secretKey)
            {
                var response1 =  this.LeadApplication.LeadUtilitySms(envelope.request);
            }
            else
            {
                response = HttpStatusCode.Unauthorized.ToString();
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            return response;
        }
        #endregion

    }
}