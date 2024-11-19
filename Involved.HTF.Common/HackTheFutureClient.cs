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

    // Modified GetData method with fieldname parameter
    public async Task<string> GetData(string route, string fieldname)
    {
        var response = await GetAsync(route);
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Error Content: " + errorContent); // Log the full response content
            throw new Exception($"Failed to get data from {route}. Status code: {response.StatusCode}. Response: {errorContent}");
        }

        // Log the raw response to inspect it
        var content = await response.Content.ReadAsStringAsync();

        try
        {
            var data = JsonSerializer.Deserialize<JsonElement>(content);

            // Check if the field exists and is not null
            if (data.TryGetProperty(fieldname, out var fieldValue))
            {
                // Check if the field is a string
                if (fieldValue.ValueKind == JsonValueKind.String)
                {
                    return fieldValue.GetString();
                }
                // Check if the field is a number (int, float, etc.)
                else if (fieldValue.ValueKind == JsonValueKind.Number)
                {
                    // Return the number as a string, or parse it to a specific type
                    return fieldValue.ToString();
                }
                // Check if the field is an array
                else if (fieldValue.ValueKind == JsonValueKind.Array)
                {
                    // If it's an array, you might want to return it as a string (for example, JSON array)
                    return fieldValue.ToString();  // Return the array as a string
                }
                else
                {
                    // Handle other types if needed
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
