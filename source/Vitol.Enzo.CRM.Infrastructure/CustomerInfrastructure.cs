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
    public class CustomerInfrastructure : BaseInfrastructure, ICustomerInfrastructure
    {

        #region Constructor
        /// <summary>
        /// CustomerInfrastructure initailizes object instance.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="crmServiceConnector"></param>
        public CustomerInfrastructure(ICRMServiceConnector crmServiceConnector, IConfiguration configuration, ILogger<CustomerInfrastructure> logger, IHttpClientFactory clientFactory) : base(configuration, logger, clientFactory)
        {
            this.CRMServiceConnector = crmServiceConnector;
            exceptionModel.ComponentName = Enum.GetName(typeof(ComponentType), ComponentType.customer);
        }
        #endregion

        #region Properties and Data Members
        public ICRMServiceConnector CRMServiceConnector { get; }
        public ExceptionModel exceptionModel = new ExceptionModel();
        string returnMsg = string.Empty;
        string CRMCustomerId = string.Empty;

      //  decimal Latitude = 0;
      //  decimal Longitude = 0;
        DateTime? CreatedDate = null;
        DateTime? ModifiedDate = null;
        DateTime? NullDate = null;

        #endregion

        #region Interface ICustomerInfrastructure Implementation
        public async Task<string> Add(Customer customer)
        {
            exceptionModel.ActionName = Enum.GetName(typeof(ActionType), ActionType.add);
            exceptionModel.Id = (int)customer.CustomerId;
            try
            {
                string accessToken = await this.CRMServiceConnector.GetAccessTokenCrm(this._clientFactory);
              //  Longitude = customer.Longitude.HasValue ? customer.Longitude.Value : 0;
               // Latitude = customer.Latitude.HasValue ? customer.Latitude.Value : 0;
                CreatedDate = customer.CreatedDate.HasValue ? customer.CreatedDate.Value : NullDate;

                string jsonObject = @"{
                                    'lastname': '" + customer.CustomerName + @"', 
                                    'emailaddress1': '" + customer.Email + @"', 
                                    'telephone1': '" + customer.PhoneNumber + @"', 
                                    'address1_line1': '" + customer.Location + @"', 
                                    'sl_customeridexternal': " + customer.CustomerId + @",     
                                    'sl_placeid': '" + customer.PlaceId + @"',        
                                    'sl_registrationnumber': '" + customer.RegistrationNumber + @"'";
 
                if (CreatedDate.HasValue) jsonObject += @" ,'sl_createddate': '" + CreatedDate.Value + @"'";
                if (customer.CreatedById.HasValue) jsonObject += @" ,'sl_createdbyid': '" + customer.CreatedById + @"'";
                if (customer.AgentId.HasValue) jsonObject += @" ,'sl_agentid': '" + customer.AgentId + @"'";
                //if (customer.Longitude.HasValue) jsonObject += @" ,'address1_longitude': " + customer.Longitude + @"";
                //if (customer.Latitude.HasValue) jsonObject += @" ,'address1_latitude': " + customer.Latitude + @"";
                if (customer.Longitude.HasValue) jsonObject += @" ,'sl_longitude': " + customer.Longitude + @"";
                if (customer.Latitude.HasValue) jsonObject += @" ,'sl_latitude': " + customer.Latitude + @"";
                jsonObject += @" }";

                Guid CustomerId = await IsCustomerAlreadyCreated(customer.CustomerId, accessToken);
                if (CustomerId != null && CustomerId != Guid.Empty)
                    return "Customer with Id " + customer.CustomerId + " already exists in CRM";

                StringContent content = new StringContent(jsonObject, Encoding.UTF8, "application/json");
                var httpClient = this._clientFactory.CreateClient("NameClientFactory");

                httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
                httpClient.DefaultRequestHeaders.Add("Prefer", "return=representation");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                HttpResponseMessage response = await httpClient.PostAsync(base.Resource + "api/data/v9.1/contacts?$select=sl_customeridexternal", content);
                if (response.IsSuccessStatusCode)
                {
                    string responseJson = await response.Content.ReadAsStringAsync();
                    dynamic responseObj = JsonConvert.DeserializeObject(responseJson);
                    CRMCustomerId = responseObj.contactid.ToString();
                }
                    else
                    throw new CrmHttpResponseException(response.Content);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(exceptionModel.getExceptionFormat(ex.ToString()));
                returnMsg = ex.ToString();
            }
            return CRMCustomerId;

        }

        public async Task<bool> Update(Customer customer)
        {
            bool result = false;
            exceptionModel.ActionName = Enum.GetName(typeof(ActionType), ActionType.update);
            exceptionModel.Id = (int)customer.CustomerId;
            try
            {
                string accessToken = await this.CRMServiceConnector.GetAccessTokenCrm(this._clientFactory);

            //    Longitude = customer.Longitude.HasValue ? customer.Longitude.Value : 0;
             //   Latitude = customer.Latitude.HasValue ? customer.Latitude.Value : 0;
                ModifiedDate = customer.ModifiedDate.HasValue ? customer.ModifiedDate.Value  : NullDate;

                using (var httpClient = this._clientFactory.CreateClient("NameClientFactory"))
                {
                    string jsonObject = @"{
                                    'lastname': '" + customer.CustomerName + @"', 
                                    'emailaddress1': '" + customer.Email + @"', 
                                    'telephone1': '" + customer.PhoneNumber + @"', 
                                    'address1_line1': '" + customer.Location + @"', 
                                    'sl_customeridexternal': " + customer.CustomerId + @",     
                                    'sl_placeid': '" + customer.PlaceId + @"',        
                                    'sl_registrationnumber': '" + customer.RegistrationNumber + @"'";

                    if (ModifiedDate.HasValue) jsonObject += @" ,'sl_modifieddate': '" + ModifiedDate.Value + @"'";
                    if (customer.ModifiedById.HasValue) jsonObject += @" ,'sl_modifiedbyid': '" + customer.ModifiedById + @"'";
                    if (customer.AgentId.HasValue) jsonObject += @" ,'sl_agentid': '" + customer.AgentId + @"'";
                    if (customer.Longitude.HasValue) jsonObject += @" ,'sl_longitude': " + customer.Longitude + @"";
                    if (customer.Latitude.HasValue) jsonObject += @" ,'sl_latitude': " + customer.Latitude + @"";
                    jsonObject += @" }";

                    Guid CustomerId = await IsCustomerAlreadyCreated(customer.CustomerId, accessToken);

                    if (CustomerId != null && CustomerId != Guid.Empty)
                    {
                        result= await UpdateCustomer(CustomerId, jsonObject, accessToken);
                    }
                }

            }
            catch (Exception ex)
            {
                this.Logger.LogError(exceptionModel.getExceptionFormat(ex.ToString()));
                returnMsg = ex.ToString();
            }
            return result;
        }
    
      
        public async Task<bool> UpdateCustomer(Guid CustomerId, string customer, string accessToken)
        {
            HttpRequestMessage updateRequest = new HttpRequestMessage(new HttpMethod("PATCH"), base.Resource + "api/data/v9.1/contacts(" + CustomerId + ")")
            {
                Content = new StringContent(customer, Encoding.UTF8, "application/json")
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

        public async Task<Guid> IsCustomerAlreadyCreated(uint CustomerId, string accessToken)
        {
            if (CustomerId <= 0)
                throw new ArgumentException("CustomerId ID must be provided.", nameof(CustomerId));

            var httpClient = this._clientFactory.CreateClient("NameClientFactory");

            httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
            httpClient.DefaultRequestHeaders.Add("Prefer", "return=representation");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            HttpResponseMessage response = await httpClient.GetAsync(base.Resource + "api/data/v9.1/contacts?$select=fullname,sl_customeridexternal&$filter=sl_customeridexternal eq " + CustomerId);

            string responseJson = await response.Content.ReadAsStringAsync();
            dynamic contact = JsonConvert.DeserializeObject(responseJson);
            JArray records = contact.value;
            return records.Count > 0 ? new Guid(contact.value[0].contactid.ToString()) : Guid.Empty;
        }
        public async Task<string> StartService(string str)
        {
            string triggerType = "Lead";
            JArray records=null;
            string resultText = null;
            string accessToken = await this.CRMServiceConnector.GetAccessTokenCrm(this._clientFactory);
            string inputValutionDate = "2019-07-15";
            DateTime startValuationDate = DateTime.Now.AddDays(-30);
            inputValutionDate = startValuationDate.ToShortDateString();
            string queryT0;
            string queryLead;
            queryLead = "api/data/v9.1/contacts?$select=telephone1,fullname,sl_make,sl_model,sl_mprice,emailaddress1,contactid,sl_appointmentdate,_sl_appointmentstatus_value,sl_valuationcreateddate,statuscode&$filter=(_sl_appointmentstatus_value eq C4693B89-26A3-E911-A836-000D3AB18D29 or _sl_appointmentstatus_value eq null) and sl_valuationcreateddate ge " + inputValutionDate + " and statuscode eq 1 &$orderby=emailaddress1 asc,sl_valuationcreateddate desc";

            queryT0 = "api/data/v9.1/contacts?$select=emailaddress1,contactid,sl_appointmentdate,_sl_appointmentstatus_value,sl_valuationcreateddate,statuscode&$filter=(_sl_appointmentstatus_value eq C4693B89-26A3-E911-A836-000D3AB18D29 or _sl_appointmentstatus_value eq null) and sl_valuationcreateddate ge "+inputValutionDate+" and statuscode eq 1 &$orderby=emailaddress1 asc,sl_valuationcreateddate desc";
            string queryAll;
            queryAll = "api/data/v9.1/contacts?$select=emailaddress1,contactid,sl_appointmentdate,_sl_appointmentstatus_value,sl_valuationcreateddate,statuscode&$filter=(_sl_appointmentstatus_value eq C4693B89-26A3-E911-A836-000D3AB18D29 or _sl_appointmentstatus_value eq null) and sl_valuationcreateddate ge " + inputValutionDate + " and statuscode eq 1 &$orderby=emailaddress1 asc,sl_valuationcreateddate desc";
            if (triggerType == "Lead")
            {
                var httpClient = this._clientFactory.CreateClient("NameClientFactory");

                httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
                httpClient.DefaultRequestHeaders.Add("Prefer", "return=representation");
                httpClient.DefaultRequestHeaders.Add("Prefer", "odata.maxpagesize=2");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                HttpResponseMessage response = await httpClient.GetAsync(base.Resource + queryLead);

                string responseJson = await response.Content.ReadAsStringAsync();
                dynamic contact = JsonConvert.DeserializeObject(responseJson);
               
                resultText = await LeadProcessContacts(contact, accessToken, resultText);
            
                //Paging
                string nextpageUri = null;
             
                if (contact["@odata.nextLink"] != null)
                    nextpageUri = contact["@odata.nextLink"].ToString(); //This URI is already encoded.

                while (nextpageUri != null )
                {
                    contact = await RetrieveMultiplePaging(nextpageUri);
                    if (contact["@odata.nextLink"] == null)
                        nextpageUri = null;
                    else
                    {
                        nextpageUri = contact["@odata.nextLink"].ToString(); //This URI is already encoded.
                        resultText = await LeadProcessContacts(contact, accessToken, resultText);
                    }
                }
                //EndPaging


           
                
            }
            else if (triggerType == "All")
            {
                var httpClient = this._clientFactory.CreateClient("NameClientFactory");

                httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
                httpClient.DefaultRequestHeaders.Add("Prefer", "return=representation");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                HttpResponseMessage response = await httpClient.GetAsync(base.Resource + queryAll);

                string responseJson = await response.Content.ReadAsStringAsync();
                dynamic contact = JsonConvert.DeserializeObject(responseJson);
                records = contact.value;
                var dateNow = DateTime.Now;
                var triggerLastUpdate = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, 23, 59, 00);
                string tmpEmail = "";
                foreach (var data in contact.value)
                {

                    if (data.emailaddress1.Value != null)
                    {
                        if (tmpEmail == data.emailaddress1.Value)
                        {
                            continue;
                        }
                        else
                        {
                            Guid CustomerId = (Guid)data.contactid;
                            bool result;
                            if (data.sl_valuationcreateddate.Value != null)
                            {

                                DateTime valuationcreateddate = data.sl_valuationcreateddate.Value;
                                int totaldays;
                                totaldays = (int)DateTime.Now.Date.Subtract(valuationcreateddate.Date).TotalDays;

                                switch (totaldays)
                                {
                                    //Trigger 1
                                    case 1:
                                        {
                                            string jsonObject = @"{'sl_trigger': 102690001, 'sl_triggerlastupdate' : '" + triggerLastUpdate + "','sl_numtotaldays':" + totaldays + "}";
                                            result = await UpdateTrigger(CustomerId, jsonObject, accessToken);
                                            break;
                                        }
                                    //Trigger 2
                                    case 3:
                                        {
                                            string jsonObject = @"{'sl_trigger': 102690002, 'sl_triggerlastupdate' : '" + triggerLastUpdate + "','sl_numtotaldays':" + totaldays + "}";
                                            result = await UpdateTrigger(CustomerId, jsonObject, accessToken);
                                            break;
                                        }
                                    //Trigger 3
                                    case 5:
                                        {
                                            string jsonObject = @"{'sl_trigger': 102690003, 'sl_triggerlastupdate' : '" + triggerLastUpdate + "','sl_numtotaldays':" + totaldays + "}";
                                            result = await UpdateTrigger(CustomerId, jsonObject, accessToken);
                                            break;
                                        }
                                    //Trigger 4
                                    case 7:
                                        {
                                            string jsonObject = @"{'sl_trigger': 102690004, 'sl_triggerlastupdate' : '" + triggerLastUpdate + "','sl_numtotaldays':" + totaldays + "}";
                                            result = await UpdateTrigger(CustomerId, jsonObject, accessToken);
                                            break;
                                        }
                                    //Trigger 5
                                    case 17:
                                        {
                                            string jsonObject = @"{'sl_trigger': 102690005, 'sl_triggerlastupdate' : '" + triggerLastUpdate + "','sl_numtotaldays':" + totaldays + "}";
                                            result = await UpdateTrigger(CustomerId, jsonObject, accessToken);
                                            break;
                                        }
                                    //Trigger 6
                                    case 27:
                                        {
                                            string jsonObject = @"{'sl_trigger': 102690006, 'sl_triggerlastupdate' : '" + triggerLastUpdate + "','sl_numtotaldays':" + totaldays + "}";
                                            result = await UpdateTrigger(CustomerId, jsonObject, accessToken);
                                            break;
                                        }
                                    default:
                                        {
                                            string jsonObject = @"{'sl_trigger': 102690008, 'sl_triggerlastupdate' : '" + triggerLastUpdate + "','sl_numtotaldays':" + totaldays + "}";
                                            result = await UpdateTrigger(CustomerId, jsonObject, accessToken);
                                            break;
                                        }
                                }

                            }
                        }
                    }
                    tmpEmail = data.emailaddress1.Value;
                }
              

            }
            return "Success Record: " +resultText;
        }
        public async Task<bool> UpdateTrigger(Guid CustomerId, string customer, string accessToken)
        {
            HttpRequestMessage updateRequest = new HttpRequestMessage(new HttpMethod("PATCH"), base.Resource + "api/data/v9.1/contacts(" + CustomerId + ")")
            {
                Content = new StringContent(customer, Encoding.UTF8, "application/json")
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
        public async Task<string> CreateSMSActivity(Guid CustomerId, string mobileNo, string textMessage)
        {
            string returnMsg = string.Empty;
            try
            {
                string accessToken = await this.CRMServiceConnector.GetAccessTokenCrm(this._clientFactory);


                string jsonObject = @"{'sl_mobile': '" + mobileNo + "', 'sl_message' : '" + textMessage + "','regardingobjectid_contact@odata.bind': '/contacts("+CustomerId+")' }";

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
                    throw new CrmHttpResponseException(response.Content);
            }
            catch (Exception ex)
            {
                
                returnMsg = ex.ToString();
            }
            return returnMsg;
        }
        public async Task<string> CreateEmailActivity( Guid fromUserId,Guid CustomerId, Guid TemplateId)
        {
            string returnMsg = string.Empty;
            try
            {
                string accessToken = await this.CRMServiceConnector.GetAccessTokenCrm(this._clientFactory);


                string jsonObject = @"{
                                          'TemplateId': '"+TemplateId+@"',
                                          'Regarding': {
                                            'contactid': '"+CustomerId+@"',
                                            '@odata.type': 'Microsoft.Dynamics.CRM.contact'
                                          },
                                          'Target': {
                                            'subject': 'Test',
                                            'description': 'aThis is the description Template text',
                                            'regardingobjectid_contact@odata.bind': '/contacts("+CustomerId+@")',
                                            'email_activity_parties': [
                                              {
                                                'partyid_systemuser@odata.bind': '/systemusers("+fromUserId+@")',
                                                'participationtypemask': 1
                                              },

                                              {
                                                'partyid_contact@odata.bind': '/contacts("+CustomerId+@")',
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
                    throw new CrmHttpResponseException(response.Content);
            }
            catch (Exception ex)
            {

                returnMsg = ex.ToString();
            }
            return returnMsg;
        }
        public async Task<dynamic> RetrieveMultiplePaging(string query)
        {
            string returnMsg = string.Empty;
            dynamic contact=null;
            try
            {
                string accessToken = await this.CRMServiceConnector.GetAccessTokenCrm(this._clientFactory);
                var httpClient = this._clientFactory.CreateClient("NameClientFactory");

                httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
                httpClient.DefaultRequestHeaders.Add("Prefer", "return=representation");
                httpClient.DefaultRequestHeaders.Add("Prefer", "odata.maxpagesize=2");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                HttpResponseMessage response = await httpClient.GetAsync(query);
                string responseJson = await response.Content.ReadAsStringAsync();
                contact = JsonConvert.DeserializeObject(responseJson);
            }
            catch (Exception ex)
            {
                returnMsg = ex.ToString();
            }
            return contact;
        }
        public async Task<string> LeadProcessContacts(dynamic contact,string accessToken,string resultText)
        {
            Guid TemplateId ;
            Guid fromUserId ;

            fromUserId = new Guid("40b1136e-a7ac-e911-a83f-000d3ab18b89");
            var dateNow = DateTime.Now;
            var triggerLastUpdate = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, 23, 59, 00);
            string tmpEmail = "";
          
            string returnMsg = string.Empty;
            foreach (var data in contact.value)
            {
                if (data.emailaddress1.Value != null)
                {
                    resultText = resultText + " Email Address: " + data.emailaddress1.Value;
                    if (tmpEmail == data.emailaddress1.Value)
                    {
                        continue;
                    }
                    else
                    {
                        Guid CustomerId = (Guid)data.contactid;
                        string fullname;
                        string make;
                        string model;
                        string mprice;

                        if (data.sl_valuationcreateddate.Value != null)
                        {
                            if (data.fullname != null)
                            {
                                fullname = data.fullname.Value;
                            }
                            if (data.sl_make != null)
                            {
                                make = data.sl_make.Value;
                            }
                            if (data.sl_model != null)
                            {
                                model = data.sl_model.Value;
                            }
                            if (data.sl_mprice != null)
                            {
                                mprice = data.sl_mprice.Value;
                            }
                            DateTime valuationcreateddate = data.sl_valuationcreateddate.Value;
                            int totaldays;
                            totaldays = (int)DateTime.Now.Date.Subtract(valuationcreateddate.Date).TotalDays;

                            switch (totaldays)
                            {
                                //Trigger 2
                                case 3:
                                    {
                                        TemplateId = new Guid("8CA49756-A7AF-E911-A838-000D3AB18F6D");
                                        string result2 = await CreateEmailActivity(fromUserId, CustomerId, TemplateId);
                                        if (data.telephone1 != null)
                                        {
                                            string smsMessage = "";
                                            string result1 = await CreateSMSActivity(CustomerId, data.telephone1.Value, smsMessage);
                                        }
                                        break;
                                    }
                                //Trigger 3
                                case 5:
                                    {
                                        TemplateId = new Guid("8CA49756-A7AF-E911-A838-000D3AB18F6D");
                                        //string result2 = await CreateEmailActivity(fromUserId, CustomerId, TemplateId);
                                        if (data.telephone1 != null) {
                                            string smsMessage = "";
                                         string result1 = await CreateSMSActivity(CustomerId, data.telephone1.Value, smsMessage);
                                        }
                                        break;
                                    }
                                //Trigger 4
                                case 7:
                                    {
                                        TemplateId = new Guid("8CA49756-A7AF-E911-A838-000D3AB18F6D");
                                        string result2 = await CreateEmailActivity(fromUserId, CustomerId, TemplateId);
                                        if (data.telephone1 != null)
                                        {
                                            string smsMessage = "";
                                            string result1 = await CreateSMSActivity(CustomerId, data.telephone1.Value, smsMessage);
                                        }
                                        break;
                                    }
                                //Trigger 5
                                case 17:
                                    {
                                        TemplateId = new Guid("8CA49756-A7AF-E911-A838-000D3AB18F6D");
                                        string result2 = await CreateEmailActivity(fromUserId, CustomerId, TemplateId);
                                        if (data.telephone1 != null)
                                        {
                                            string smsMessage = "";
                                            string result1 = await CreateSMSActivity(CustomerId, data.telephone1.Value, smsMessage);
                                        }
                                        break;
                                    }
                                //Trigger 6
                                case 27:
                                    {
                                        TemplateId = new Guid("8CA49756-A7AF-E911-A838-000D3AB18F6D");
                                        string result2 = await CreateEmailActivity(fromUserId, CustomerId, TemplateId);
                                        if (data.telephone1 != null)
                                        {
                                            string smsMessage = "";
                                            string result1 = await CreateSMSActivity(CustomerId, data.telephone1.Value, smsMessage);
                                        }
                                        break;
                                    }

                            }

                        }
                    }
                }
                tmpEmail = data.emailaddress1.Value;
            }
            returnMsg = resultText;
            return returnMsg;
        }
        #endregion  
    }

}