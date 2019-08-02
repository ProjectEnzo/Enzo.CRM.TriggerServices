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
    public class BodyTypeController : ControllerBase
    {
        #region Constructor
        /// <summary>
        /// AppointmentController initializes class object.
        /// </summary>
        /// <param name="BodyTypeApplication"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public BodyTypeController(IBodyTypeApplication BodyTypeApplication)
        {
            this.BodyTypeApplication = BodyTypeApplication;
        }
        #endregion

        #region Properties and Data Members
        public IBodyTypeApplication BodyTypeApplication { get; }

        #endregion
        [HttpGet]
        public ActionResult<string> Get()
        {
            return "Test";
        }
        #region API Methods
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add([FromBody] BodyType BodyType)
        {
            var newBodyType = await this.BodyTypeApplication.Add(BodyType);

            return Ok(newBodyType);
        }

        [HttpPut]
        [Route("update")]
        public async Task<bool> Update([FromBody] BodyType BodyType)
        {
            var response = await this.BodyTypeApplication.Update(BodyType);

            return response;
        }
        #endregion
    }
}