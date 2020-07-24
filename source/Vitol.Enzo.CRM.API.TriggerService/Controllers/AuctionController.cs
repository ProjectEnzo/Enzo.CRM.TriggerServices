using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Vitol.Enzo.CRM.ApplicationInterface;
using Vitol.Enzo.CRM.Core.Interface;

namespace Vitol.Enzo.CRM.API.TriggerService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionController : ControllerBase
    {
        public AuctionController(IAuctionApplication auctionApplication, IHeaderValue headerValue, IConfiguration configuration, ILogger<AuctionController> logger)
        {
            this.AuctionApplication = auctionApplication;
            this.Configuration = configuration;

        }

        public IAuctionApplication AuctionApplication { get; }
        public IConfiguration Configuration { get; }

        [HttpPost]
        [Route("AuctionUtilityService")]
        public async Task<string> AuctionUtilityService()
        {
            string secretKey = Configuration.GetSection("Keys:EncriptionKeyAuction").Value;
            var response="";
            if (Request.Headers["Token"].ToString() == secretKey)
            {
                response = await this.AuctionApplication.AuctionUtilityService();
            }
            else
            {
                response = HttpStatusCode.Unauthorized.ToString();
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            return response;
        }
    }
}