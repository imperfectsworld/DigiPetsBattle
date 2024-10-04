using Digi_Pets_Battle.Models;

namespace Digi_Pets_Battle.Services;

public class AccountDetailServices
{
    private readonly HttpClient _httpClient;

    public AccountDetailServices(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("http://localhost:8080/accounts");
    }

    public async Task<AccountDetails> PerformAction(string apiKey, string action)
    {
        HttpResponseMessage response = await _httpClient.PostAsync($"/by-key/{apiKey}/action?action={action}", null);

        if (response.IsSuccessStatusCode)
        {
            AccountDetails accountDetails = await response.Content.ReadFromJsonAsync<AccountDetails>();
            return accountDetails;
        }

        return null; // Handle the case where the response is not successful
    }
    public async Task<AccountDetails> GetAccountByApiKey(string apiKey)
    {
        HttpResponseMessage response = await _httpClient.GetAsync($"/by-key/{apiKey}");

        if (response.IsSuccessStatusCode)
        {
            AccountDetails accountDetails = await response.Content.ReadFromJsonAsync<AccountDetails>();
            return accountDetails;
        }

        return null;
    }
}
    
    
    