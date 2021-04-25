using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qins.MQTTServer
{
    public class MqttHelper
    {
        public static IMqttServer Server { get; set; }

        public static void PublishAsync(string topic, byte[] payload)
        {
            if (Server != null)
            {
                Server.PublishAsync(new MQTTnet.MqttApplicationMessage()
                {
                    Topic = topic,
                    Payload = payload
                }, new System.Threading.CancellationToken(false));
            }
        }
        public static void PublishAsync(string topic, string payload)
        {
            if (Server != null)
            {
                Server.PublishAsync(new MQTTnet.MqttApplicationMessage()
                {
                    Topic = topic,
                    Payload = Encoding.UTF8.GetBytes(payload)
                }, new System.Threading.CancellationToken(false)); ;
            }
        }
    }
}
