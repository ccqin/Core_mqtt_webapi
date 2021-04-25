using MQTTnet;
using MQTTnet.Server;
using System;
using System.Text;

namespace MqttBroker
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {

            var optionsBuilder = new MqttServerOptionsBuilder()
                                    .WithConnectionBacklog(100)
                                    .WithDefaultEndpointPort(1884);

            var mqttServer = new MqttFactory().CreateMqttServer();
            await mqttServer.StartAsync(optionsBuilder.Build());

            Console.WriteLine("mqtt服务器创建成功");
            mqttServer.UseClientConnectedHandler(async e =>
            {
                Console.WriteLine("客户端连接-----");
                Console.WriteLine("客户端ID:" + e.ClientId);
            });

            mqttServer.UseApplicationMessageReceivedHandler(async e =>
            {
                Console.WriteLine("服务器端接收到信息");
                Console.WriteLine("topic:" + e.ApplicationMessage.Topic);
                Console.WriteLine("payload:" + Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
            });

            mqttServer.UseClientDisconnectedHandler(async e =>
            {
                Console.WriteLine("客户端断开连接");
                Console.WriteLine("客户端信息clientid：" + e.ClientId);
            });

            Console.ReadLine();
        }
    }
}
