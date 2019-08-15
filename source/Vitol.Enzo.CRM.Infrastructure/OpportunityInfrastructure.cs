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
    public class OpportunityInfrastructure : BaseInfrastructure, IOpportunityInfrastructure
    {
        //SMS TEMPLATE
        string smsT1 = string.Empty;
        string smsT2 = string.Empty;
        string smsT3 = string.Empty;
        string smsT4 = string.Empty;


        //Email Template
        string templateT1 = string.Empty;
        string templateT2 = string.Empty;
        string templateT3 = string.Empty;
        string templateT4 = string.Empty;


        //Email Sender Id
        string emailsenderId;
        string liveDate = string.Empty;
        string baseUrl = string.Empty;
        string tmpEmail = "";
        string tmpRegistrationNumber = "";
        #region Constructor
        /// <summary>
        /// CustomerInfrastructure initailizes object instance.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="crmServiceConnector"></param>
        public OpportunityInfrastructure(ICRMServiceConnector crmServiceConnector, IConfiguration configuration, ILogger<OpportunityInfrastructure> logger) : base(configuration, logger)
        {
            this.CRMServiceConnector = crmServiceConnector;
            exceptionModel.ComponentName = Enum.GetName(typeof(ComponentType), ComponentType.opportuntiy);
            //SMS Template
            smsT1 = Configuration["Opportunity:smsT1"];
            smsT2 = Configuration["Opportunity:smsT2"];
            smsT3 = Configuration["Opportunity:smsT3"];
            smsT4 = Configuration["Opportunity:smsT4"];


            //Email Template
            templateT1 = Configuration["Opportunity:templateT1"];
            templateT2 = Configuration["Opportunity:templateT2"];
            templateT3 = Configuration["Opportunity:templateT3"];
            templateT4 = Configuration["Opportunity:templateT4"];


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



        #endregion

        #region Interface ILeadInfrastructure Implementation

        public async Task<string> OpportunityUtilityService(string str)
        {


            exceptionModel.ActionName = Enum.GetName(typeof(ActionType), ActionType.opportunityUtilityService);
            if (str == "Token123")
            {
                return "Token Not found";
            }


            string resultText = null;
            try
            {
                tmpEmail = "";
                tmpRegistrationNumber = "";
                string triggerType = "Opportunity";
                JArray records = null;
                string accessToken = await this.CRMServiceConnector.GetAccessTokenCrm();
                string inputApointmentDate = string.Empty;
                DateTime startValuationDate = DateTime.Now.AddDays(-30);
                inputApointmentDate = startValuationDate.ToString("yyyy-MM-dd");
                DateTime currentDate = DateTime.Now;
                string currentAppointmentDate = currentDate.ToString("yyyy-MM-ddTHH:mm:ssZ");



                Guid appointmentCancelledId = await RetrieveAppointmentId("CANCELLED");
                Guid appointmentAssignedId = await RetrieveAppointmentId("ASSIGNED");
                string queryOpportunity;
                queryOpportunity = "api/data/v9.1/contacts?$select=sl_registrationnumber,telephone1,fullname,sl_make,sl_model,sl_mprice,emailaddress1,contactid,sl_appointmentdate,_sl_appointmentstatus_value,sl_valuationcreateddate,statuscode&$filter=(_sl_appointmentstatus_value ne " + appointmentCancelledId.ToString() + " and _sl_appointmentstatus_value ne " + appointmentAssignedId.ToString() + " ) and sl_appointmentdate ge " + inputApointmentDate + " and sl_appointmentdate lt " + currentAppointmentDate + " and sl_valuationcreateddate ge " + liveDate + " and statuscode eq 1 and sl_mprice ne null and donotbulkemail ne true &$orderby=emailaddress1 asc,sl_registrationnumber asc,sl_appointmentdate desc";
                if (triggerType == "Opportunity")
                {
                    HttpClient httpClient = new HttpClient();

                    httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
                    httpClient.DefaultRequestHeaders.Add("Prefer", "return=representation");
                    httpClient.DefaultRequestHeaders.Add("Prefer", "odata.maxpagesize=20");
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    HttpResponseMessage response = await httpClient.GetAsync(base.Resource + queryOpportunity);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseJson = await response.Content.ReadAsStringAsync();
                        dynamic contact = JsonConvert.DeserializeObject(responseJson);
                        if (contact != null)
                        {
                            records = contact.value;
                            if (records != null && records.Count > 0)
                            {
                                resultText = await OpportunityProcessContacts(contact, resultText);

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
                                        resultText = await OpportunityProcessContacts(contact, resultText);
                                        resultText = resultText + " Page End (Last) ";
                                    }
                                    else
                                    {
                                        resultText = resultText + " Page Start ";
                                        nextpageUri = contact["@odata.nextLink"].ToString(); //This URI is already encoded.
                                        resultText = await OpportunityProcessContacts(contact, resultText);
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
        public async Task<string> OpportunityProcessContacts(dynamic contact, string resultText)
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
                    if (data.emailaddress1.Value != null && data.sl_registrationnumber.Value != null)
                    {
                        resultText = resultText + " Email Address: " + data.emailaddress1.Value;
                        if (tmpEmail == data.emailaddress1.Value && tmpRegistrationNumber == data.sl_registrationnumber.Value)
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

                            if (data.sl_appointmentdate.Value != null)
                            {

                                DateTime appointmentdate = data.sl_appointmentdate.Value;
                                appointmentdate = appointmentdate.Date;
                                int totaldays;
                                totaldays = (int)DateTime.Now.Date.Subtract(appointmentdate).TotalDays;

                                switch (totaldays)
                                {
                                    //Trigger 1
                                    case 2:
                                        {
                                            string queryString = CustomerId.ToString() + "@" + "sl_opportunitytemplate1";
                                            queryString = await Encryption(queryString);
                                            bool result = await UpdateTrigger(CustomerId, "sl_opportunitytemplate1", queryString, queryString, baseUrl);
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
                                            string queryString = CustomerId.ToString() + "@" + "sl_opportunitytemplate2";
                                            queryString = await Encryption(queryString);
                                            bool result = await UpdateTrigger(CustomerId, "sl_opportunitytemplate2", queryString, queryString, baseUrl);
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
                                    case 14:
                                        {
                                            string queryString = CustomerId.ToString() + "@" + "sl_opportunitytemplate3";
                                            queryString = await Encryption(queryString);
                                            bool result = await UpdateTrigger(CustomerId, "sl_opportunitytemplate3", queryString, queryString, baseUrl);
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
                                    //Trigger 4
                                    case 21:
                                        {
                                            string queryString = CustomerId.ToString() + "@" + "sl_opportunitytemplate4";
                                            queryString = await Encryption(queryString);
                                            bool result = await UpdateTrigger(CustomerId, "sl_opportunitytemplate4", queryString, queryString, baseUrl);
                                            if (!string.IsNullOrEmpty(templateT4))
                                            {
                                                TemplateId = await RetrieveTemplateId(templateT4);
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
                                                if (!string.IsNullOrEmpty(smsT4))
                                                {
                                                    string smsMessage = smsT4;
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
                    tmpRegistrationNumber = data.sl_registrationnumber.Value;
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
        public async Task<Guid> RetrieveAppointmentId(string statusName)
        {
            try
            {
                string accessToken = await this.CRMServiceConnector.GetAccessTokenCrm();
                Guid appointmentId = Guid.Empty;
                JArray records = null;
                string query = "api/data/v9.1/sl_appointmentstatuses?$select=sl_appointmentstatusid,sl_name&$filter=sl_appointmentstatusname eq '" + statusName + "'";
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