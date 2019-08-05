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
    public class CustomerApplication :  ICustomerApplication
    {

        #region Constructor
        /// <summary>
        /// CustomerApplication initailizes object instance.
        /// </summary>
        /// <param name="customerInfrastructure"></param>
        public CustomerApplication(ICustomerInfrastructure customerInfrastructure)
        {
            this.CustomerInfrastructure = customerInfrastructure;
           // this.CRMServiceConnector = crmServiceConnector;
            
        }
        #endregion

        #region Properties and Data Members
        /// <summary>
        /// CustomerInfrastructure holds the Infrastructure object.
        /// </summary>
        public ICustomerInfrastructure CustomerInfrastructure { get; }
        public ICRMServiceConnector CRMServiceConnector { get; }
        #endregion

        #region Interface ICustomerApplication Implementation
        public async Task<string> Add(Customer customer)
        {
            return await this.CustomerInfrastructure.Add(customer);
        }

        public async Task<bool> Update(Customer customer)
        {
            return await this.CustomerInfrastructure.Update(customer);
        }
        public async Task<string> StartService(string str)
        {
            return await this.CustomerInfrastructure.StartService(str);
        }


        #endregion
    }
}
