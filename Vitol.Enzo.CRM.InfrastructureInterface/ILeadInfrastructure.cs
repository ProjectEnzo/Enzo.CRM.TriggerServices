using System;
using System.Threading.Tasks;
using Vitol.Enzo.CRM.Domain;

namespace Vitol.Enzo.CRM.InfrastructureInterface
{
    public interface ILeadInfrastructure
    {

        Task<string> LeadUtilityService(string str);

    }
}
