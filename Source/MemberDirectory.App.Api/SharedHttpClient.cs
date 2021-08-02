using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MemberDirectory.App.Api
{
    /// <summary>
    /// The .Net HttpClient was designed to be shared where possible in the application
    /// as a single instance, mainly for performance reasons. This class manages that shared client.
    /// </summary>
    public class SharedHttpClient : IDisposable
    {
        // Lazy initializes the object when it is first accessed.
        private static readonly Lazy<HttpClient> _client = new(() => new HttpClient());

        public static HttpClient Client => _client.Value;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _client.Value?.Dispose();
            }
        }
    }
}
