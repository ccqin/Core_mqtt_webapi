using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qins.MQTTServer
{
    public class Config
    {
        public const int TcpPort = 9089;
        public const int WsPort = 9088;
        //如果不需要密码，请将username和pwd赋值为null
        public static string UserName = "admin";
        public static string Pwd = "admin123456";
    }
}
