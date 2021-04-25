using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Qins.MQTTServer.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MqttController : ControllerBase
    {
        [HttpPost]
        [HttpOptions]
        public void Publish(string topic)
        {

            MemoryStream ms = new MemoryStream();
            Request.BodyReader.AsStream().CopyTo(ms);
            ms.Position = 0;
            MqttHelper.PublishAsync(topic, ms.ToArray());

        }
    }
}
