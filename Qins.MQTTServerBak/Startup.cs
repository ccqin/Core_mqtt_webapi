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

            #region MQTT����
            string hostIp = Configuration["MqttOption:HostIp"];//IP��ַ
            int hostPort = int.Parse(Configuration["MqttOption:HostPort"]);//�˿ں�
            int timeout = int.Parse(Configuration["MqttOption:Timeout"]);//��ʱʱ��
            string username = Configuration["MqttOption:UserName"];//�û���
            string password = Configuration["MqttOption:Password"];//����

            //����������
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

            //����ע��
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
            //MQTT���������¼�
            app.UseMqttServer(server =>
            {
                //���������¼�
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
                //����ֹͣ�¼�
                server.StoppedHandler = new MqttServerStoppedHandlerDelegate(args =>
                {

                });

                //�ͻ��������¼�
                server.ClientConnectedHandler = new MqttServerClientConnectedHandlerDelegate (args =>
                {
                    var clientId = args.ClientId;
                    logger.LogInformation($"'{args.ClientId}' Connected");
                });
                //�ͻ��˶Ͽ��¼�
                server.ClientDisconnectedHandler = new MqttServerClientDisconnectedHandlerDelegate (args =>
                {
                    var clientId = args.ClientId;
                    logger.LogInformation($"'{args.ClientId}' Disconnect");
                });
                //�յ���Ϣ�¼�
                server.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(e =>
                {
                    logger.LogInformation($"'{e.ClientId}' reported '{e.ApplicationMessage.Topic}' > '{Encoding.UTF8.GetString(e.ApplicationMessage.Payload ?? new byte[0])}'");
                });
                //���������¼�
                server.ClientSubscribedTopicHandler = new MqttServerClientSubscribedHandlerDelegate(e =>
                {
                    var ClientId = e.ClientId;
                    var Topic = e.TopicFilter.Topic;
                    logger.LogInformation($"�ͻ���[{ClientId}]�Ѷ������⣺{Topic}");
                });
            });

            Controllers.ServiceLocator.Instance = app.ApplicationServices;
        }

    }
}

