using Microsoft.VisualStudio.TestPlatform.Utilities;
using System.Text;
using System.Text.Json;

namespace FC.Codeflix.Catalog.EndToEndTests.Base
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        public ApiClient(HttpClient httpclient)
          => _httpClient = httpclient;

        public async Task<(HttpResponseMessage?, IOutput?)> Post<TOutput>(string route, object payload)
        {
            var response = await _httpClient.PostAsync(
                route,
                new StringContent(JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
                )
             );
            var outputString = await response.Content.ReadAsStringAsync();
            var output = JsonSerializer.Deserialize<TOutput>(outputString,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                }
                );
            return (response, (IOutput)output);
        }
    }
}
