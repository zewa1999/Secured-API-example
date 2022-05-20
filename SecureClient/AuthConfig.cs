using System.Globalization;
using Microsoft.Extensions.Configuration;

namespace SecureClient;

public class AuthConfig
{
    public string Instance { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string BaseAddress { get; set; } = string.Empty;
    public string ResourceId { get; set; } = string.Empty;

    public string Authority
    {
        get
        {
            return string.Format(CultureInfo.InvariantCulture, Instance, TenantId);
        }
    }

    public static AuthConfig ReadJsonFromFile(string path)
    {
        IConfiguration config;

        var builder = new ConfigurationBuilder();
        builder.SetBasePath(Directory.GetCurrentDirectory());
        builder.AddJsonFile(path);

        config = builder.Build();

        return config.Get<AuthConfig>();
    }
}