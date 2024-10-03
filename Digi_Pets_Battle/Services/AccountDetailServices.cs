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

    public async Task<AccountDetails> GetAll()
    {
        AccountDetails result = await _httpClient.GetFromJsonAsync<AccountDetails>("api/Pet");
        return (result);
    }
}
    
    
    