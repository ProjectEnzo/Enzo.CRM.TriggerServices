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
    public class TransmissionTypeController : ControllerBase
    {
        #region Constructor
        /// <summary>
        /// AppointmentController initializes class object.
        /// </summary>
        /// <param name="TransmissionTypeApplication"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public TransmissionTypeController(ITransmissionTypeApplication TransmissionTypeApplication)
        {
            this.TransmissionTypeApplication = TransmissionTypeApplication;
        }
        #endregion

        #region Properties and Data Members
        public ITransmissionTypeApplication TransmissionTypeApplication { get; }

        #endregion
        [HttpGet]
        public ActionResult<string> Get()
        {
            return "Test";
        }
        #region API Methods
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add([FromBody] TransmissionType TransmissionType)
        {
            var newTransmissionType = await this.TransmissionTypeApplication.Add(TransmissionType);

            return Ok(newTransmissionType);
        }

        [HttpPut]
        [Route("update")]
        public async Task<bool> Update([FromBody] TransmissionType TransmissionType)
        {
            var response = await this.TransmissionTypeApplication.Update(TransmissionType);

            return response;
        }
        #endregion
    }
}