using System;
using System.Collections.Generic;

namespace VoxCake.Networking.Common
{
    [Serializable]
    internal class ServerInfoDto
    {
        public string ip;
        public int port;
        public int tickRate;
        public int maxTcpPacketSize;
        public int maxUdpPacketSize;
        public Dictionary<string, object> properties;
        
        public override string ToString()
        {
            return $"[ ip: {ip}, port: {port} ]";
        }
    }
}