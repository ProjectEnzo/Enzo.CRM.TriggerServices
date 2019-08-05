using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vitol.Enzo.CRM.ApplicationInterface;
using Vitol.Enzo.CRM.Domain;

namespace Vitol.Enzo.CRM.API.InspectionInspection.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InspectionController : Controller
    {
        #region Constructor
        /// <summary>
        /// InspectionController initializes class object.
        /// </summary>
        /// <param name="InspectionApplication"></param>
        /// <param name="headerValue"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public InspectionController(IInspectionApplication InspectionApplication)
        {
            this.InspectionApplication = InspectionApplication;
        }
        #endregion

        #region Properties and Data Members
        public IInspectionApplication InspectionApplication { get; }

        #endregion

        #region API Methods
        [HttpGet]
        public ActionResult<string> Get()
        {
            return "Test";
        }
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add([FromBody] Inspection Inspection)
        {
            string newCRMInspectionId = string.Empty;
            newCRMInspectionId = await this.InspectionApplication.Add(Inspection);
            return Ok(newCRMInspectionId);
        }

        [HttpPut]
        [Route("update")]
        public async Task<bool> Update([FromBody] Inspection Inspection)
        {
            return await this.InspectionApplication.Update(Inspection);
        }
        #endregion
    }
}