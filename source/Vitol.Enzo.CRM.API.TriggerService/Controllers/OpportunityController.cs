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

namespace Vitol.Enzo.CRM.API.Opportunity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpportunityController : ControllerBase
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
        public OpportunityController(IOpportunityApplication opportunityApplication,  IHeaderValue headerValue, IConfiguration configuration, ILogger<OpportunityController> logger)
        {
            this.OpportunityApplication = opportunityApplication;
           
        }
        #endregion

        #region Properties and Data Members

         public IOpportunityApplication OpportunityApplication { get; }
         

        #endregion

        #region API Methods

        [HttpGet]
        [Route("OpportunityUtilityService")]
        public async Task<string> OpportunityUtilityService(string str)
        {
            var response = await this.OpportunityApplication.OpportunityUtilityService(str);

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