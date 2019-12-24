using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Vitol.Enzo.CRM.Domain;
using Vitol.Enzo.CRM.InfrastructureInterface;
using Vitol.Enzo.CRM.ServiceConnectorInterface;
namespace Vitol.Enzo.CRM.Infrastructure
{
    public class LeadInfrastructure : BaseInfrastructure, ILeadInfrastructure
    {
        //SMS TEMPLATE
        string smsT2 = string.Empty;
        string smsT3 = string.Empty;
        string smsT4 = string.Empty;
        string smsT5 = string.Empty;
        string smsT6 = string.Empty;

        //Email Template
        string templateT2 = string.Empty;
        string templateT3 = string.Empty;
        string templateT4 = string.Empty;
        string templateT5 = string.Empty;
        string templateT6 = string.Empty;


        //Email Sender Id
        string emailsenderId;
        string liveDate = string.Empty;
        string baseUrl = string.Empty;
        string returnMsg = string.Empty;
        string CRMCustomerId = string.Empty;
        string tmpEmail = string.Empty;
        string tmpRegistrationNumber = string.Empty;
        string timeZoneStr = string.Empty;
        string appointmentStatusCancelled = string.Empty;
        int TotalRecord;
        int emailSent;


        #region Constructor
        /// <summary>
        /// CustomerInfrastructure initailizes object instance.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="crmServiceConnector"></param>
        public LeadInfrastructure(ICRMServiceConnector crmServiceConnector, IConfiguration configuration, ILogger<LeadInfrastructure> logger, IHttpClientFactory clientFactory) : base(configuration, logger, clientFactory)
        {
            this.CRMServiceConnector = crmServiceConnector;
            exceptionModel.ComponentName = Enum.GetName(typeof(ComponentType), ComponentType.lead);
            //SMS Template
            smsT2 = Configuration["Lead:smsT2"];
            smsT3 = Configuration["Lead:smsT3"];
            smsT4 = Configuration["Lead:smsT4"];
            smsT5 = Configuration["Lead:smsT5"];
            smsT6 = Configuration["Lead:smsT6"];

            //Email Template
            templateT2 = Configuration["Lead:templateT2"];
            templateT3 = Configuration["Lead:templateT3"];
            templateT4 = Configuration["Lead:templateT4"];
            templateT5 = Configuration["Lead:templateT5"];
            templateT6 = Configuration["Lead:templateT6"];

            //Email Sender Id
            emailsenderId = Configuration["AzureCRM:emailSenderId"];
            liveDate = Configuration["AzureCRM:liveDate"];
            baseUrl = Configuration["AzureCRM:baseUrl"];
            timeZoneStr = Configuration["AzureCRM:timeZoneStr"];
            appointmentStatusCancelled = Configuration["AzureCRM:appointmentStatusCancelled"];

        }
        #endregion

        #region Properties and Data Members
        public ICRMServiceConnector CRMServiceConnector { get; }
        public ExceptionModel exceptionModel = new ExceptionModel();



        #endregion

        #region Interface ILeadInfrastructure Implementation

