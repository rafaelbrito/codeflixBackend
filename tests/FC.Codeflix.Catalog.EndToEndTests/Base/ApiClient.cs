using FC.Codeflix.Catalog.Api.Configurations.Polices;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Json;

namespace FC.Codeflix.Catalog.EndToEndTests.Base
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _defaultSerializerOptions;

        public ApiClient(HttpClient httpclient)
        {
            _httpClient = httpclient;
            _defaultSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new JsonSkaneCasePolicy(),
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<(HttpResponseMessage?, TOutput?)> Post<TOutput>(string route, object payload)
            where TOutput : class
        {
            var response = await _httpClient.PostAsync(
                route,
                new StringContent(
                    JsonSerializer.Serialize(payload,
                    _defaultSerializerOptions
                    ),
                    Encoding.UTF8,
                    "application/json"
                )
             );
            TOutput? output = await GetOutput<TOutput>(response);
            return (response, output);
        }

        public async Task<(HttpResponseMessage?, TOutput?)> Get<TOutput>(string route, object? queryStringParametersObject = null)
            where TOutput : class
        {
            var url = PrepareGetRoute(route, queryStringParametersObject);
            var response = await _httpClient.GetAsync(url);
            TOutput? output = await GetOutput<TOutput>(response);
            return (response, output);
        }

        public async Task<(HttpResponseMessage?, TOutput?)> Delete<TOutput>(string route)
            where TOutput : class
        {
            var response = await _httpClient.DeleteAsync(route);
            TOutput? output = await GetOutput<TOutput>(response);
            return (response, output);
        }

        public async Task<(HttpResponseMessage?, TOutput?)> Put<TOutput>(string route, object payload)
           where TOutput : class
        {
            var response = await _httpClient.PutAsync(
                route,
                new StringContent(
                    JsonSerializer.Serialize(payload,
                    _defaultSerializerOptions
                    ),
                    Encoding.UTF8,
                    "application/json"
                )
             );
            TOutput? output = await GetOutput<TOutput>(response);
            return (response, output);
        }





        private async Task<TOutput> GetOutput<TOutput>(HttpResponseMessage response)
            where TOutput : class
        {
            var outputString = await response.Content.ReadAsStringAsync();
            TOutput? output = null;
            if (!string.IsNullOrWhiteSpace(outputString))
                output = JsonSerializer.Deserialize<TOutput>(outputString, 
                    _defaultSerializerOptions
                    );
            return output!;
        }

        private string PrepareGetRoute(string route, object? queryStringParametersObject)
        {
            if (queryStringParametersObject is null)
                return route;
            var parametersJson = JsonSerializer
                .Serialize(queryStringParametersObject,
                 _defaultSerializerOptions
                );
            var parametersDictionary = Newtonsoft.Json.JsonConvert
                .DeserializeObject<Dictionary<string, string>>(parametersJson);
            return QueryHelpers.AddQueryString(route, parametersDictionary!);
        }
    }
}
