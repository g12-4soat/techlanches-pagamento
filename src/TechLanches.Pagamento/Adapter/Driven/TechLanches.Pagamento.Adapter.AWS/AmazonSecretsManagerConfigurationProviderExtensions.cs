using Microsoft.Extensions.Configuration;

namespace TechLanches.Pagamento.Adapter.AWS
{
    public static class AmazonSecretsManagerConfigurationProviderExtensions
    {
        public static IConfigurationBuilder AddAmazonSecretsManager(this IConfigurationBuilder configurationBuilder,
                        string region,
                        string secretName)
        {
            var configurationSource =
                    new AmazonSecretsManagerConfigurationSource(region, secretName);

            return configurationBuilder.Add(configurationSource);
        }
    }
}
