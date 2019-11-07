using System;
using System.Threading.Tasks;
using Vitol.Enzo.CRM.Domain;

namespace Vitol.Enzo.CRM.InfrastructureInterface
{
    public interface ILeadInfrastructure
    {

        Task<string> LeadUtilityService(string str);
        Task<string> LeadUtilityServicePK(string str);
        string LeadUtilitySms(string str);

    }
}
