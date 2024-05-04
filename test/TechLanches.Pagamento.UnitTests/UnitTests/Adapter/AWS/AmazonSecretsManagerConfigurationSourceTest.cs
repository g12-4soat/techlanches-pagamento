using Microsoft.Extensions.Configuration;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechLanches.Pagamento.Adapter.AWS;

namespace TechLanches.Pagamento.UnitTests.UnitTests.Adapter.AWS
{
    public class AmazonSecretsManagerConfigurationSourceTest
    {
        [Fact]
        public void Build_ShouldReturnConfigurationProvider_WithCorrectParameters()
        {
            // Arrange
            var region = "us-east-1";
            var secretName = "MySecret";
            var source = new AmazonSecretsManagerConfigurationSource(region, secretName);
            var builder = Substitute.For<IConfigurationBuilder>();

            // Act
            var provider = source.Build(builder);

            // Assert
            Assert.NotNull(provider);
            Assert.IsType<AmazonSecretsManagerConfigurationProvider>(provider);
        }
    }
}
