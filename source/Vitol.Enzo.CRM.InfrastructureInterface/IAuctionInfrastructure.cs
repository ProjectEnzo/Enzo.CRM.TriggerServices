using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Vitol.Enzo.CRM.InfrastructureInterface
{
    public interface IAuctionInfrastructure
    {
        Task<string> AuctionUtilityService();
    }
}
