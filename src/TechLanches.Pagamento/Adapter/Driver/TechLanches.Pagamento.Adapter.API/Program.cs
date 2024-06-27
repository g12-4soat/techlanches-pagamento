using Polly;
using Polly.Extensions.Http;
using TechLanches.Pagamento.Adapter.API.Configuration;
using TechLanches.Pagamento.Adapter.AWS;
using TechLanches.Pagamento.Application.Constantes;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true, true)
    .AddEnvironmentVariables();

//AWS Secrets Manager
builder.WebHost.ConfigureAppConfiguration(((_, configurationBuilder) =>
{
    configurationBuilder.AddAmazonSecretsManager("us-east-1", "lambda-auth-credentials");
}));

builder.Services.Configure<TechLanchesCognitoSecrets>(builder.Configuration);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Colocado somente para conseguir rodar local... Recomenda��o da Microsoft � para rodar somente em produ��o
//TODO: Remover antes de finalizar a fase 
builder.Services.AddHsts(options =>
{
    options.ExcludedHosts.Clear();
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(60);
});

//Add cognito auth
builder.Services.AddAuthenticationConfig();

//Setting Swagger
builder.Services.AddSwaggerConfiguration();

//DI Abstraction
builder.Services.AddDependencyInjectionConfiguration();

//Setting mapster
builder.Services.RegisterMaps();

builder.Services.AddHealthChecks();


//Criar uma politica de retry (tente 3x, com timeout de 3 segundos)
var retryPolicy = HttpPolicyExtensions.HandleTransientHttpError()
                  .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(retryAttempt));

//Registrar httpclient

builder.Services.AddHttpClient(Constantes.API_PEDIDO, httpClient =>
{
    var url = Environment.GetEnvironmentVariable("PEDIDO_SERVICE");
    httpClient.BaseAddress = new Uri($"http://{url}:5050");
}).AddPolicyHandler(retryPolicy);

builder.Services.AddMemoryCache();

var app = builder.Build();

app.UseHsts();
app.UseRouting();
app.AddCustomMiddlewares();
app.UseAuthentication();
app.UseAuthorization();

app.UseSwaggerConfiguration();
app.AddHealthCheckEndpoint();
app.UseMapEndpointsConfiguration();
app.UseHttpsRedirection();

app.Run();