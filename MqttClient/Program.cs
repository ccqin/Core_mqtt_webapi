using MQTTnet;
using MQTTnet.Client.Options;
using System;
using System.Threading;

namespace MqttClient
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            //创建一个mqttclient
            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();

            //创建tcp options 用于连接mqtt
            var options = new MqttClientOptionsBuilder()
                //clinet的名称
                .WithClientId("client1")
                //mqtt broker的服务器地址IP，默认的是1883端口
                .WithTcpServer("127.0.0.1", 1884)
                //hivemq的登录用户名
                .WithUserProperty("admin", "hivemq")
                .WithCleanSession()
                .Build();
            await mqttClient.ConnectAsync(options, CancellationToken.None);

            Console.WriteLine("连接成功");

            while (true)
            {
                Console.WriteLine("请输入MQTT 主题topic，如果退出，请输入Q");
                string topic = Console.ReadLine();
                if (string.Equals(topic, "Q"))
                    break;
                Console.WriteLine("请输入MQTT 内容");
                string payload = Console.ReadLine();
                var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build();

                await mqttClient.PublishAsync(message, CancellationToken.None);

                Console.WriteLine("发布消息成功");
                Console.WriteLine("-----------------");
            }
        }
    }
}
