using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using System.Net.Http.Headers;
using System.Text;

namespace BillingSystem.Infrastructure.Integration
{
    public class RESTClient : IRESTClient
    {
        private string _suffixAdrress;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public RESTClient(ILogger logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        private HttpClient CreateHttpClient(string baseAddress, string suffixAddress, Dictionary<string, string> headers, bool requireAuth = false, string authToken = "")
        {
            _suffixAdrress = suffixAddress;
            var handler = new HttpClientHandler();
            handler.UseDefaultCredentials = true;
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    return true;
                };

            var _client = new HttpClient(handler);

            _client.BaseAddress = new Uri(baseAddress);

            if (headers == null)
                headers = new Dictionary<string, string>();

            headers.Add("Accept", "application/json");
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            foreach (var header in headers)
            {
                _client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            if (requireAuth)
            {
                //var accessToken = await HttpContext.GetTokenAsync("access_token");
                //_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var authTokenBytes = Encoding.ASCII.GetBytes(authToken);
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Convert.ToBase64String(authTokenBytes));
            }

            return _client;
        }

        public async Task<Tresponse> PostAsync<Trequest, Tresponse>(Trequest trequest, string baseAddress, string suffixAddress, Dictionary<string, string>? headers = null, bool requireAuth = false, string authToken = "") where Trequest : class, new()
        {
            try
            {
                using (var _client = CreateHttpClient(baseAddress, suffixAddress, headers))
                {
                    var json = JsonConvert.SerializeObject(trequest);
                    var request = new StringContent(json, Encoding.UTF8, "application/json");


                    var response = await _client.PostAsync(_suffixAdrress, request);
                    var result = await response.Content.ReadAsStringAsync();

                    Tresponse tresponse;

                    if (response.IsSuccessStatusCode)
                    {
                        tresponse = JsonConvert.DeserializeObject<Tresponse>(result);
                        return tresponse;
                    }
                    else
                    {
                        throw new Exception(await response.Content.ReadAsStringAsync());
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Tresponse> GetAsync<Tresponse>(Dictionary<string, string> parameters, string baseAddress, string suffixAddress, Dictionary<string, string>? headers = null, bool requireAuth = false, string authToken = "")
        {
            try
            {
                using (var _client = CreateHttpClient(baseAddress, suffixAddress, headers))
                {
                    var queryString = string.Empty;
                    HttpResponseMessage response;

                    if (parameters != null)
                    {
                        queryString = await ParamsToStringAsync(parameters);
                        response = await _client.GetAsync(_suffixAdrress + "?" + queryString);
                    }
                    else
                    {
                        response = await _client.GetAsync(_suffixAdrress);
                    }

                    var result = await response.Content.ReadAsStringAsync();

                    Tresponse tresponse;

                    if (response.IsSuccessStatusCode)
                    {
                        tresponse = JsonConvert.DeserializeObject<Tresponse>(result);
                        return tresponse;
                    }
                    else
                    {
                        throw new Exception(await response.Content.ReadAsStringAsync());
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"error in RESTClient {ex.Message} , {ex.StackTrace}");
                throw;
            }
        }

        private static async Task<string> ParamsToStringAsync(Dictionary<string, string> urlParams)
        {
            using (HttpContent content = new FormUrlEncodedContent(urlParams))
                return await content.ReadAsStringAsync();
        }

    }
}
