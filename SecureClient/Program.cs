using Microsoft.Identity.Client;
using System.Net.Http.Headers;

namespace SecureClient;

public class Program
{
    private static async Task Main(string[] args)
    {
        var config = AuthConfig.ReadJsonFromFile("appsettings.json");

        string token = await GetTokenAsync(config);
        await MakeCallAsync(token, config);
    }

    private static async Task<string> GetTokenAsync(AuthConfig config)
    {
        IConfidentialClientApplication app;

        app = ConfidentialClientApplicationBuilder.Create(config.ClientId)
            .WithClientSecret(config.ClientSecret)
            .WithAuthority(new Uri(config.Authority))
            .Build();

        string[] resourceIds = new string[] { config.ResourceId };

        AuthenticationResult? result = null;

        try
        {
            result = await app.AcquireTokenForClient(resourceIds).ExecuteAsync();
            Console.WriteLine("Token acquired!");
            Console.WriteLine(result.AccessToken);
        }
        catch (MsalClientException ex)
        {
            Console.WriteLine(ex.Message);
        }

        if (result != null)
        {
            return result.AccessToken;
        }
        else
        {
            return string.Empty;
        }
    }

    private static async Task MakeCallAsync(string token, AuthConfig config)
    {
        var httpClient = new HttpClient();
        var requestHeaders = httpClient.DefaultRequestHeaders;

        if (requestHeaders.Accept == null || requestHeaders.Accept.Any(
            m => m.MediaType == "application/json"))
        {
            httpClient.DefaultRequestHeaders.Accept.Add(new
                MediaTypeWithQualityHeaderValue("application/json"));
        }

        requestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

        var responseMessage = await httpClient.GetAsync(config.BaseAddress);

        if (responseMessage.IsSuccessStatusCode)
        {
            string json = await responseMessage.Content.ReadAsStringAsync();
            Console.WriteLine("Call was made successfully!");
        }
        else
        {
            string json = await responseMessage.Content.ReadAsStringAsync();
            Console.WriteLine("Call was NOT made successfully!");
        }
    }
}