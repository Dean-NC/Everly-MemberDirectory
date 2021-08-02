using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MemberDirectory.App.Api.Services
{
    public class AnonymousUrlShortener : Interfaces.IUrlShortener
    {
        const string _serviceUrl = "https://api.sh4re.be";

        private readonly ILogger<AnonymousUrlShortener> _logger;

        public AnonymousUrlShortener(ILogger<AnonymousUrlShortener> logger)
        {
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
                using HttpContent requestContent = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
                {
                    new KeyValuePair<string, string>("url", url)
                });

                using var response = await SharedHttpClient.Client.PostAsync(_serviceUrl, requestContent);

                if (response.IsSuccessStatusCode)
                {
                    string resultJson = await response.Content.ReadAsStringAsync();
                    var shortenResult = JsonSerializer.Deserialize<ShortenResult>(resultJson);

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
