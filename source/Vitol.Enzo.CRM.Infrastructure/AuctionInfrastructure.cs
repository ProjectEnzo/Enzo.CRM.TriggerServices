using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Vitol.Enzo.Core.Extensions;
using Vitol.Enzo.CRM.Domain;
using Vitol.Enzo.CRM.InfrastructureInterface;
using Vitol.Enzo.CRM.ServiceConnectorInterface;
using Vitol.Enzo.Infrastructure.Extensions;

namespace Vitol.Enzo.CRM.Infrastructure
{
    public class AuctionInfrastructure : BaseInfrastructure, IAuctionInfrastructure
    {
        public AuctionInfrastructure(ICRMServiceConnector crmServiceConnector, IConfiguration configuration, ILogger<LeadInfrastructure> logger, IHttpClientFactory clientFactory) : base(configuration, logger, clientFactory)
        {
            this.CRMServiceConnector = crmServiceConnector;
            exceptionModel.ComponentName = Enum.GetName(typeof(ComponentType), ComponentType.Auction);
        }

        private const string GetStoredProcedureName = "CustomerTest";
        private const string CustomerIdColumnName = "CustomerId";
        private const string CustomerNameColumnName = "CustomerName";
        private const string EmailColumnName = "Email";
        private const string PhoneNumberColumnName = "PhoneNumber";
        private const string LocationColumnName = "Location";
        private const string LongitudeColumnName = "Longitude";
        private const string LatitudeColumnName = "Latitude";
        private const string RegistrationNumberColumnName = "RegistrationNumber";
        public async Task<string> AuctionUtilityService()
        {
        var result= await GetAllCustomers();
            if (result == true)
                return "true";
            else 
                return "false";
          
        }

        public async Task <bool>GetAllCustomers()
        {
            CRMCustomer CustomerItem = null;
            List<CRMCustomer> Customerlist = new List<CRMCustomer>();
            using (var dataReader = await base.ExecuteReader(null, AuctionInfrastructure.GetStoredProcedureName, CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {

                    while (dataReader.Read())
                    {

                        CustomerItem = new CRMCustomer
                        {
                            sl_customeridexternal = dataReader.GetUnsignedIntegerValue(AuctionInfrastructure.CustomerIdColumnName),
                            lastname = dataReader.GetStringValue(AuctionInfrastructure.CustomerNameColumnName),
                            emailaddress1 = dataReader.GetStringValue(AuctionInfrastructure.EmailColumnName),
                            telephone1 = dataReader.GetStringValue(AuctionInfrastructure.PhoneNumberColumnName),
                            sl_registrationnumber = dataReader.GetStringValue(AuctionInfrastructure.RegistrationNumberColumnName),
                            sl_latitude = dataReader.GetDecimalValue(AuctionInfrastructure.LatitudeColumnName),
                            sl_longitude = dataReader.GetDecimalValue(AuctionInfrastructure.LongitudeColumnName),
                            address1_line1 = dataReader.GetStringValue(AuctionInfrastructure.LocationColumnName)

                        };
                        Customerlist.Add(CustomerItem);
                    }

                     if (!dataReader.IsClosed)
                    {
                        dataReader.Close();
                    }
                    foreach(var Customer in  Customerlist)
                    {                        
                        await AddToCRM(Customer);
                    }
                        return true;
                }
                else
                    return false;
            }
        }


        public async Task<string> AddToCRM(CRMCustomer customer)
        {
            exceptionModel.ActionName = Enum.GetName(typeof(ActionType), ActionType.Upsert);
            exceptionModel.Id = (int)customer.sl_customeridexternal;
            var json = JsonConvert.SerializeObject(customer);
            string CRMCustomerId="", returnMsg="";
            bool resultflag = false;
            HttpResponseMessage response;
            exceptionModel.ActionName = Enum.GetName(typeof(ActionType), ActionType.add);
            exceptionModel.Id = (int)customer.sl_customeridexternal;
            try
            {
                string accessToken = await this.CRMServiceConnector.GetAccessTokenCrm(this._clientFactory);
                Guid CustomerId = await IsCustomerAlreadyCreated(customer.sl_customeridexternal, accessToken);
                if (CustomerId != null && CustomerId != Guid.Empty)
                    resultflag = true;

                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                var httpClient = this._clientFactory.CreateClient("NameClientFactory");

                httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
                httpClient.DefaultRequestHeaders.Add("Prefer", "return=representation");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                if (resultflag)
                {
                    HttpRequestMessage updateRequest = new HttpRequestMessage(new HttpMethod("PATCH"), base.Resource + "api/data/v9.1/contacts(" + CustomerId + ")")
                    {
                        Content = new StringContent(json, Encoding.UTF8, "application/json")
                    };
                    response = await httpClient.SendAsync(updateRequest);
                }
                else
                {

                    response = await httpClient.PostAsync(base.Resource + "api/data/v9.1/contacts?$select=sl_customeridexternal", content);
                }

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
                this.Logger.LogError(exceptionModel.getExceptionFormat(ex));
                returnMsg = ex.ToString();
            }
            return CRMCustomerId;

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
        public ICRMServiceConnector CRMServiceConnector { get; }
        public ExceptionModel exceptionModel = new ExceptionModel();
    }
}
