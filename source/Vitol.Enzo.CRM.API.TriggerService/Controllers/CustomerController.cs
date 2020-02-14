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

namespace Vitol.Enzo.CRM.API.Customer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
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
        public CustomerController(ICustomerApplication customerApplication,  IHeaderValue headerValue, IConfiguration configuration, ILogger<CustomerController> logger)
        {
            this.CustomerApplication = customerApplication;
           
        }
        #endregion

        #region Properties and Data Members

         public ICustomerApplication CustomerApplication { get; }
         

        #endregion

        #region API Methods

        [HttpGet]
        [Route("startservice")]
        public async Task<string> StartService(string str)
        {
            var response = await this.CustomerApplication.StartService(str);

            return response;
        }

        [HttpGet]
        public ActionResult<string> Get()
        {
            return "Test";
        }
        /// <summary>
        /// Index Method used to keep api alive.
        /// Azure always ping this method
        /// API Path:  /
        /// </summary>
        /// <returns>Test string</returns>
        [HttpGet("/")]
        [AllowAnonymous]
        public string Index()
        {
            return "Hello Test at: TriggerService " + DateTime.Now;
        }

        //[EnableCors("_myAllowSpecificOrigins")]


        #endregion

    }
}