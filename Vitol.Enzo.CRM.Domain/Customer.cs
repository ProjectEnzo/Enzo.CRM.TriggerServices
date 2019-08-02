using System;
using Vitol.Enzo.CRM.ViewModel.Common;

namespace Vitol.Enzo.CRM.Domain
{
    public class Customer:BaseRequest
    {
        public uint CustomerId { get; set; }
        public int ReasonCodeId { get; set; }
        public int? AgentId { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Location { get; set; }
        public string PlaceId { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }
        public string RegistrationNumber { get; set; }
        public bool? ActiveColumn { get; set; }
        public bool? IsAppointmentColumn { get; set; }
        public bool IsAppointment { get; set; }
      //  public DateTime? CreatedDate { get; set; }
        public string QuotationDate { get; set; }
        public uint? InspectionId { get; set; }

        //public int CurrentUserId { get; set; }
        //public bool Active { get; set; }
    }
}
