using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vitol.Enzo.CRM.ApplicationInterface;
using Vitol.Enzo.CRM.Domain;

namespace Vitol.Enzo.CRM.API.InspectionInspector.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InspectorController : Controller
    {
        #region Constructor
        /// <summary>
        /// InspectorController initializes class object.
        /// </summary>
        /// <param name="InspectorApplication"></param>
        /// <param name="headerValue"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public InspectorController(IInspectorApplication InspectorApplication)
        {
            this.InspectorApplication = InspectorApplication;
        }
        #endregion

        #region Properties and Data Members
        public IInspectorApplication InspectorApplication { get; }

        #endregion

        #region API Methods
        [HttpGet]
        public ActionResult<string> Get()
        {
            return "Test";
        }
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add([FromBody] Inspector Inspector)
        {
            string newCRMInspectorId = string.Empty;
            newCRMInspectorId = await this.InspectorApplication.Add(Inspector);
            return Ok(newCRMInspectorId);
        }

        [HttpPut]
        [Route("update")]
        public async Task<bool> Update([FromBody] Inspector Inspector)
        {
            return await this.InspectorApplication.Update(Inspector);
        }
        #endregion
    }
}