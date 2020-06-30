using System;
using System.Collections.Generic;

namespace VoxCake.Networking.Common
{
    [Serializable]
    public class ClientInfoDto
    {
        public string ip;
        public int port;
        public Dictionary<string, object> properties;
        
        public override string ToString()
        {
            return $"[ ip: {ip}, port: {port} ]";
        }
    }
}