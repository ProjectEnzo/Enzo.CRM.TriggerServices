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
    public class ModelController : ControllerBase
    {
        #region Constructor
        /// <summary>
        /// ModelController initializes class object.
        /// </summary>
        /// <param name="makeApplication"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public ModelController(IModelApplication modelApplication)
        {
            this.ModelApplication = modelApplication;
        }
        #endregion

        #region Properties and Data Members
        public IModelApplication ModelApplication { get; }

        #endregion
        [HttpGet]
        public ActionResult<string> Get()
        {
            return "Test";
        }
        #region API Methods
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add([FromBody] Model model)
        {
            var newModel = await this.ModelApplication.Add(model);

            return Ok(newModel);
        }

        [HttpPut]
        [Route("update")]
        public async Task<bool> Update([FromBody] Model model)
        {
            var response = await this.ModelApplication.Update(model);

            return response;
        }
        #endregion
    }
}