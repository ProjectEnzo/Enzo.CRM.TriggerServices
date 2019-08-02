using System;
using System.Collections.Generic;
using System.Text;

namespace Vitol.Enzo.CRM.ViewModel.Common
{
    /// <summary>
    /// BaseRequestVM class holds basic request properties.
    /// </summary>
    public class BaseRequest
    {
        #region Properties and Data Members
        public bool Active { get; set; }
        public uint? CurrentUserId { get; set; }
        
        public DateTime? CreatedDate { get; set; }//sl_createddate
        public int? CreatedById { get; set; }//sl_createdbyid
        public int? ModifiedById { get; set; }//sl_modifiedbyid
        public DateTime? ModifiedDate { get; set; }//sl_modifiedbyid
        #endregion
    }
}
