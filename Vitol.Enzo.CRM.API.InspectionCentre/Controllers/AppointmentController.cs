using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vitol.Enzo.CRM.ApplicationInterface;
using Vitol.Enzo.CRM.Domain;
using Vitol.Enzo.CRM.InfrastructureInterface;

namespace Vitol.Enzo.CRM.API.InspectionCentre.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        #region Constructor
        /// <summary>
        /// AppointmentController initializes class object.
        /// </summary>
        /// <param name="AppointmentApplication"></param>
        /// <param name="headerValue"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public AppointmentController(IAppointmentApplication appointmentApplication)
        {
            this.AppointmentApplication = appointmentApplication;
        }
        #endregion

        #region Properties and Data Members
        public IAppointmentApplication AppointmentApplication { get; }

        #endregion

        #region API Methods
        [HttpGet]
        public ActionResult<string> Get()
        {
            return "Test";
        }
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add([FromBody] Appointment appointment)
        {
            string newCRMAppointmentId = string.Empty;
            newCRMAppointmentId = await this.AppointmentApplication.Add(appointment);
            return Ok(newCRMAppointmentId);
        }

        [HttpPut]
        [Route("update")]
        public async Task<bool> Update([FromBody] Appointment appointment)
        {
            return await this.AppointmentApplication.Update(appointment);
        }

        [HttpPut("cancel")]
        public async Task<bool> Cancel([FromBody] Appointment appointment)
        {
            var response = await AppointmentApplication.Cancel(appointment);
            
            return response;
        }

        #endregion
    }
}