        public async Task<string> LeadUtilityService(string str)
        {
          
            exceptionModel.ActionName = Enum.GetName(typeof(ActionType), ActionType.leadUtilityService);
          
            string resultText = null;
            try
            {
                tmpEmail = "";
                tmpRegistrationNumber = "";


                string triggerType = "Lead";
                JArray records = null;
                string accessToken = await this.CRMServiceConnector.GetAccessTokenCrm(this._clientFactory);
                string inputValutionDate = string.Empty;
                DateTime startValuationDate = DateTime.Now.AddDays(-30);
                inputValutionDate= startValuationDate.ToString("yyyy-MM-dd");
                TotalRecord = 0;
                emailSent = 0;
                this.Logger.LogDebug("Lead Checkeddate: "+DateTime.Now.ToString());
                Guid appointmentCancelledId = await RetrieveAppointmentId(appointmentStatusCancelled);
                string queryLead;
                queryLead = "api/data/v9.1/contacts?$select=sl_registrationnumber,telephone1,fullname,sl_make,sl_model,sl_mprice,emailaddress1,contactid,sl_appointmentdate,_sl_appointmentstatus_value,sl_valuationcreateddate,statuscode&$filter=(_sl_appointmentstatus_value eq " + appointmentCancelledId.ToString() + " or _sl_appointmentstatus_value eq null) and sl_mprice ne null and sl_valuationcreateddate ge " + inputValutionDate + " and sl_valuationcreateddate ge " + liveDate + " and statuscode eq 1 and donotbulkemail ne true &$orderby=emailaddress1 asc,sl_registrationnumber asc,sl_valuationcreateddate desc";
                this.Logger.LogDebug("Query Lead: " + queryLead);
                if (triggerType == "Lead")
                {
                    var httpClient = this._clientFactory.CreateClient("NameClientFactory");

                    httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
                    httpClient.DefaultRequestHeaders.Add("Prefer", "return=representation");
                    httpClient.DefaultRequestHeaders.Add("Prefer", "odata.maxpagesize=20");
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    HttpResponseMessage response = await httpClient.GetAsync(base.Resource + queryLead);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseJson = await response.Content.ReadAsStringAsync();
                        dynamic contact = JsonConvert.DeserializeObject(responseJson);
                        if (contact != null)
                        {
                            records = contact.value;
                            if (records != null && records.Count > 0)
                            {
                                resultText = await LeadProcessContacts(contact,  resultText);

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
                                        resultText = await LeadProcessContacts(contact, resultText);
                                        resultText = resultText + " Page End (Last) ";
                                    }
                                    else
                                    {
                                        resultText = resultText + " Page Start ";
                                        nextpageUri = contact["@odata.nextLink"].ToString(); //This URI is already encoded.
                                        resultText = await LeadProcessContacts(contact, resultText);
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

            this.Logger.LogDebug("Total Lead Number of records : " + TotalRecord);
            this.Logger.LogDebug("Total Lead Number of Emails sent : " + emailSent);
            return "Success Record: " + resultText;
        }
       
        public async Task<string> CreateSMSActivity(Guid CustomerId, string mobileNo, string textMessage,string triggerTemplate)
        {
            exceptionModel.ActionName = Enum.GetName(typeof(ActionType), ActionType.leadUtilityService);
            string returnMsg = string.Empty;
            try
            {
                string accessToken = await this.CRMServiceConnector.GetAccessTokenCrm(this._clientFactory);
 
                string jsonObject = @"{
                'sl_trigger': 102690001,
                'sl_triggertemplate': " + triggerTemplate + @", 
                'sl_mobile': '" + mobileNo + @"', 
                ""sl_message"":" + '"' + textMessage + '"' + @",
                'regardingobjectid_contact@odata.bind': '/contacts(" + CustomerId + @")' }";
                StringContent content = new StringContent(jsonObject, Encoding.UTF8, "application/json");
                var httpClient = this._clientFactory.CreateClient("NameClientFactory");

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
                    this.Logger.LogError(exceptionModel.getExceptionFormat("SMS not sent : "+CustomerId.ToString()));
            }
            catch (Exception ex)
            {
                this.Logger.LogError(exceptionModel.getExceptionFormat(ex.ToString()));
                returnMsg = ex.ToString();
            }
            return returnMsg;
        }
        public async Task<string> CreateEmailActivity(Guid fromUserId, Guid CustomerId, Guid TemplateId,string triggerTemplate)
        {
            exceptionModel.ActionName = Enum.GetName(typeof(ActionType), ActionType.createEmailActivity);
            string returnMsg = string.Empty;
            try
            {
                string accessToken = await this.CRMServiceConnector.GetAccessTokenCrm(this._clientFactory);


                string jsonObject = @"{
                                          'TemplateId': '" + TemplateId + @"',
                                          'Regarding': {
                                            'contactid': '" + CustomerId + @"',
                                            '@odata.type': 'Microsoft.Dynamics.CRM.contact'
                                          },
                                          'Target': {
                                            'sl_trigger': 102690001,
                                            'sl_triggertemplate': " + triggerTemplate + @", 
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
                var httpClient = this._clientFactory.CreateClient("NameClientFactory");

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
                string accessToken = await this.CRMServiceConnector.GetAccessTokenCrm(this._clientFactory);
                var httpClient = this._clientFactory.CreateClient("NameClientFactory");

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
        public async Task<string> LeadProcessContacts(dynamic contact, string resultText)
        {
            string returnMsg = string.Empty;
            var globalTimezoneValue = TimeZoneInfo.FindSystemTimeZoneById(timeZoneStr);
            try
            {
                string accessToken = await this.CRMServiceConnector.GetAccessTokenCrm(this._clientFactory);
                Guid TemplateId;
               Guid fromUserId=Guid.Empty;
                if (!string.IsNullOrEmpty(emailsenderId))
                {
                    fromUserId = new Guid(emailsenderId);
                }  
                foreach (var data in contact.value)
                {
                    TotalRecord = TotalRecord + 1;
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
                            string fullname=string.Empty;
                            string make = string.Empty;
                            string model = string.Empty;
                            string mprice = string.Empty;

                            if (data.sl_valuationcreateddate.Value != null)
                            {
                                DateTime valuationcreateddate = data.sl_valuationcreateddate.Value;
                               
                                valuationcreateddate = TimeZoneInfo.ConvertTime(valuationcreateddate, globalTimezoneValue);
                                var dateTimeNowUTC = DateTime.Now.ToUniversalTime();
                                var dateTimeNowTimeZone = TimeZoneInfo.ConvertTime(dateTimeNowUTC, globalTimezoneValue);
                                //var dayDiff = dateTimeNowTimeZone.Date.Subtract(crmTimezoneDate.Date).TotalDays;

                                valuationcreateddate = valuationcreateddate.Date;
                                int totaldays;
                                totaldays = (int)dateTimeNowTimeZone.Date.Subtract(valuationcreateddate).TotalDays;
                                switch (totaldays)
                                {
                                    //Trigger 2
                                    case 3:
                                        {
                                            fullname = data.fullname != null ? data.fullname.Value : "";
                                            string emailaddress1 = data.emailaddress1 != null ? data.emailaddress1.Value : "";
                                            this.Logger.LogDebug("No of days "+ totaldays + " | Lead Trigger 2 | Name : " + fullname + " | Email: " + emailaddress1);
                                            string queryString = CustomerId.ToString() + "@" + "sl_leadtemplate2";
                                            queryString = await Encryption(queryString);
                                            bool result = await UpdateTrigger(CustomerId, "sl_leadtemplate2", queryString, queryString, baseUrl);
                                            if (!string.IsNullOrEmpty(templateT2))
                                            {
                                                TemplateId = await RetrieveTemplateId(templateT2);
                                                if (TemplateId != null)
                                                {
                                                    string result2 = await CreateEmailActivity(fromUserId, CustomerId, TemplateId, "102690000");
                                                }
                                            }

                                            if (data.telephone1 != null)
                                            {
                                                
                                                fullname = data.fullname != null ? data.fullname.Value : "";
                                                make = data.sl_make != null ? data.sl_make.Value : "";
                                                model = data.sl_model != null ? data.sl_model.Value : "";
                                                mprice = data.sl_mprice != null ? data.sl_mprice.Value : "";
                                                if(!string.IsNullOrEmpty(smsT2))
                                                {
                                                string smsMessage = smsT2;
                                                    smsMessage= smsMessage.Replace("{contactname}", fullname);
                                                    smsMessage = smsMessage.Replace("{make}", make);
                                                    smsMessage = smsMessage.Replace("{model}", model);
                                                    smsMessage = smsMessage.Replace("{valuation}", mprice);
                                                    string result1 = await CreateSMSActivity(CustomerId, data.telephone1.Value, smsMessage, "102690000");
                                                }
                                            }
                                            emailSent = emailSent + 1;
                                            break;
                                        }
                                    //Trigger 3
                                    case 5:
                                        {
                                            fullname = data.fullname != null ? data.fullname.Value : "";
                                            string emailaddress1 = data.emailaddress1 != null ? data.emailaddress1.Value : "";
                                            this.Logger.LogDebug("No of days " + totaldays + " | Lead Trigger 3 | Name : " + fullname + " | Email: " + emailaddress1);
                                            string queryString = CustomerId.ToString() + "@" + "sl_leadtemplate3";
                                            queryString = await Encryption(queryString);
                                            bool result = await UpdateTrigger(CustomerId, "sl_leadtemplate3", queryString, queryString, baseUrl);
                                            TemplateId = await RetrieveTemplateId(templateT3);
                                            if (TemplateId != null)
                                            {
                                                string result2 = await CreateEmailActivity(fromUserId, CustomerId, TemplateId, "102690001");
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
                                                    string result1 = await CreateSMSActivity(CustomerId, data.telephone1.Value, smsMessage, "102690001");
                                                }
                                            }
                                            emailSent = emailSent + 1;
                                            break;
                                        }
                                    //Trigger 4
                                    case 7:
                                        {
                                            fullname = data.fullname != null ? data.fullname.Value : "";
                                            string emailaddress1 = data.emailaddress1 != null ? data.emailaddress1.Value : "";
                                            this.Logger.LogDebug("No of days " + totaldays + " | Lead Trigger 4 | Name : " + fullname + " | Email: " + emailaddress1);
                                            string queryString = CustomerId.ToString() + "@" + "sl_leadtemplate4";
                                            queryString = await Encryption(queryString);
                                            bool result = await UpdateTrigger(CustomerId, "sl_leadtemplate4", queryString, queryString, baseUrl);
                                            TemplateId = await RetrieveTemplateId(templateT4);
                                            if (TemplateId != null)
                                            {
                                                string result2 = await CreateEmailActivity(fromUserId, CustomerId, TemplateId, "102690002");
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
                                                    string result1 = await CreateSMSActivity(CustomerId, data.telephone1.Value, smsMessage, "102690002");
                                                }
                                            }
                                            emailSent = emailSent + 1;
                                            break;
                                        }
                                    //Trigger 5
                                    case 17:
                                        {
                                            fullname = data.fullname != null ? data.fullname.Value : "";
                                            string emailaddress1 = data.emailaddress1 != null ? data.emailaddress1.Value : "";
                                            this.Logger.LogDebug("No of days " + totaldays + " | Lead Trigger 5 | Name : " + fullname + " | Email: " + emailaddress1);
                                            string queryString = CustomerId.ToString() + "@" + "sl_leadtemplate5";
                                            queryString = await Encryption(queryString);
                                            bool result = await UpdateTrigger(CustomerId, "sl_leadtemplate5", queryString, queryString, baseUrl);
                                            TemplateId = await RetrieveTemplateId(templateT5);
                                            if (TemplateId != null)
                                            {
                                                string result2 = await CreateEmailActivity(fromUserId, CustomerId, TemplateId, "102690003");
                                            }
                                            if (data.telephone1 != null)
                                            {
                                                fullname = data.fullname != null ? data.fullname.Value : "";
                                                make = data.sl_make != null ? data.sl_make.Value : "";
                                                model = data.sl_model != null ? data.sl_model.Value : "";
                                                mprice = data.sl_mprice != null ? data.sl_mprice.Value : "";
                                                if (!string.IsNullOrEmpty(smsT5))
                                                {
                                                    string smsMessage = smsT5;
                                                    smsMessage = smsMessage.Replace("{contactname}", fullname);
                                                    smsMessage = smsMessage.Replace("{make}", make);
                                                    smsMessage = smsMessage.Replace("{model}", model);
                                                    smsMessage = smsMessage.Replace("{valuation}", mprice);
                                                    string result1 = await CreateSMSActivity(CustomerId, data.telephone1.Value, smsMessage, "102690003");
                                                }
                                            }
                                            emailSent = emailSent + 1;
                                            break;
                                        }
                                    //Trigger 6
                                    case 27:
                                        {
                                            fullname = data.fullname != null ? data.fullname.Value : "";
                                            string emailaddress1 = data.emailaddress1 != null ? data.emailaddress1.Value : "";
                                            this.Logger.LogDebug("No of days " + totaldays + " | Lead Trigger 6 | Name : " + fullname + " | Email: " + emailaddress1);
                                            string queryString = CustomerId.ToString() + "@" + "sl_leadtemplate6";
                                            queryString = await Encryption(queryString);
                                            bool result = await UpdateTrigger(CustomerId, "sl_leadtemplate6", queryString, queryString, baseUrl);
                                            TemplateId = await RetrieveTemplateId(templateT6);
                                            if (TemplateId != null)
                                            {
                                                string result2 = await CreateEmailActivity(fromUserId, CustomerId, TemplateId, "102690004");
                                            }
                                            if (data.telephone1 != null)
                                            {
                                                fullname = data.fullname != null ? data.fullname.Value : "";
                                                make = data.sl_make != null ? data.sl_make.Value : "";
                                                model = data.sl_model != null ? data.sl_model.Value : "";
                                                mprice = data.sl_mprice != null ? data.sl_mprice.Value : "";
                                                if (!string.IsNullOrEmpty(smsT6))
                                                {
                                                    string smsMessage = smsT6;
                                                    smsMessage = smsMessage.Replace("{contactname}", fullname);
                                                    smsMessage = smsMessage.Replace("{make}", make);
                                                    smsMessage = smsMessage.Replace("{model}", model);
                                                    smsMessage = smsMessage.Replace("{valuation}", mprice);
                                                    string result1 = await CreateSMSActivity(CustomerId, data.telephone1.Value, smsMessage, "102690004");
                                                }
                                            }
                                            emailSent = emailSent + 1;
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
                string accessToken = await this.CRMServiceConnector.GetAccessTokenCrm(this._clientFactory);
                Guid templateId = Guid.Empty;

                JArray records = null;

                string query = "api/data/v9.1/templates?$select=createdon,title&$filter=title eq '" + templateName + "'&$orderby=createdon desc";
                dynamic template = null;

                var httpClient = this._clientFactory.CreateClient("NameClientFactory");

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
                string accessToken = await this.CRMServiceConnector.GetAccessTokenCrm(this._clientFactory);
                Guid appointmentId = Guid.Empty;

                JArray records = null;

                string query = "api/data/v9.1/sl_appointmentstatuses?$select=sl_appointmentstatusid,sl_name&$filter=sl_name eq '" + statusName + "'";
                dynamic appointment = null;

                var httpClient = this._clientFactory.CreateClient("NameClientFactory");

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
                string accessToken = await this.CRMServiceConnector.GetAccessTokenCrm(this._clientFactory);
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
                var httpClient = this._clientFactory.CreateClient("NameClientFactory");
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

        public string LeadUtilitySms(string param)
        {
            string xmlResponse = "";
            string Exception = "";
            Exception = Configuration["ERRORSMS"];
            try
            {
                var client = new RestClient(Configuration["SmartMessageAPI"]);
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddParameter("undefined", "data=" + param, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                xmlResponse = response.Content;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Exception = Exception.Replace("{Exception}", response.ErrorMessage.ToString());
                    xmlResponse = Exception;
                }
            }
            catch (Exception ex)
            {
                Exception = Exception.Replace("{Exception}", ex.ToString());
                xmlResponse = Exception;
            }

            return WebUtility.UrlEncode(xmlResponse).ToString();
        }

        #endregion
    }

}