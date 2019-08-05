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

namespace Vitol.Enzo.CRM.API.Prospect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProspectController : ControllerBase
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
        public ProspectController(IProspectApplication prospectApplication,  IHeaderValue headerValue, IConfiguration configuration, ILogger<ProspectController> logger)
        {
            this.ProspectApplication = prospectApplication;

        }
        #endregion

        #region Properties and Data Members

         public IProspectApplication ProspectApplication { get; }
        

        #endregion

        #region API Methods

        [HttpGet]
        [Route("ProspectUtilityService")]
        public async Task<string> LeadUtilityService(string str)
        {
            var response = await this.ProspectApplication.ProspectUtilityService(str);

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