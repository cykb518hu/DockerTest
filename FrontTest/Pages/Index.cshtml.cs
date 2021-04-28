using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace FrontTest.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            var url = "http://localhost:5123/weatherforecast";

            var uri = new Uri(url);

            ConsulClient client = new ConsulClient(c =>
            {
                c.Address = new Uri("http://localhost:8500/");
                c.Datacenter = "dcl";
            });

            var response = client.Agent.Services().Result.Response;
            var serviceList = response.Where(x => x.Value.Service.ToLower() == "logservice").ToArray();
            var item = serviceList[new Random().Next(0, serviceList.Length)];
            url = $"http://{item.Value.Address}:{item.Value.Port}{uri.PathAndQuery}";

            var str = InvokeApi(url);

            ViewData["data"] = str;


        }

        public string InvokeApi(string url)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpRequestMessage message = new HttpRequestMessage();
                message.Method = HttpMethod.Get;
                message.RequestUri = new Uri(url);
                var result = httpClient.SendAsync(message).Result;
                string content = result.Content.ReadAsStringAsync().Result;
                return content;


            }
        }
    }
}
