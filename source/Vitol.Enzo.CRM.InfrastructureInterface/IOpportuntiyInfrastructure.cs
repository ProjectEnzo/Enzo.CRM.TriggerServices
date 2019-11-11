using System;
using System.Threading.Tasks;
using Vitol.Enzo.CRM.Domain;

namespace Vitol.Enzo.CRM.InfrastructureInterface
{
    public interface IOpportunityInfrastructure
    {

        Task<string> OpportunityUtilityService(string str);
        Task<string> QualifiedOpportunityUtilityServiceTrigger1(string str);
        Task<string> QualifiedOpportunityUtilityServiceTrigger5(string str);

    }
}
