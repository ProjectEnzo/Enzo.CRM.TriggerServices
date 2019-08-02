using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vitol.Enzo.CRM.ApplicationInterface;
using Vitol.Enzo.CRM.Domain;

namespace Vitol.Enzo.CRM.API.Vehicle.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrimLevelController : ControllerBase
    {
        #region Constructor
        /// <summary>
        /// AppointmentController initializes class object.
        /// </summary>
        /// <param name="TrimLevelApplication"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public TrimLevelController(ITrimLevelApplication TrimLevelApplication)
        {
            this.TrimLevelApplication = TrimLevelApplication;
        }
        #endregion

        #region Properties and Data Members
        public ITrimLevelApplication TrimLevelApplication { get; }

        #endregion
        [HttpGet]
        public ActionResult<string> Get()
        {
            return "Test";
        }
        #region API Methods
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add([FromBody] TrimLevel TrimLevel)
        {
            var newTrimLevel = await this.TrimLevelApplication.Add(TrimLevel);

            return Ok(newTrimLevel);
        }

        [HttpPut]
        [Route("update")]
        public async Task<bool> Update([FromBody] TrimLevel TrimLevel)
        {
            var response = await this.TrimLevelApplication.Update(TrimLevel);

            return response;
        }
        #endregion
    }
}