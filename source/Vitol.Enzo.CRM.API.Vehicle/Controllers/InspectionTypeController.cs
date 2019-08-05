using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vitol.Enzo.CRM.ApplicationInterface;
using Vitol.Enzo.CRM.Domain;

namespace Vitol.Enzo.CRM.API.Vehicle.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InspectionTypeController : ControllerBase
    {
        #region Constructor
        /// <summary>
        /// AppointmentController initializes class object.
        /// </summary>
        /// <param name="InspectionTypeApplication"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public InspectionTypeController(IInspectionTypeApplication InspectionTypeApplication)
        {
            this.InspectionTypeApplication = InspectionTypeApplication;
        }
        #endregion

        #region Properties and Data Members
        public IInspectionTypeApplication InspectionTypeApplication { get; }

        #endregion
        [HttpGet]
        public ActionResult<string> Get()
        {
            return "Test";
        }
        #region API Methods
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add([FromBody] InspectionType InspectionType)
        {
            var newInspectionType = await this.InspectionTypeApplication.Add(InspectionType);

            return Ok(newInspectionType);
        }

        [HttpPut]
        [Route("update")]
        public async Task<string> Update([FromBody] InspectionType InspectionType)
        {
            var response = await this.InspectionTypeApplication.Update(InspectionType);

            return response;
        }
        #endregion
    }
}