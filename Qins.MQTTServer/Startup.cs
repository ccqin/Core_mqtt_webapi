using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MQTTnet.AspNetCore.Extensions;
using MQTTnet.Protocol;
using MQTTnet.Server;
using System;

namespace Qins.MQTTServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            Config.UserName = Configuration["MqttOption:UserName"];
            Config.Pwd = Configuration["MqttOption:Password"];

            services.AddControllers();

            var optionBuilder = new MqttServerOptionsBuilder()
                .WithDefaultEndpointBoundIPAddress(System.Net.IPAddress.Parse("127.0.0.1"))
                .WithDefaultEndpointPort(Config.TcpPort)
                .WithDefaultCommunicationTimeout(TimeSpan.FromMilliseconds(5000))
                .WithConnectionValidator(t =>
                {
                    if (t.Username != Config.UserName || t.Password != Config.Pwd)
                    {
                        t.ReasonCode = MqttConnectReasonCode.NotAuthorized;
                    }
                    t.ReasonCode = MqttConnectReasonCode.Success;
                });
            var option = optionBuilder.Build();

            //·þÎñ×¢Èë
            services.AddHostedMqttServer(option);

            services.AddMqttConnectionHandler();

            services.AddMqttWebSocketServerAdapter();

            services.AddMqttTcpServerAdapter();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseMqttEndpoint();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseMqttServer(S =>
            {
                MqttHelper.Server = S;
            });
        }
    }
}
