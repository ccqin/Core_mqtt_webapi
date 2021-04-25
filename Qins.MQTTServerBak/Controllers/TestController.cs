using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qins.MQTTServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TestController : RootController
    {
        [HttpGet]
        public string Test()
        {
            string result = "";
            //从服务集合中获取MQTT服务
            var service = ServiceLocator.Instance.GetService(typeof(MQTTnet.Server.MqttServer));
            var messager = (MQTTnet.Server.MqttServer)service;

            //这里你可以构建消息并发布

            return result;
        }

        //[HttpGet]
        //public string Get()
        //{
        //    string result = "";
        //    //从服务集合中获取MQTT服务
        //    var service = ServiceLocator.Instance.GetService(typeof(MQTTnet.Server.MqttServer));
        //    var messager = (MQTTnet.Server.MqttServer)service;

        //    //这里你可以构建消息并发布

        //    return result;
        //}
    }
}
