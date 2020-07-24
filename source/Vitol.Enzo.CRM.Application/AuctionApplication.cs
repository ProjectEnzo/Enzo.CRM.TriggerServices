using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Vitol.Enzo.CRM.ApplicationInterface;
using Vitol.Enzo.CRM.InfrastructureInterface;
using Vitol.Enzo.CRM.ServiceConnectorInterface;

namespace Vitol.Enzo.CRM.Application
{
    class AuctionApplication: IAuctionApplication
    {
        #region Constructor
        /// <summary>
        ///AuctionApplication initailizes object instance.
        /// </summary>
        /// <param name="leadInfrastructure"></param>
        public AuctionApplication(IAuctionInfrastructure auctionInfrastructure)
        {
            this.AuctionInfrastructure = auctionInfrastructure;
            // this.CRMServiceConnector = crmServiceConnector;

        }
        public IAuctionInfrastructure AuctionInfrastructure { get; }
        public ICRMServiceConnector CRMServiceConnector { get; }

        public async Task<string> AuctionUtilityService()
        {
            return await this.AuctionInfrastructure.AuctionUtilityService();
        }
        #endregion

    }
}
