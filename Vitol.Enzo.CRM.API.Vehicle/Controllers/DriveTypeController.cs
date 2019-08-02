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
    public class DriveTypeController : ControllerBase
    {
        #region Constructor
        /// <summary>
        /// AppointmentController initializes class object.
        /// </summary>
        /// <param name="DriveTypeApplication"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public DriveTypeController(IDriveTypeApplication DriveTypeApplication)
        {
            this.DriveTypeApplication = DriveTypeApplication;
        }
        #endregion

        #region Properties and Data Members
        public IDriveTypeApplication DriveTypeApplication { get; }

        #endregion
        [HttpGet]
        public ActionResult<string> Get()
        {
            return "Test";
        }
        #region API Methods
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add([FromBody] DriveType DriveType)
        {
            var newDriveType = await this.DriveTypeApplication.Add(DriveType);

            return Ok(newDriveType);
        }

        [HttpPut]
        [Route("update")]
        public async Task<bool> Update([FromBody] DriveType DriveType)
        {
            var response = await this.DriveTypeApplication.Update(DriveType);

            return response;
        }
        #endregion
    }
}