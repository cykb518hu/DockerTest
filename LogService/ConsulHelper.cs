using Consul;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogService
{
    public static class ConsulHelper
    {
        public static void ConsulRegist(this IConfiguration configuration)
        {
            ConsulClient client = new ConsulClient(c =>
            {
                c.Address = new Uri("http://localhost:8500/");
                c.Datacenter = "dcl";
            });

            string ip = configuration["ip"];
            int port = int.Parse(configuration["port"]);
            var http = $"http://{ip}:{port}/WeatherForecast";
            Console.WriteLine(http);




            client.Agent.ServiceRegister(new AgentServiceRegistration()
            {
                ID = "LogService" + Guid.NewGuid(),
                Name = "LogService",
                Address = ip,
                Port = port,
                Check=new AgentServiceCheck()
                {
                    Interval=TimeSpan.FromSeconds(12),
                    HTTP= http,
                    Timeout=TimeSpan.FromSeconds(5),
                    DeregisterCriticalServiceAfter=TimeSpan.FromSeconds(60)
                }
            });
        }
    }
}
