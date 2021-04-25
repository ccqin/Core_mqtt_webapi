using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qins.MQTTServer.Controllers
{
    public class RootController : ControllerBase
    {
        protected MqttServer Messager { get; private set; }

        public RootController()
        {
            var service = ServiceLocator.Instance.GetService(typeof(MqttServer));
            Messager = (MqttServer)service;
            //typeof(MqttServer).IsInstanceOfType(server);
        }
    }

    public static class ServiceLocator
    {
        public static IServiceProvider Instance { get; set; }
    }
}
