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
    public class AccountController : ControllerBase
    {
        #region Constructor
        /// <summary>
        /// AccountController initializes class object.
        /// </summary>
        /// <param name="userApplication"></param>
       
        public AccountController(IUserApplication userApplication)
        {
            this.UserApplication = userApplication;
           
        }
        #endregion

        #region Properties and Data Members

        public IUserApplication UserApplication { get; }
      
        #endregion

        #region API Methods

        [HttpGet]
        public ActionResult<string> Get()
        {
            return "Test";
        }


        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add([FromBody] ApplicationUser applicationUser)
        {
            string newCRMapplicationUser = string.Empty;
            newCRMapplicationUser = await this.UserApplication.Add(applicationUser);
            return Ok(newCRMapplicationUser);
        }

        [HttpPut]
        [Route("update")]
        public async Task<bool> Update([FromBody] ApplicationUser applicationUser)
        {
            var response = await this.UserApplication.Update(applicationUser);

            return response;
        }

        #endregion
    }
}