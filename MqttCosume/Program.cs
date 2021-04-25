using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System;
using System.Text;
using System.Threading;

namespace MqttCosume
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
                .WithClientId("client2")
                //mqtt broker的服务器地址IP，默认的是1883端口
                .WithTcpServer("127.0.0.1", 1884)
                //hivemq的登录用户名
                .WithUserProperty("admin", "hivemq")
                .WithCleanSession()
                .Build();
            await mqttClient.ConnectAsync(options, CancellationToken.None);

            Console.WriteLine("连接成功");


            await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("mytopic").Build());
            await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("hello").Build());

            Console.WriteLine("订阅消息成功");


            mqttClient.UseApplicationMessageReceivedHandler(async e =>
            {
                Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
                Console.WriteLine();

            });

            Console.ReadLine();
        }
    }
}
