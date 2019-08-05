using System;
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

        }
        #endregion

        #region Properties and Data Members

         public ILeadApplication LeadApplication { get; }


        #endregion

        #region API Methods

        [HttpGet]
        [Route("LeadUtilityService")]
        public async Task<string> LeadUtilityService(string str)
        {
            var response = await this.LeadApplication.LeadUtilityService(str);

            return response;
        }

        [HttpGet]
        public ActionResult<string> Get()
        {
            return "Test";
        }
  

        #endregion

    }
}