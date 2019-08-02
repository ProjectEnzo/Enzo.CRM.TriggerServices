using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vitol.Enzo.CRM.ApplicationInterface;
using Vitol.Enzo.CRM.Domain;

namespace Vitol.Enzo.CRM.API.InspectionInspectionStatus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InspectionStatusController : Controller
    {
        #region Constructor
        /// <summary>
        /// InspectionStatusController initializes class object.
        /// </summary>
        /// <param name="InspectionStatusApplication"></param>
        /// <param name="headerValue"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public InspectionStatusController(IInspectionStatusApplication InspectionStatusApplication)
        {
            this.InspectionStatusApplication = InspectionStatusApplication;
        }
        #endregion

        #region Properties and Data Members
        public IInspectionStatusApplication InspectionStatusApplication { get; }

        #endregion

        #region API Methods
        [HttpGet]
        public ActionResult<string> Get()
        {
            return "Test";
        }
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add([FromBody] InspectionStatus InspectionStatus)
        {
            string newCRMInspectionStatusId = string.Empty;
            newCRMInspectionStatusId = await this.InspectionStatusApplication.Add(InspectionStatus);
            return Ok(newCRMInspectionStatusId);
        }

        [HttpPut]
        [Route("update")]
        public async Task<bool> Update([FromBody] InspectionStatus InspectionStatus)
        {
            return await this.InspectionStatusApplication.Update(InspectionStatus);
        }
        #endregion
    }
}