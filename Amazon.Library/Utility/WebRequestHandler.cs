using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Amazon.Library.Utilities
{
    public class WebRequestHandler
    {
        private string host = "localhost";
        private string port = "7244";
        private HttpClient Client { get; }

        public WebRequestHandler()
        {
            Client = new HttpClient();
        }

        public async Task<string> Get(string url)
        {
            var fullUrl = $"https://{host}:{port}{url}";
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(fullUrl).ConfigureAwait(false);
                    var content = await response.Content.ReadAsStringAsync();
                    response.EnsureSuccessStatusCode(); // This will throw if the status code is not 2xx
                    return content;
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error in GET request to {fullUrl}: {e.Message}");
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unexpected error in GET request to {fullUrl}: {e.Message}");
                throw;
            }
        }

        public async Task<string> Delete(string url)
        {
            var fullUrl = $"https://{host}:{port}{url}";
            try
            {
                using (var client = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Delete, fullUrl))
                    {
                        var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                        var content = await response.Content.ReadAsStringAsync();
                        response.EnsureSuccessStatusCode(); // This will throw if the status code is not 2xx
                        return content;
                    }
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error in DELETE request to {fullUrl}: {e.Message}");
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unexpected error in DELETE request to {fullUrl}: {e.Message}");
                throw;
            }
        }

        public async Task<string> Post(string url, object obj)
        {
            var fullUrl = $"https://{host}:{port}{url}";
            try
            {
                using (var client = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Post, fullUrl))
                    {
                        var json = JsonConvert.SerializeObject(obj);
                        using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
                        {
                            request.Content = stringContent;

                            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                            var content = await response.Content.ReadAsStringAsync();
                            response.EnsureSuccessStatusCode(); // This will throw if the status code is not 2xx
                            return content;
                        }
                    }
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error in POST request to {fullUrl}: {e.Message}");
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unexpected error in POST request to {fullUrl}: {e.Message}");
                throw;
            }
        }
    }
}
