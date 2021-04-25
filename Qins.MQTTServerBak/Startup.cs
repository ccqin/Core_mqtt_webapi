using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.AspNetCore;
using MQTTnet.AspNetCore.Extensions;
using MQTTnet.Client.Receiving;
using MQTTnet.Protocol;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace Qins.MQTTServer
{
    public class Startup
    {
        ILogger<Startup> logger;
        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            this.logger = logger;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            #region MQTT配置
            string hostIp = Configuration["MqttOption:HostIp"];//IP地址
            int hostPort = int.Parse(Configuration["MqttOption:HostPort"]);//端口号
            int timeout = int.Parse(Configuration["MqttOption:Timeout"]);//超时时间
            string username = Configuration["MqttOption:UserName"];//用户名
            string password = Configuration["MqttOption:Password"];//密码

            //构建配置项
            var optionBuilder = new MqttServerOptionsBuilder()
                .WithDefaultEndpointBoundIPAddress(System.Net.IPAddress.Parse(hostIp))
                .WithDefaultEndpointPort(hostPort)
                .WithDefaultCommunicationTimeout(TimeSpan.FromMilliseconds(timeout))
                .WithConnectionValidator(t =>
                {
                    if (t.Username != username || t.Password != password)
                    {
                        t.ReasonCode = MqttConnectReasonCode.NotAuthorized;
                    }
                    t.ReasonCode = MqttConnectReasonCode.Success;
                });
            var option = optionBuilder.Build();

            //服务注入
            services
                .AddHostedMqttServer(option)
                .AddMqttConnectionHandler()
                .AddConnections();
            #endregion

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseMvc();
            app.UseRouting();

            //app.UseConnections(c => c.MapConnectionHandler<MqttConnectionHandler>("/data", options =>
            //{
            //    options.WebSockets.SubProtocolSelector = MQTTnet.AspNetCore.ApplicationBuilderExtensions.SelectSubProtocol;
            //}));
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //});
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapMqtt("/test");
            });
            //MQTT声明周期事件
            app.UseMqttServer(server =>
            {
                //服务启动事件
                server.StartedHandler = new MqttServerStartedHandlerDelegate(async args =>
                {
                    var frameworkName = GetType().Assembly.GetCustomAttribute<TargetFrameworkAttribute>()?
                        .FrameworkName;


                    logger.LogInformation($"Mqtt hosted on {frameworkName} is awesome");
                    //var msg = new MqttApplicationMessageBuilder()
                    //    .WithPayload($"Mqtt hosted on {frameworkName} is awesome")
                    //    .WithTopic("message");

                    //while (true)
                    //{
                    //    try
                    //    {
                    //        await server.PublishAsync(msg.Build());
                    //        msg.WithPayload($"Mqtt hosted on {frameworkName} is still awesome at {DateTime.Now}");
                    //    }
                    //    catch (Exception e)
                    //    {
                    //        Console.WriteLine(e);
                    //    }
                    //    finally
                    //    {
                    //        await Task.Delay(TimeSpan.FromSeconds(2));
                    //    }
                    //}
                });
                //服务停止事件
                server.StoppedHandler = new MqttServerStoppedHandlerDelegate(args =>
                {

                });

                //客户端连接事件
                server.ClientConnectedHandler = new MqttServerClientConnectedHandlerDelegate (args =>
                {
                    var clientId = args.ClientId;
                    logger.LogInformation($"'{args.ClientId}' Connected");
                });
                //客户端断开事件
                server.ClientDisconnectedHandler = new MqttServerClientDisconnectedHandlerDelegate (args =>
                {
                    var clientId = args.ClientId;
                    logger.LogInformation($"'{args.ClientId}' Disconnect");
                });
                //收到消息事件
                server.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(e =>
                {
                    logger.LogInformation($"'{e.ClientId}' reported '{e.ApplicationMessage.Topic}' > '{Encoding.UTF8.GetString(e.ApplicationMessage.Payload ?? new byte[0])}'");
                });
                //订阅主题事件
                server.ClientSubscribedTopicHandler = new MqttServerClientSubscribedHandlerDelegate(e =>
                {
                    var ClientId = e.ClientId;
                    var Topic = e.TopicFilter.Topic;
                    logger.LogInformation($"客户端[{ClientId}]已订阅主题：{Topic}");
                });
            });

            Controllers.ServiceLocator.Instance = app.ApplicationServices;
        }

    }
}

