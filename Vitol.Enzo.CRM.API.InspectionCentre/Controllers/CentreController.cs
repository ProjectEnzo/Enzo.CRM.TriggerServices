using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Vitol.Enzo.CRM.ApplicationInterface;
using Vitol.Enzo.CRM.Core.Interface;
using Vitol.Enzo.CRM.Domain;

namespace Vitol.Enzo.CRM.API.InspectionCentre.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CentreController : ControllerBase
    {
        #region Constructor
        /// <summary>
        /// CentreController initializes class object.
        /// </summary>
        /// <param name="CentreApplication"></param>
        /// <param name="headerValue"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public CentreController(ICentreApplication centreApplication, IHeaderValue headerValue, IConfiguration configuration, ILogger<CentreController> logger)
        {
            this.CentreApplication = centreApplication;
            Logger = logger;
        }
        #endregion

        #region Properties and Data Members
        public ICentreApplication CentreApplication { get; }
        public ILogger<CentreController> Logger { get; }

        #endregion

        #region API Methods
        [HttpGet]
        public ActionResult<string> Get()
        {
            return "Test";
        }
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add([FromBody] Domain.Centre centre)
        {
            string newCRMCentreId = string.Empty;
            newCRMCentreId = await this.CentreApplication.Add(centre);
            return Ok(newCRMCentreId);
        }

        [HttpPut]
        [Route("update")]
        public async Task<bool> Update([FromBody] Centre centre)
        {
            return await this.CentreApplication.Update(centre);
        }
        #endregion
    }
}