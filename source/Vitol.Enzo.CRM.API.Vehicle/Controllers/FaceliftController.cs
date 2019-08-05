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
    public class FaceliftController : ControllerBase
    {
        #region Constructor
        /// <summary>
        /// AppointmentController initializes class object.
        /// </summary>
        /// <param name="FaceliftApplication"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public FaceliftController(IFaceliftApplication FaceliftApplication)
        {
            this.FaceliftApplication = FaceliftApplication;
        }
        #endregion

        #region Properties and Data Members
        public IFaceliftApplication FaceliftApplication { get; }

        #endregion
        [HttpGet]
        public ActionResult<string> Get()
        {
            return "Test";
        }
        #region API Methods
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add([FromBody] Facelift Facelift)
        {
            var newFacelift = await this.FaceliftApplication.Add(Facelift);

            return Ok(newFacelift);
        }

        [HttpPut]
        [Route("update")]
        public async Task<bool> Update([FromBody] Facelift Facelift)
        {
            var response = await this.FaceliftApplication.Update(Facelift);

            return response;
        }
        #endregion
    }
}