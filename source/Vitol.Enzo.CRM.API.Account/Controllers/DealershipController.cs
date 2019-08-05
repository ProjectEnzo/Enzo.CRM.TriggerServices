using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vitol.Enzo.CRM.ApplicationInterface;
using Vitol.Enzo.CRM.Domain;

namespace Vitol.Enzo.CRM.API.Account.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DealershipController : ControllerBase
    {
        #region Constructor
        /// <summary>
        /// CustomerController initializes class object.
        /// </summary>
        /// <param name="DealershipApplication"></param>
        public DealershipController(IDealershipApplication DealershipApplication)
        {
            this.DealershipApplication = DealershipApplication;

        }
        #endregion

        #region Properties and Data Members

        public IDealershipApplication DealershipApplication { get; }
        #endregion

        #region API Methods

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add([FromBody] Dealership Dealership)
        {
            string newCRMDealershiptId = string.Empty;
            newCRMDealershiptId = await this.DealershipApplication.Add(Dealership);
            return Ok(newCRMDealershiptId);
        }

        [HttpPut]
        [Route("update")]
        public async Task<bool> Update([FromBody]Dealership Dealership)
        {
            var response = await this.DealershipApplication.Update(Dealership);

            return response;
        }


        #endregion
    }
}