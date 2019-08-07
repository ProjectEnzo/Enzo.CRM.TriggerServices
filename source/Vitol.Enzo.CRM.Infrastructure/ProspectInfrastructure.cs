using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Vitol.Enzo.CRM.Domain;
using Vitol.Enzo.CRM.InfrastructureInterface;
using Vitol.Enzo.CRM.ServiceConnectorInterface;

namespace Vitol.Enzo.CRM.Infrastructure
{
    public class ProspectInfrastructure : BaseInfrastructure, IProspectInfrastructure
    {
        //SMS TEMPLATE
        string smsT1 = string.Empty;
        string smsT2 = string.Empty;
        string smsT3 = string.Empty;


        //Email Template
        string templateT1 = string.Empty;
        string templateT2 = string.Empty;
        string templateT3 = string.Empty;



        //Email Sender Id
        string emailsenderId;
        string liveDate = string.Empty;
        string baseUrl = string.Empty;
        #region Constructor
        /// <summary>
        /// CustomerInfrastructure initailizes object instance.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="crmServiceConnector"></param>
        public ProspectInfrastructure(ICRMServiceConnector crmServiceConnector, IConfiguration configuration, ILogger<ProspectInfrastructure> logger) : base(configuration, logger)
        {
            this.CRMServiceConnector = crmServiceConnector;
            exceptionModel.ComponentName = Enum.GetName(typeof(ComponentType), ComponentType.propect);
            //SMS Template
            smsT1 = Configuration["Prospect:smsT2"];
            smsT2 = Configuration["Prospect:smsT2"];
            smsT3 = Configuration["Prospect:smsT3"];

            //Email Template
            templateT1 = Configuration["Prospect:templateT2"];
            templateT2 = Configuration["Prospect:templateT2"];
            templateT3 = Configuration["Prospect:templateT3"];



            //Email Sender Id
            emailsenderId = Configuration["AzureCRM:emailSenderId"];
            liveDate = Configuration["AzureCRM:liveDate"];
            baseUrl = Configuration["AzureCRM:baseUrl"];

        }
        #endregion

        #region Properties and Data Members
        public ICRMServiceConnector CRMServiceConnector { get; }
        public ExceptionModel exceptionModel = new ExceptionModel();
        string returnMsg = string.Empty;
        string CRMCustomerId = string.Empty;
        string tmpEmail = "";
        string tmpRegistrationNumber = "";


        #endregion

        #region Interface ILeadInfrastructure Implementation

