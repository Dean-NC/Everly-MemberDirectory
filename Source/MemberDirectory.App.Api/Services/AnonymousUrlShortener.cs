using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MemberDirectory.App.Api.Services
{
    /// <summary>
    /// This Url shortener uses a free remote service that currently does not require registration or an API key.
    /// </summary>
    public class AnonymousUrlShortener : Interfaces.IUrlShortener
    {
        // I found the free shortener service "sh4re". I wrote this wrapper class around it.
        const string _serviceUrl = "https://api.sh4re.be";

        private readonly HttpClient _httpClient;
        private readonly ILogger<AnonymousUrlShortener> _logger;

        public AnonymousUrlShortener(HttpClient httpClient, ILogger<AnonymousUrlShortener> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<string> Shorten(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return null;

            // The service requires you to POST a set of Form Url-encoded values, with 1 key
            // called "url" and a value of the URL you want to shorten.
            //
            // It returns a Json result with the following properties:
            //      (bool) success
            //      (string) url

            try
            {
                // Prepare the Form Url-encoded value
                using HttpContent requestContent = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
                {
                    new KeyValuePair<string, string>("url", url)
                });

                // Call the service
                using var response = await _httpClient.PostAsync(_serviceUrl, requestContent);

                if (response.IsSuccessStatusCode)
                {
                    // Read the response as string data and deserialize to a private class object
                    string resultJson = await response.Content.ReadAsStringAsync();
                    var shortenResult = JsonSerializer.Deserialize<ShortenResult>(resultJson);

                    // If we have a shortened result, return it
                    if (shortenResult?.success == true && shortenResult.url?.Length > 0)
                    {
                        return shortenResult.url;
                    }
                }
                else
                {
                    _logger.LogError("Error shortening URL: " + response.StatusCode + " " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Empty);
            }

            return null;
        }

        class ShortenResult
        {
            public bool success { get; set; }

            public string url { get; set; }
        }
    }
}
