using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet.AspNetCore.Extensions;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Logging;

namespace Qins.MQTTServer
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        private static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel(o =>
                {
                    o.ListenAnyIP(9089, l => l.UseMqtt());
                    o.ListenAnyIP(5000); // default http pipeline
                })
                .UseStartup<Startup>()
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConsole(); // 加上这个
                })
                .Build();

        //public static IHostBuilder CreateHostBuilder(string[] args) =>
        //    Host.CreateDefaultBuilder(args)
        //        .ConfigureWebHostDefaults(webBuilder =>
        //        {
        //            webBuilder.UseStartup<Startup>()
        //            .UseKestrel(o =>
        //            {
        //                o.ListenAnyIP(61613, l => l.UseMqtt());
        //                o.ListenAnyIP(5000);
        //            });
        //        });

        //.ConfigureWebHostDefaults(webBuilder =>
        //{
        //    webBuilder.UseStartup<Startup>()
        //    .UseKestrel(o =>
        //    {
        //        o.ListenAnyIP(61613, m => m.UseMqtt());//绑定MQTT服务端口
        //                                               //o.ListenAnyIP(5000);
        //    });
        //});
    }
}
