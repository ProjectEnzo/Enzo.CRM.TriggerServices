using System;
using System.Threading.Tasks;
using Vitol.Enzo.CRM.Domain;

namespace Vitol.Enzo.CRM.InfrastructureInterface
{
    public interface IOpportunityInfrastructure
    {

        Task<string> OpportunityUtilityService(string str);
        Task<string> QualifiedOpportunityServiceTrigger1(string str);
        Task<string> QualifiedOpportunityServiceTrigger5(string str);

    }
}
