using System;
using System.Threading.Tasks;
using Vitol.Enzo.CRM.Domain;

namespace Vitol.Enzo.CRM.InfrastructureInterface
{
    public interface ICustomerInfrastructure
    {
        Task<string> Add(Customer customer);
        Task<bool> Update(Customer customer);
        Task<bool> UpdateCustomer(Guid customerId, string customer, string accessToken);
        Task<Guid> IsCustomerAlreadyCreated(uint customerId, string accessToken);
        Task<string> StartService(string str);

    }
}
