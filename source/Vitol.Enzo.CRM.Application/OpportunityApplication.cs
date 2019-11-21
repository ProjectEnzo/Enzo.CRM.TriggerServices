using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Vitol.Enzo.CRM.ApplicationInterface;
using Vitol.Enzo.CRM.Domain;
using Vitol.Enzo.CRM.InfrastructureInterface;
using Vitol.Enzo.CRM.ServiceConnectorInterface;

namespace Vitol.Enzo.CRM.Application
{
    public class OpportunityApplication : IOpportunityApplication
    {

        #region Constructor
        /// <summary>
        ///leadApplication initailizes object instance.
        /// </summary>
        /// <param name="opportunityInfrastructure"></param>
        public OpportunityApplication(IOpportunityInfrastructure opportunityInfrastructure)
        {
            this.OpportunityInfrastructure = opportunityInfrastructure;
           // this.CRMServiceConnector = crmServiceConnector;
            
        }
        #endregion

        #region Properties and Data Members
        /// <summary>
        /// CustomerInfrastructure holds the Infrastructure object.
        /// </summary>
        public IOpportunityInfrastructure OpportunityInfrastructure { get; }
        public ICRMServiceConnector CRMServiceConnector { get; }
        #endregion

        #region Interface IOpportunityApplication Implementation

        public async Task<string> QualifiedOpportunityServiceTrigger14(string str)
        {
            return await this.OpportunityInfrastructure.QualifiedOpportunityServiceTrigger14(str);
        }
        public async Task<string> QualifiedOpportunityServiceTrigger1(string str)
        {
            return await this.OpportunityInfrastructure.QualifiedOpportunityServiceTrigger1(str);
        }
        public async Task<string> QualifiedOpportunityServiceTrigger5(string str)
        {
            return await this.OpportunityInfrastructure.QualifiedOpportunityServiceTrigger5(str);
        }
        public async Task<string> OpportunityUtilityServicePK(string str)
        {
            return await this.OpportunityInfrastructure.OpportunityUtilityServicePK(str);
        }


        #endregion
    }
}
