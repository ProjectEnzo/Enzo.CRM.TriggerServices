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
    public class DealerTypeController : ControllerBase
    {
        #region Constructor
        /// <summary>
        /// CustomerController initializes class object.
        /// </summary>
        /// <param name="DealerTypeApplication"></param>
        public DealerTypeController(IDealerTypeApplication DealerTypeApplication)
        {
            this.DealerTypeApplication = DealerTypeApplication;

        }
        #endregion

        #region Properties and Data Members

        public IDealerTypeApplication DealerTypeApplication { get; }
        #endregion

        #region API Methods

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add([FromBody] DealerType DealerType)
        {
            string newCRMDealerTypetId = string.Empty;
            newCRMDealerTypetId = await this.DealerTypeApplication.Add(DealerType);
            return Ok(newCRMDealerTypetId);
        }

        [HttpPut]
        [Route("update")]
        public async Task<bool> Update([FromBody]DealerType DealerType)
        {
            var response = await this.DealerTypeApplication.Update(DealerType);

            return response;
        }


        #endregion
    }
}