        public async Task<string> ProspectUtilityService(string str)
        {


            exceptionModel.ActionName = Enum.GetName(typeof(ActionType), ActionType.prospectUtilityService);
            if (str == "Token123")
            {
                return "Token Not found";
            }


            string resultText = null;
            try
            {
                string triggerType = "Prospect";
                JArray records = null;
                string accessToken = await this.CRMServiceConnector.GetAccessTokenCrm();
                string inputInspectionDate = string.Empty;
                DateTime startInspectionDate = DateTime.Now.AddDays(-30);
                inputInspectionDate = startInspectionDate.ToString("yyyy-MM-dd");


                Guid appointmentId = await RetrieveAppointmentId();
                //Fetch Inspections Id
                Guid InspectionCancelledId = await RetrieveInspectionStatusId("Inspection Cancelled");
                Guid AgreementNotSignedId = await RetrieveInspectionStatusId("Agreement Not Signed");

                //Fetch Vehcile purchased Id
                Guid PurchaseAgreedId = await VehiclePurchaseStatusId("Purchase Agreed");
                Guid AuctionCreatedId = await VehiclePurchaseStatusId("Auction Created");
                Guid InTransitId = await VehiclePurchaseStatusId("In Transit");


                string queryProspect;
                queryProspect = "api/data/v9.1/contacts?$select=sl_vehicleregistrationnumber,telephone1,fullname,sl_make,sl_model,sl_mprice,emailaddress1,contactid,sl_appointmentdate,_sl_inspectionstatus_value,_sl_appointmentstatus_value,sl_valuationcreateddate,sl_inspectioncreateddate,statuscode&$filter=(_sl_inspectionstatus_value eq " + AgreementNotSignedId.ToString() + " or _sl_inspectionstatus_value eq " + InspectionCancelledId.ToString() + ") and ( _sl_vehiclepurchasestatus_value  eq  " + PurchaseAgreedId.ToString() + " or _sl_vehiclepurchasestatus_value  eq  " + AuctionCreatedId.ToString() + " or _sl_vehiclepurchasestatus_value  eq " + InTransitId.ToString() + ")and sl_inspectioncreateddate ge " + inputInspectionDate + " and  sl_valuationcreateddate ge " + liveDate + " and statuscode eq 1 &$orderby=emailaddress1 asc,sl_valuationcreateddate desc";
                if (triggerType == "Prospect")
                {
                    HttpClient httpClient = new HttpClient();

                    httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
                    httpClient.DefaultRequestHeaders.Add("Prefer", "return=representation");
                    httpClient.DefaultRequestHeaders.Add("Prefer", "odata.maxpagesize=20");
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    HttpResponseMessage response = await httpClient.GetAsync(base.Resource + queryProspect);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseJson = await response.Content.ReadAsStringAsync();
                        dynamic contact = JsonConvert.DeserializeObject(responseJson);
                        if (contact != null)
                        {
                            records = contact.value;
                            if (records != null && records.Count > 0)
                            {
                                resultText = await ProspectProcessContacts(contact, resultText);

                                //Paging
                                string nextpageUri = null;

                                if (contact["@odata.nextLink"] != null)
                                    nextpageUri = contact["@odata.nextLink"].ToString(); //This URI is already encoded.

                                while (nextpageUri != null)
                                {
                                    contact = await RetrieveMultiplePaging(nextpageUri);
                                    if (contact["@odata.nextLink"] == null)
                                    {
                                        resultText = resultText + " Page Start (Last) ";
                                        nextpageUri = null;
                                        resultText = await ProspectProcessContacts(contact, resultText);
                                        resultText = resultText + " Page End (Last) ";
                                    }
                                    else
                                    {
                                        resultText = resultText + " Page Start ";
                                        nextpageUri = contact["@odata.nextLink"].ToString(); //This URI is already encoded.
                                        resultText = await ProspectProcessContacts(contact, resultText);
                                        resultText = resultText + " Page End ";
                                    }
                                }
                                //EndPaging
                            }
                            else
                            {
                                this.Logger.LogError(exceptionModel.getExceptionFormat("Contact Not found"));
                            }

                        }
                        else
                        {
                            this.Logger.LogError(exceptionModel.getExceptionFormat("Contact Not found"));
                        }
                    }
                    else
                        this.Logger.LogError(exceptionModel.getExceptionFormat(response.Content.ToString()));

                }
            }
            catch (Exception ex)
            {
                this.Logger.LogError(exceptionModel.getExceptionFormat(ex.ToString()));

            }


            return "Success Record: " + resultText;
        }

