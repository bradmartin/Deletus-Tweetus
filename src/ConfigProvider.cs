using System.IO;
using Microsoft.Extensions.Configuration;

public static class ConfigValueProvider
{
    private static readonly IConfigurationRoot Configuration;

    static ConfigValueProvider()
    {
        IConfigurationBuilder builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        Configuration = builder.Build();
    }

    public static string Get(string name)
    {
        return Configuration[name];
    }
}