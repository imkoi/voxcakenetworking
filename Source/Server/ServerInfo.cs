using System.Xml.Schema;
using VoxCake.Networking.Common;

namespace VoxCake.Networking
{
    public class ServerInfo
    {
        public string Ip { get; internal set; }
        public int Port { get; internal set; }
        public int TickRate { get; internal set; }
        public int MaxTcpPacketSize { get; internal set; }
        public int MaxUdpPacketSize { get; internal set; }

        internal ServerInfo(ServerInfoDto dto)
        {
            Ip = dto.ip;
            Port = dto.port;
            TickRate = dto.tickRate;
            MaxTcpPacketSize = dto.maxTcpPacketSize;
            MaxUdpPacketSize = dto.maxUdpPacketSize;
        }
        
        public ServerInfo(string ip, int port)
        {
            Ip = ip;
            Port = port;
            TickRate = 10;
            MaxTcpPacketSize = 255;
            MaxUdpPacketSize = 255;
        }
        
        public ServerInfo(string ip, int port, int tickRate)
        {
            Ip = ip;
            Port = port;
            TickRate = tickRate;
            MaxTcpPacketSize = 255;
            MaxUdpPacketSize = 255;
        }

        public ServerInfo(string ip, int port, int tickRate, int maxPacketSize)
        {
            Ip = ip;
            Port = port;
            TickRate = tickRate;
            MaxTcpPacketSize = maxPacketSize;
            MaxUdpPacketSize = maxPacketSize;
        }

        public ServerInfo(string ip, int port, int tickRate, int maxTcpPacketSize, int maxUdpPacketSize)
        {
            Ip = ip;
            Port = port;
            TickRate = tickRate;
            MaxTcpPacketSize = maxTcpPacketSize;
            MaxUdpPacketSize = maxUdpPacketSize;
        }

        public override string ToString()
        {
            return $"[ ip: {Ip}, port: {Port} ]";
        }
    }
}