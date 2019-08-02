using System;
using System.Collections.Generic;
using System.Text;

namespace Vitol.Enzo.CRM.Core.Interface
{
  public interface IHeaderValue
    {
        #region Properties
        string ApplicationKey { get; set; }
        string AcceptLangauage { get; set; }
        #endregion
    }
}
