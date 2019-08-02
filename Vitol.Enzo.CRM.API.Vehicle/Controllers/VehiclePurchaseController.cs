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
    public class VehiclePurchaseController : ControllerBase
    {
        #region Constructor
        /// <summary>
        /// AppointmentController initializes class object.
        /// </summary>
        /// <param name="VehiclePurchaseApplication"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public VehiclePurchaseController(IVehiclePurchaseApplication VehiclePurchaseApplication)
        {
            this.VehiclePurchaseApplication = VehiclePurchaseApplication;
        }
        #endregion

        #region Properties and Data Members
        public IVehiclePurchaseApplication VehiclePurchaseApplication { get; }

        #endregion
        [HttpGet]
        public ActionResult<string> Get()
        {
            return "Test";
        }
        #region API Methods
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add([FromBody] VehiclePurchase VehiclePurchase)
        {
            var newVehiclePurchase = await this.VehiclePurchaseApplication.Add(VehiclePurchase);

            return Ok(newVehiclePurchase);
        }

        [HttpPut]
        [Route("update")]
        public async Task<bool> Update([FromBody] VehiclePurchase VehiclePurchase)
        {
            var response = await this.VehiclePurchaseApplication.Update(VehiclePurchase);

            return response;
        }
        #endregion
    }
}