using Vitol.Enzo.CRM.ViewModel.Common;

namespace Vitol.Enzo.CRM.Domain
{
    public class EngineType:BaseRequest
    {
        public uint EngineTypeId { get; set; }//sl_enginetypecode
        public string EngineTypeCode { get; set; }//sl_enginetypecode
        public string EngineTypeName { get; set; }//sl_name
        public string EngineTypeNameTranslation { get; set; }//sl_enginetypenametranslation
        
    }
}