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
    public class LeadApplication :  ILeadApplication
    {

        #region Constructor
        /// <summary>
        ///leadApplication initailizes object instance.
        /// </summary>
        /// <param name="leadInfrastructure"></param>
        public LeadApplication(ILeadInfrastructure leadInfrastructure)
        {
            this.LeadInfrastructure = leadInfrastructure;
           // this.CRMServiceConnector = crmServiceConnector;
            
        }
        #endregion

        #region Properties and Data Members
        /// <summary>
        /// CustomerInfrastructure holds the Infrastructure object.
        /// </summary>
        public ILeadInfrastructure LeadInfrastructure { get; }
        public ICRMServiceConnector CRMServiceConnector { get; }
        #endregion

        #region Interface ILeadApplication Implementation

        public async Task<string> LeadUtilityService(string str)
        {
            return await this.LeadInfrastructure.LeadUtilityService(str);
        }
        #endregion
    }
}
