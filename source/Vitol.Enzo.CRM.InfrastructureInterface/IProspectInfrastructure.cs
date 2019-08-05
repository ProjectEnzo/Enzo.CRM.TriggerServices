using System;
using System.Threading.Tasks;
using Vitol.Enzo.CRM.Domain;

namespace Vitol.Enzo.CRM.InfrastructureInterface
{
    public interface IProspectInfrastructure
    {

        Task<string> ProspectUtilityService(string str);

    }
}
