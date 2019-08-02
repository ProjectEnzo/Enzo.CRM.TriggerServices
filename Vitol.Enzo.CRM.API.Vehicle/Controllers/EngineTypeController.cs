using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vitol.Enzo.CRM.ApplicationInterface;
using Vitol.Enzo.CRM.Domain;

namespace Vitol.Enzo.CRM.API.Vehicle.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EngineTypeController : ControllerBase
    {
        #region Constructor
        /// <summary>
        /// AppointmentController initializes class object.
        /// </summary>
        /// <param name="EngineTypeApplication"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public EngineTypeController(IEngineTypeApplication EngineTypeApplication)
        {
            this.EngineTypeApplication = EngineTypeApplication;
        }
        #endregion

        #region Properties and Data Members
        public IEngineTypeApplication EngineTypeApplication { get; }

        #endregion
        [HttpGet]
        public ActionResult<string> Get()
        {
            return "Test";
        }
        #region API Methods
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add([FromBody] EngineType EngineType)
        {
            var newEngineType = await this.EngineTypeApplication.Add(EngineType);

            return Ok(newEngineType);
        }

        [HttpPut]
        [Route("update")]
        public async Task<bool> Update([FromBody] EngineType EngineType)
        {
            var response = await this.EngineTypeApplication.Update(EngineType);

            return response;
        }
        #endregion
    }
}