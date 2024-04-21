using Polly;
using Polly.Extensions.Http;
using System.Net.Http.Headers;
using TechLanches.Pagamento.Adapter.API.Configuration;
using TechLanches.Pagamento.Adapter.AWS;
using TechLanches.Pagamento.Application.Options;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true, true)
    .AddEnvironmentVariables();

//AWS Secrets Manager
//builder.WebHost.ConfigureAppConfiguration(((_, configurationBuilder) =>
//{
//    configurationBuilder.AddAmazonSecretsManager("us-east-1", "lambda-auth-credentials");
//}));

builder.Services.Configure<TechLanchesCognitoSecrets>(builder.Configuration);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Add cognito auth
builder.Services.AddAuthenticationConfig();

//Setting Swagger
builder.Services.AddSwaggerConfiguration();

//DI Abstraction
builder.Services.AddDependencyInjectionConfiguration();

//Setting mapster
builder.Services.RegisterMaps();

builder.Services.Configure<ApplicationOptions>(builder.Configuration.GetSection("ApiMercadoPago"));

//Criar uma politica de retry (tente 3x, com timeout de 3 segundos)
var retryPolicy = HttpPolicyExtensions.HandleTransientHttpError()
                  .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(retryAttempt));

//Registrar httpclient
builder.Services.AddHttpClient("MercadoPago", httpClient =>
{
    httpClient.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", builder.Configuration.GetSection($"ApiMercadoPago:AccessToken").Value);
    httpClient.BaseAddress = new Uri(builder.Configuration.GetSection($"ApiMercadoPago:BaseUrl").Value);
}).AddPolicyHandler(retryPolicy);



var app = builder.Build();

app.AddCustomMiddlewares();
app.UseAuthentication();
app.UseAuthorization();

app.UseSwaggerConfiguration();

app.UseMapEndpointsConfiguration();

app.UseHttpsRedirection();

app.Run();