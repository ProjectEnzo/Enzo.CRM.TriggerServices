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
    public class DealerController : ControllerBase
    {
        #region Constructor
        /// <summary>
        /// CustomerController initializes class object.
        /// </summary>
        /// <param name="DealerApplication"></param>
        public DealerController(IDealerApplication DealerApplication)
        {
            this.DealerApplication = DealerApplication;

        }
        #endregion

        #region Properties and Data Members

        public IDealerApplication DealerApplication { get; }
        #endregion

        #region API Methods

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add([FromBody] Dealer Dealer)
        {
            string newCRMDealertId = string.Empty;
            newCRMDealertId = await this.DealerApplication.Add(Dealer);
            return Ok(newCRMDealertId);
        }

        [HttpPut]
        [Route("update")]
        public async Task<bool> Update([FromBody]Dealer Dealer)
        {
            var response = await this.DealerApplication.Update(Dealer);

            return response;
        }


        #endregion
    }
}