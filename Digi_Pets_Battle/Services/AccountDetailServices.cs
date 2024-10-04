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

    public async Task<AccountDetails> GetAccountDetailsByApiKey(string apiKey)
    {
        AccountDetails result = await _httpClient.GetFromJsonAsync<AccountDetails>($"/by-key/{apiKey}");
        return (result);
    }

    public async Task<AccountDetails> GetAccountDetails(long id, string apiKey)
    {
        AccountDetails result = await _httpClient.GetFromJsonAsync<AccountDetails>($"?username={id}n&apiKey={apiKey}");
        return result;
    }

    public async Task<AccountDetails> GettingAction(string action, string apiKey)
    {
        AccountDetails result = await _httpClient.GetFromJsonAsync<AccountDetails>($"/by-key/{apiKey}/action?action={action}");
        return result;
    }
}
    
    
    