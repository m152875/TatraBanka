using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Web;
using System.Web.Http;

namespace Cora.CommIss.Iss.TatraBanka
{
    public class HtmlActionResult : IHttpActionResult
    {
        private readonly string _html;

        public HtmlActionResult(string html)
        {
            _html = html;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(_html, Encoding.UTF8, "text/html")
            };
            return Task.FromResult(response);
        }
    }
}