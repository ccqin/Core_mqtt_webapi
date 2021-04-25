using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet.AspNetCore;
using Microsoft.AspNetCore;
using MQTTnet.AspNetCore.Extensions;

namespace Qins.MQTTServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        //public static IHostBuilder CreateHostBuilder(string[] args) =>
        //    Host.CreateDefaultBuilder(args)
        //        .ConfigureWebHostDefaults(webBuilder =>
        //        {
        //            webBuilder.UseStartup<Startup>().UseKestrel(o =>
        //            {
        //                o.ListenAnyIP(Config.WsPort);
        //                o.ListenAnyIP(Config.TcpPort, t => t.UseMqtt());
        //            });
        //        });
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
    }
}
