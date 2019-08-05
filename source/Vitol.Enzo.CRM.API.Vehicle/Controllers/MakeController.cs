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
    public class MakeController : ControllerBase
    {
        #region Constructor
        /// <summary>
        /// AppointmentController initializes class object.
        /// </summary>
        /// <param name="makeApplication"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public MakeController(IMakeApplication makeApplication)
        {
            this.MakeApplication = makeApplication;
        }
        #endregion

        #region Properties and Data Members
        public IMakeApplication MakeApplication { get; }

        #endregion
        [HttpGet]
        public ActionResult<string> Get()
        {
            return "Test";
        }
        #region API Methods
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add([FromBody] Make make)
        {
            var newMake = await this.MakeApplication.Add(make);

            return Ok(newMake);
        }

        [HttpPut]
        [Route("update")]
        public async Task<bool> Update([FromBody] Make make)
        {
            var response = await this.MakeApplication.Update(make);

            return response;
        }
        #endregion
    }
}