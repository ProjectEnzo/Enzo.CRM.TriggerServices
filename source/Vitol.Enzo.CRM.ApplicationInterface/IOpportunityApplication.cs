using System;
using System.Threading.Tasks;
using Vitol.Enzo.CRM.Domain;

namespace Vitol.Enzo.CRM.ApplicationInterface
{
    
        public interface IOpportunityApplication
    {
        Task<string> OpportunityUtilityService(string str);
        Task<string> OpportunityUtilityServicePK(string str);
    }
}
