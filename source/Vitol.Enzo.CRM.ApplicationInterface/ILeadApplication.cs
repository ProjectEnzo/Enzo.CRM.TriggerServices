using System;
using System.Threading.Tasks;
using Vitol.Enzo.CRM.Domain;

namespace Vitol.Enzo.CRM.ApplicationInterface
{
    
        public interface ILeadApplication
        {
        Task<string> LeadUtilityService(string str);
        string LeadUtilitySms(string str);
    }
}