        public async Task<string> CreateSMSActivity(Guid CustomerId, string mobileNo, string textMessage)
        {
            exceptionModel.ActionName = Enum.GetName(typeof(ActionType), ActionType.leadUtilityService);
            string returnMsg = string.Empty;
            try
            {
                string accessToken = await this.CRMServiceConnector.GetAccessTokenCrm();

                string jsonObject = @"{
                'sl_trigger': 102690001,
                'sl_mobile': '" + mobileNo + @"', 
                ""sl_message"":" + '"' + textMessage + '"' + @",
                'regardingobjectid_contact@odata.bind': '/contacts(" + CustomerId + @")' }";
                StringContent content = new StringContent(jsonObject, Encoding.UTF8, "application/json");
                HttpClient httpClient = new HttpClient();

                httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
                httpClient.DefaultRequestHeaders.Add("Prefer", "return=representation");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                HttpResponseMessage response = await httpClient.PostAsync(base.Resource + "api/data/v9.1/sl_smses", content);
                if (response.IsSuccessStatusCode)
                {
                    string responseJson = await response.Content.ReadAsStringAsync();
                }
                else
                    this.Logger.LogError(exceptionModel.getExceptionFormat("SMS not sent : " + CustomerId.ToString()));
            }
            catch (Exception ex)
            {
                this.Logger.LogError(exceptionModel.getExceptionFormat(ex.ToString()));
                returnMsg = ex.ToString();
            }
            return returnMsg;
        }
        public async Task<string> CreateEmailActivity(Guid fromUserId, Guid CustomerId, Guid TemplateId)
        {
            exceptionModel.ActionName = Enum.GetName(typeof(ActionType), ActionType.createEmailActivity);
            string returnMsg = string.Empty;
            try
            {
                string accessToken = await this.CRMServiceConnector.GetAccessTokenCrm();


                string jsonObject = @"{
                                          'TemplateId': '" + TemplateId + @"',
                                          'Regarding': {
                                            'contactid': '" + CustomerId + @"',
                                            '@odata.type': 'Microsoft.Dynamics.CRM.contact'
                                          },
                                          'Target': {
                                            'sl_trigger': 102690001,
                                            'followemailuserpreference': true,
                                            'regardingobjectid_contact@odata.bind': '/contacts(" + CustomerId + @")',
                                            'email_activity_parties': [
                                              {
                                                'partyid_systemuser@odata.bind': '/systemusers(" + fromUserId + @")',
                                                'participationtypemask': 1
                                              },

                                              {
                                                'partyid_contact@odata.bind': '/contacts(" + CustomerId + @")',
                                                'participationtypemask': 2
                                              }

                                            ],

                                            '@odata.type': 'Microsoft.Dynamics.CRM.email'

                                          } 
                                        }";

                StringContent content = new StringContent(jsonObject, Encoding.UTF8, "application/json");
                HttpClient httpClient = new HttpClient();

                httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
                httpClient.DefaultRequestHeaders.Add("Prefer", "return=representation");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                HttpResponseMessage response = await httpClient.PostAsync(base.Resource + "api/data/v9.1/SendEmailFromTemplate", content);
                if (response.IsSuccessStatusCode)
                {
                    string responseJson = await response.Content.ReadAsStringAsync();
                }
                else
                    this.Logger.LogError(exceptionModel.getExceptionFormat(response.Content.ToString()));
            }
            catch (Exception ex)
            {
                this.Logger.LogError(exceptionModel.getExceptionFormat(ex.ToString()));
                returnMsg = ex.ToString();
            }
            return returnMsg;
        }
        public async Task<dynamic> RetrieveMultiplePaging(string query)
        {
            string returnMsg = string.Empty;
            dynamic contact = null;
            try
            {
                string accessToken = await this.CRMServiceConnector.GetAccessTokenCrm();
                HttpClient httpClient = new HttpClient();

                httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
                httpClient.DefaultRequestHeaders.Add("Prefer", "return=representation");
                httpClient.DefaultRequestHeaders.Add("Prefer", "odata.maxpagesize=20");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                HttpResponseMessage response = await httpClient.GetAsync(query);
                string responseJson = await response.Content.ReadAsStringAsync();
                contact = JsonConvert.DeserializeObject(responseJson);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(exceptionModel.getExceptionFormat(ex.ToString()));
                returnMsg = ex.ToString();
            }
            return contact;
        }
        public async Task<string> ProspectProcessContacts(dynamic contact, string resultText)
        {
            string returnMsg = string.Empty;
            try
            {
                string accessToken = await this.CRMServiceConnector.GetAccessTokenCrm();
                Guid TemplateId;
                Guid fromUserId = Guid.Empty;
                if (!string.IsNullOrEmpty(emailsenderId))
                {
                    fromUserId = new Guid(emailsenderId);
                }
                foreach (var data in contact.value)
                {
                    if (data.emailaddress1.Value != null && data.sl_vehicleregistrationnumber.Value != null)
                    {
                        resultText = resultText + " Email Address: " + data.emailaddress1.Value;
                        if (tmpEmail == data.emailaddress1.Value && tmpRegistrationNumber == data.sl_vehicleregistrationnumber.Value)
                        {
                            continue;
                        }
                        else
                        {
                            Guid CustomerId = (Guid)data.contactid;
                            string fullname = string.Empty;
                            string make = string.Empty;
                            string model = string.Empty;
                            string mprice = string.Empty;

                            if (data.sl_inspectioncreateddate.Value != null)
                            {

                                DateTime inspectioncreateddate = data.sl_inspectioncreateddate.Value;
                                inspectioncreateddate = inspectioncreateddate.Date;
                                int totaldays;
                                totaldays = (int)DateTime.Now.Date.Subtract(inspectioncreateddate.Date).TotalDays;

                                switch (totaldays)
                                {
                                    //Trigger 1
                                    case 2:
                                        {
                                            string queryString = CustomerId.ToString() + "@" + "sl_prospecttemplate1";
                                            queryString = await Encryption(queryString);
                                            bool result = await UpdateTrigger(CustomerId, "sl_prospecttemplate1", queryString, queryString, baseUrl);
                                            if (!string.IsNullOrEmpty(templateT1))
                                            {
                                                TemplateId = await RetrieveTemplateId(templateT1);
                                                if (TemplateId != null)
                                                {
                                                    string result2 = await CreateEmailActivity(fromUserId, CustomerId, TemplateId);
                                                }
                                            }

                                            if (data.telephone1 != null)
                                            {

                                                fullname = data.fullname != null ? data.fullname.Value : "";
                                                make = data.sl_make != null ? data.sl_make.Value : "";
                                                model = data.sl_model != null ? data.sl_model.Value : "";
                                                mprice = data.sl_mprice != null ? data.sl_mprice.Value : "";
                                                if (!string.IsNullOrEmpty(smsT1))
                                                {
                                                    string smsMessage = smsT1;
                                                    smsMessage = smsMessage.Replace("{contactname}", fullname);
                                                    smsMessage = smsMessage.Replace("{make}", make);
                                                    smsMessage = smsMessage.Replace("{model}", model);
                                                    smsMessage = smsMessage.Replace("{valuation}", mprice);
                                                    string result1 = await CreateSMSActivity(CustomerId, data.telephone1.Value, smsMessage);
                                                }
                                            }
                                            break;
                                        }
                                    //Trigger 2
                                    case 5:
                                        {
                                            string queryString = CustomerId.ToString() + "@" + "sl_prospecttemplate2";
                                            queryString = await Encryption(queryString);
                                            bool result = await UpdateTrigger(CustomerId, "sl_prospecttemplate2", queryString, queryString, baseUrl);
                                            if (!string.IsNullOrEmpty(templateT2))
                                            {
                                                TemplateId = await RetrieveTemplateId(templateT2);
                                                if (TemplateId != null)
                                                {
                                                    string result2 = await CreateEmailActivity(fromUserId, CustomerId, TemplateId);
                                                }
                                            }

                                            if (data.telephone1 != null)
                                            {

                                                fullname = data.fullname != null ? data.fullname.Value : "";
                                                make = data.sl_make != null ? data.sl_make.Value : "";
                                                model = data.sl_model != null ? data.sl_model.Value : "";
                                                mprice = data.sl_mprice != null ? data.sl_mprice.Value : "";
                                                if (!string.IsNullOrEmpty(smsT2))
                                                {

                                                    string smsMessage = smsT2;
                                                    smsMessage = smsMessage.Replace("{contactname}", fullname);
                                                    smsMessage = smsMessage.Replace("{make}", make);
                                                    smsMessage = smsMessage.Replace("{model}", model);
                                                    smsMessage = smsMessage.Replace("{valuation}", mprice);
                                                    string result1 = await CreateSMSActivity(CustomerId, data.telephone1.Value, smsMessage);
                                                }
                                            }
                                            break;
                                        }
                                    //Trigger 3
                                    case 20:
                                        {
                                            string queryString = CustomerId.ToString() + "@" + "sl_prospecttemplate3";
                                            queryString = await Encryption(queryString);
                                            bool result = await UpdateTrigger(CustomerId, "sl_prospecttemplate3", queryString, queryString, baseUrl);
                                            if (!string.IsNullOrEmpty(templateT3))
                                            {
                                                TemplateId = await RetrieveTemplateId(templateT3);
                                                if (TemplateId != null)
                                                {
                                                    string result2 = await CreateEmailActivity(fromUserId, CustomerId, TemplateId);
                                                }
                                            }
                                            if (data.telephone1 != null)
                                            {
                                                fullname = data.fullname != null ? data.fullname.Value : "";
                                                make = data.sl_make != null ? data.sl_make.Value : "";
                                                model = data.sl_model != null ? data.sl_model.Value : "";
                                                mprice = data.sl_mprice != null ? data.sl_mprice.Value : "";
                                                if (!string.IsNullOrEmpty(smsT2))
                                                {
                                                    string smsMessage = smsT3;
                                                    smsMessage = smsMessage.Replace("{contactname}", fullname);
                                                    smsMessage = smsMessage.Replace("{make}", make);
                                                    smsMessage = smsMessage.Replace("{model}", model);
                                                    smsMessage = smsMessage.Replace("{valuation}", mprice);
                                                    string result1 = await CreateSMSActivity(CustomerId, data.telephone1.Value, smsMessage);
                                                }
                                            }
                                            break;
                                        }


                                }

                            }
                        }
                    }
                    tmpEmail = data.emailaddress1.Value;
                    tmpRegistrationNumber = data.sl_vehicleregistrationnumber.Value;
                }
                returnMsg = resultText;
            }
            catch (Exception ex)
            {
                this.Logger.LogError(exceptionModel.getExceptionFormat(ex.ToString()));

            }
            return returnMsg;
        }
        public async Task<Guid> RetrieveTemplateId(string templateName)
        {
            try
            {
                string accessToken = await this.CRMServiceConnector.GetAccessTokenCrm();
                Guid templateId = Guid.Empty;

                JArray records = null;

                string query = "api/data/v9.1/templates?$select=createdon,title&$filter=title eq '" + templateName + "'&$orderby=createdon desc";
                dynamic template = null;

                HttpClient httpClient = new HttpClient();

                httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
                httpClient.DefaultRequestHeaders.Add("Prefer", "return=representation");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                HttpResponseMessage response = await httpClient.GetAsync(base.Resource + query);
                if (response.IsSuccessStatusCode)
                {
                    string responseJson = await response.Content.ReadAsStringAsync();
                    template = JsonConvert.DeserializeObject(responseJson);
                    if (template != null)
                        records = template.value;
                    else
                        return Guid.Empty;
                }
                else
                {
                    this.Logger.LogError(exceptionModel.getExceptionFormat(response.Content.ToString()));
                    return Guid.Empty;

                }


                return records != null && records.Count > 0 ? new Guid(template.value[0].templateid.ToString()) : Guid.Empty;
            }
            catch (Exception ex)
            {
                this.Logger.LogError(exceptionModel.getExceptionFormat(ex.ToString()));
                return Guid.Empty;
            }

        }
        public async Task<Guid> RetrieveAppointmentId()
        {
            try
            {
                string accessToken = await this.CRMServiceConnector.GetAccessTokenCrm();
                Guid appointmentId = Guid.Empty;

                JArray records = null;

                string query = "api/data/v9.1/sl_appointmentstatuses?$select=sl_appointmentstatusid,sl_name&$filter=sl_appointmentstatusname eq 'CANCELLED'";
                dynamic appointment = null;

                HttpClient httpClient = new HttpClient();

                httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
                httpClient.DefaultRequestHeaders.Add("Prefer", "return=representation");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                HttpResponseMessage response = await httpClient.GetAsync(base.Resource + query);
                if (response.IsSuccessStatusCode)
                {
                    string responseJson = await response.Content.ReadAsStringAsync();
                    appointment = JsonConvert.DeserializeObject(responseJson);
                    if (appointment != null)
                        records = appointment.value;
                    else
                        return Guid.Empty;
                }
                else
                {
                    this.Logger.LogError(exceptionModel.getExceptionFormat(response.Content.ToString()));
                    return Guid.Empty;
                }


                return records != null && records.Count > 0 ? new Guid(appointment.value[0].sl_appointmentstatusid.ToString()) : Guid.Empty;
            }
            catch (Exception ex)
            {
                this.Logger.LogError(exceptionModel.getExceptionFormat(ex.ToString()));
                return Guid.Empty;
            }

        }
        public async Task<Guid> RetrieveInspectionStatusId(string statusName)
        {
            try
            {
                string accessToken = await this.CRMServiceConnector.GetAccessTokenCrm();
                Guid InspectionStatusId = Guid.Empty;

                JArray records = null;

                string query = "api/data/v9.1/sl_inspectionstatuses?$select=sl_inspectionstatusid,sl_name&$filter=sl_name eq '" + statusName + "'";
                dynamic InspectionStatus = null;

                HttpClient httpClient = new HttpClient();

                httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
                httpClient.DefaultRequestHeaders.Add("Prefer", "return=representation");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                HttpResponseMessage response = await httpClient.GetAsync(base.Resource + query);
                if (response.IsSuccessStatusCode)
                {
                    string responseJson = await response.Content.ReadAsStringAsync();
                    InspectionStatus = JsonConvert.DeserializeObject(responseJson);
                    if (InspectionStatus != null)
                        records = InspectionStatus.value;
                    else
                        return Guid.Empty;
                }
                else
                {
                    this.Logger.LogError(exceptionModel.getExceptionFormat(response.Content.ToString()));
                    return Guid.Empty;
                }


                return records != null && records.Count > 0 ? new Guid(InspectionStatus.value[0].sl_inspectionstatusid.ToString()) : Guid.Empty;
            }
            catch (Exception ex)
            {
                this.Logger.LogError(exceptionModel.getExceptionFormat(ex.ToString()));
                return Guid.Empty;
            }

        }
        public async Task<Guid> VehiclePurchaseStatusId(string statusName)
        {
            try
            {
                string accessToken = await this.CRMServiceConnector.GetAccessTokenCrm();
                Guid VehiclePurchaseStatusId = Guid.Empty;

                JArray records = null;

                string query = "api/data/v9.1/sl_vehiclepurchasestatuses?$select=sl_name,sl_vehiclepurchasestatusid&$filter=sl_name eq '" + statusName + "'";
                dynamic VehiclePurchaseStatus = null;

                HttpClient httpClient = new HttpClient();

                httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
                httpClient.DefaultRequestHeaders.Add("Prefer", "return=representation");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                HttpResponseMessage response = await httpClient.GetAsync(base.Resource + query);
                if (response.IsSuccessStatusCode)
                {
                    string responseJson = await response.Content.ReadAsStringAsync();
                    VehiclePurchaseStatus = JsonConvert.DeserializeObject(responseJson);
                    if (VehiclePurchaseStatus != null)
                        records = VehiclePurchaseStatus.value;
                    else
                        return Guid.Empty;
                }
                else
                {
                    this.Logger.LogError(exceptionModel.getExceptionFormat(response.Content.ToString()));
                    return Guid.Empty;
                }


                return records != null && records.Count > 0 ? new Guid(VehiclePurchaseStatus.value[0].sl_vehiclepurchasestatusid.ToString()) : Guid.Empty;
            }
            catch (Exception ex)
            {
                this.Logger.LogError(exceptionModel.getExceptionFormat(ex.ToString()));
                return Guid.Empty;
            }

        }
        public async Task<string> Encryption(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public async Task<bool> UpdateTrigger(Guid CustomerId, string attributeName, string querystring, string unsubscribeurl, string baseUrl)
        {
            try
            {
                string accessToken = await this.CRMServiceConnector.GetAccessTokenCrm();
                unsubscribeurl = CustomerId.ToString();
                string jsonObject = @"{
                '" + attributeName + @"': true,
                ""sl_querystring"":" + '"' + querystring + '"' + @", 
                ""sl_baseurl"":" + '"' + baseUrl + '"' + @", 
                ""sl_unsubscribeurl"":" + '"' + unsubscribeurl + '"' + @" }";
                HttpRequestMessage updateRequest = new HttpRequestMessage(new HttpMethod("PATCH"), base.Resource + "api/data/v9.1/contacts(" + CustomerId + ")")
                {
                    Content = new StringContent(jsonObject, Encoding.UTF8, "application/json")
                };
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                HttpResponseMessage updateResponse = await httpClient.SendAsync(updateRequest);
                if (updateResponse.StatusCode == HttpStatusCode.NoContent) //204
                {
                    await updateResponse.Content.ReadAsStringAsync();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                this.Logger.LogError(exceptionModel.getExceptionFormat(ex.ToString()));
                return false;
            }

        }


        #endregion  
    }

}