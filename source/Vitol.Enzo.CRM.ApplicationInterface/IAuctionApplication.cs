using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Vitol.Enzo.CRM.ApplicationInterface
{
    public  interface IAuctionApplication
    {
        Task<string> AuctionUtilityService();
    }
}
