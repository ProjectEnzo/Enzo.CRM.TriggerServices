using System;
using System.Threading.Tasks;
using Vitol.Enzo.CRM.Domain;

namespace Vitol.Enzo.CRM.ApplicationInterface
{
    
        public interface IOpportunityApplication
    {
        Task<string> QualifiedOpportunityServiceTrigger14(string str);
        Task<string> QualifiedOpportunityServiceTrigger5(string str);
        Task<string> QualifiedOpportunityServiceTrigger1(string str);

        Task<string> OpportunityUtilityServicePK(string str);
    }
}
