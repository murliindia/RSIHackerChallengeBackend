using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RSIHackerChallenge.Tests
{
    public class TestHttpMessageHandler : HttpMessageHandler
    {
        private readonly object _content;
        private readonly HttpStatusCode _statusCode;

        public TestHttpMessageHandler(object content, HttpStatusCode statusCode)
        {
            _content = content;
            _statusCode = statusCode;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(_statusCode);
            if (_content != null)
            {
                response.Content = new StringContent(JsonConvert.SerializeObject(_content));
            }

            return response;
        }
    }
}
