using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Vitol.Enzo.CRM.Domain
{
    public enum ComponentType
    {
        lead,
        customer,
        propect,
        opportuntiy,
        Auction

    }
    public enum ActionType
    {
        add,
        update,
        cancel,
        createSMSActivity,
        createEmailActivity,
        retrieveMultiplePaging,
        leadProcessContacts,
        retrieveTemplateId,
        retrieveAppointmentId,
        leadUtilityService,
        opportunityUtilityService,
        prospectUtilityService,
        OpportunityUtilityServiceTrigger5,
        OpportunityUtilityServiceTrigger1,
        Upsert
    }

}
    
