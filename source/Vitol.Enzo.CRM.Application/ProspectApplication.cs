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
    public class ProspectApplication : IProspectApplication
    {

        #region Constructor
        /// <summary>
        ///prospectApplication initailizes object instance.
        /// </summary>
        /// <param name="prospectInfrastructure"></param>
        public ProspectApplication(IProspectInfrastructure prospectInfrastructure)
        {
            this.ProspectInfrastructure = prospectInfrastructure;
           // this.CRMServiceConnector = crmServiceConnector;
            
        }
        #endregion

        #region Properties and Data Members
        /// <summary>
        /// CustomerInfrastructure holds the Infrastructure object.
        /// </summary>
        public IProspectInfrastructure ProspectInfrastructure { get; }
        public ICRMServiceConnector CRMServiceConnector { get; }
        #endregion

        #region Interface IProspectApplication Implementation

        public async Task<string> ProspectUtilityService(string str)
        {
            return await this.ProspectInfrastructure.ProspectUtilityService(str);
        }


        #endregion
    }
}
