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
    public class FuelTypeController : ControllerBase
    {
        #region Constructor
        /// <summary>
        /// AppointmentController initializes class object.
        /// </summary>
        /// <param name="FuelTypeApplication"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public FuelTypeController(IFuelTypeApplication FuelTypeApplication)
        {
            this.FuelTypeApplication = FuelTypeApplication;
        }
        #endregion

        #region Properties and Data Members
        public IFuelTypeApplication FuelTypeApplication { get; }

        #endregion
        [HttpGet]
        public ActionResult<string> Get()
        {
            return "Test";
        }
        #region API Methods
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add([FromBody] FuelType FuelType)
        {
            var newFuelType = await this.FuelTypeApplication.Add(FuelType);

            return Ok(newFuelType);
        }

        [HttpPut]
        [Route("update")]
        public async Task<bool> Update([FromBody] FuelType FuelType)
        {
            var response = await this.FuelTypeApplication.Update(FuelType);

            return response;
        }
        #endregion
    }
}