using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Serilog.Debugging;

namespace Serilog.Sinks.Graylog.Core.Transport.Http
{
    public class HttpTransportClient : ITransportClient<string>
    {
        private readonly string _graylogUrl;
        private readonly HttpClient _httpClient;

        public HttpTransportClient(string graylogUrl, string certificatePath)
        {
            _graylogUrl = graylogUrl;
            _httpClient = InitializeHttpClient(certificatePath);
            _httpClient.DefaultRequestHeaders.ExpectContinue = false;
            _httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
        }

        public async Task Send(string message)
        {
            var content = new StringContent(message, System.Text.Encoding.UTF8, "application/json");
            var url = new Uri(_graylogUrl);

            HttpResponseMessage result = await _httpClient.PostAsync(url, content).ConfigureAwait(false);

            if (!result.IsSuccessStatusCode)
            {
                throw new LoggingFailedException("Unable send log message to graylog via HTTP transport");
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        private HttpClient InitializeHttpClient(string certificatePath) =>
            string.IsNullOrEmpty(certificatePath) ? new HttpClient() : CreateHttpClientWithCertificate(certificatePath);

        private HttpClient CreateHttpClientWithCertificate(string certificatePath)
        {
            try
            {
                var handler = new HttpClientHandler();
                var certificate = new X509Certificate2(certificatePath);
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.ClientCertificates.Add(certificate);

                return new HttpClient(handler);
            }
            catch (Exception ex)
            {
                throw new LoggingFailedException(ex.Message);
            }
        }
    }
}