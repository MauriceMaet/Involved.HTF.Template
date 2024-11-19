using System;
using System.Linq;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Involved.HTF.Common;

public class HackTheFutureClient : HttpClient
{
    public HackTheFutureClient()
    {
        BaseAddress = new Uri("https://app-htf-2024.azurewebsites.net/");
    }

    public async Task Login()
    {
        var response = await GetAsync($"/api/team/token?teamname=Nebula-Navigators&password=674390d9-db9a-4b09-be6b-b128afc20f41");
        if (!response.IsSuccessStatusCode)
            throw new Exception("You weren't able to log in, did you provide the correct credentials?");
        var token = await response.Content.ReadFromJsonAsync<AuthResponse>();
        DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Token);
    }

    public async Task<string> GetData(string route, string fieldname)
    {
        var response = await GetAsync(route);
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Error Content: " + errorContent);
            throw new Exception($"Failed to get data from {route}. Status code: {response.StatusCode}. Response: {errorContent}");
        }

        var content = await response.Content.ReadAsStringAsync();

        try
        {
            var data = JsonSerializer.Deserialize<JsonElement>(content);

            if (data.TryGetProperty(fieldname, out var fieldValue))
            {
                if (fieldValue.ValueKind == JsonValueKind.String)
                {
                    return fieldValue.GetString();
                }
                else if (fieldValue.ValueKind == JsonValueKind.Number)
                {
                    return fieldValue.ToString();
                }
                else if (fieldValue.ValueKind == JsonValueKind.Array)
                {
                    return fieldValue.ToString();
                }
                else
                {
                    throw new Exception($"Expected a string, number, or array for field '{fieldname}', but got {fieldValue.ValueKind}.");
                }
            }
            else
            {
                throw new Exception($"Field '{fieldname}' not found in the API response.");
            }
        }
        catch (JsonException ex)
        {
            throw new Exception($"Error parsing JSON response: {ex.Message}", ex);
        }
    }





}

public class AuthResponse
{
    public string Token { get; set; }
}

public class ApiResponse
{
    public string Commands { get; set; }
}
