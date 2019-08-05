using System;
using System.Threading.Tasks;
using Vitol.Enzo.CRM.Domain;

namespace Vitol.Enzo.CRM.ApplicationInterface
{
    
        public interface IProspectApplication
    {
        Task<string> ProspectUtilityService(string str);
        }
}
