using System;
using System.Collections.Generic;
using System.Text;
using Vitol.Enzo.CRM.Core.Interface;

namespace Vitol.Enzo.CRM.Core.Model
{
    public class HeaderValue: IHeaderValue
    {
        #region Properties
        public string ApplicationKey { get; set; }
        public string AcceptLangauage { get; set; }
        #endregion
    }
}
