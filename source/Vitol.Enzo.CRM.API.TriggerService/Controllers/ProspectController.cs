﻿using System;
using System.Net;
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
            this.Configuration = configuration;

        }
        #endregion

        #region Properties and Data Members

         public IProspectApplication ProspectApplication { get; }
         public IConfiguration Configuration { get; }

        #endregion

        #region API Methods

        [HttpPost]
        [Route("ProspectUtilityService")]
        public async Task<string> ProspectUtilityService(string str)
        {

            string secretKey = Configuration.GetSection("Keys:EncryptionkeyProspect").Value;
            var response = "";
            if (Request.Headers["Token"].ToString() == secretKey)
            {
                var response1 =  this.ProspectApplication.ProspectUtilityService(str);
            }
            else
            {
                response = HttpStatusCode.Unauthorized.ToString();
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            return response;

        }

        [HttpPost]
        [Route("HotProspectUtilityService")]
        public async Task<string> HotProspectUtilityService(string str)
        {

            string secretKey = Configuration.GetSection("Keys:EncryptionkeyProspect").Value;
            var response = "";
            if (Request.Headers["Token"].ToString() == secretKey)
            {
                var response1 = this.ProspectApplication.HotProspectUtilityService(str);
            }
            else
            {
                response = HttpStatusCode.Unauthorized.ToString();
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
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