using System;
using System.Threading.Tasks;
using Vitol.Enzo.CRM.Domain;

namespace Vitol.Enzo.CRM.ApplicationInterface
{
    
        public interface ICustomerApplication
        {
       Task<string> Add(Customer customer);
        Task<bool> Update(Customer customer);
        Task<string> StartService(string str);
    }
}
