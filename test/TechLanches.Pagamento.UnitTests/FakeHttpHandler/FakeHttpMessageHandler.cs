using System.Net;
using System.Text;

namespace TechLanches.Pagamento.UnitTests.FakeHttpHandler
{
    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly string _mockResponse;
        private readonly HttpStatusCode _httpStatusCode;
        public FakeHttpMessageHandler(string mockJsonResponse, HttpStatusCode httpStatusCode)
        {
            _mockResponse = mockJsonResponse;
            _httpStatusCode = httpStatusCode;
        }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(_httpStatusCode);
            response.Content = new StringContent(_mockResponse, Encoding.UTF8, "application/json"); ;
            return Task.FromResult(response);
        }
    }
}
