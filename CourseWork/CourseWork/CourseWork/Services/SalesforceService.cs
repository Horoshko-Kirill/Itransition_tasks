using System.Net.Http.Headers;
using System.Text.Json;
using CourseWork.Models.ViewModels;

namespace CourseWork.Services
{
    public class SalesforceService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;

        private string? _accessToken;
        private string? _instanceUrl;

        public SalesforceService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }

       
        private async Task EnsureAccessTokenAsync()
        {
            if (!string.IsNullOrEmpty(_accessToken)) return;

            var request = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "password",
                ["client_id"] = _config["Salesforce:ConsumerKey"],
                ["client_secret"] = _config["Salesforce:ConsumerSecret"],
                ["username"] = _config["Salesforce:Username"],
                ["password"] = _config["Salesforce:Password"] + _config["Salesforce:SecurityToken"]
            });

            var response = await _http.PostAsync("https://login.salesforce.com/services/oauth2/token", request);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Error Salesforce authentication: {body}");

            var json = JsonDocument.Parse(body);
            _accessToken = json.RootElement.GetProperty("access_token").GetString();
            _instanceUrl = json.RootElement.GetProperty("instance_url").GetString();
        }

      
        public async Task<SalesforceResult> CreateAccountAndContactAsync(string accountName, string firstName, string lastName, string email)
        {
            await EnsureAccessTokenAsync();

            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

         
            var query = $"SELECT Id, AccountId FROM Contact WHERE Email = '{email}' LIMIT 1";
            var queryResponse = await _http.GetAsync($"{_instanceUrl}/services/data/v57.0/query/?q={Uri.EscapeDataString(query)}");
            var queryRaw = await queryResponse.Content.ReadAsStringAsync();

            if (!queryResponse.IsSuccessStatusCode)
            {
                return new SalesforceResult
                {
                    Success = false,
                    ErrorMessage = $"Error checking existing contact: {queryRaw}"
                };
            }

            var queryJson = JsonDocument.Parse(queryRaw);
            int totalSize = queryJson.RootElement.GetProperty("totalSize").GetInt32();

            if (totalSize > 0)
            {
                var existingContact = queryJson.RootElement.GetProperty("records")[0];
                return new SalesforceResult
                {
                    Success = false,
                    ContactId = existingContact.GetProperty("Id").GetString(),
                    AccountId = existingContact.GetProperty("AccountId").GetString(),
                    ErrorMessage = "User already exists in CRM"
                };
            }

          
            var accountBody = JsonSerializer.Serialize(new { Name = accountName });
            var accountResponse = await _http.PostAsync(
                $"{_instanceUrl}/services/data/v57.0/sobjects/Account",
                new StringContent(accountBody, System.Text.Encoding.UTF8, "application/json")
            );

            if (!accountResponse.IsSuccessStatusCode)
            {
                var error = await accountResponse.Content.ReadAsStringAsync();
                return new SalesforceResult
                {
                    Success = false,
                    ErrorMessage = $"Error creating account: {error}"
                };
            }

            var accountJson = JsonDocument.Parse(await accountResponse.Content.ReadAsStringAsync());
            string accountId = accountJson.RootElement.GetProperty("id").GetString()!;

           
            var contactBody = JsonSerializer.Serialize(new
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                AccountId = accountId
            });

            var contactResponse = await _http.PostAsync(
                $"{_instanceUrl}/services/data/v57.0/sobjects/Contact",
                new StringContent(contactBody, System.Text.Encoding.UTF8, "application/json")
            );

            if (!contactResponse.IsSuccessStatusCode)
            {
                var error = await contactResponse.Content.ReadAsStringAsync();
                return new SalesforceResult
                {
                    Success = false,
                    ErrorMessage = $"Error creating contact: {error}"
                };
            }

            var contactJson = JsonDocument.Parse(await contactResponse.Content.ReadAsStringAsync());
            string contactId = contactJson.RootElement.GetProperty("id").GetString()!;


            return new SalesforceResult
            {
                Success = true,
                AccountId = accountId,
                ContactId = contactId
            };
        }
    }
}
