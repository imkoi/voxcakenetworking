using VoxCake.Networking.Common;

namespace VoxCake.Networking
{
    public class ServerInfo
    {
        public string Ip { get; internal set; }
        public int Port { get; internal set; }
        public int TickRate { get; internal set; }

        internal ServerInfo(ServerInfoDto dto)
        {
            Ip = dto.ip;
            Port = dto.port;
            TickRate = dto.tickRate;
        }
        
        public ServerInfo(string ip, int port)
        {
            Ip = ip;
            Port = port;
            TickRate = 10;
        }
        
        public ServerInfo(string ip, int port, int tickRate)
        {
            Ip = ip;
            Port = port;
            TickRate = tickRate;
        }

        public override string ToString()
        {
            return $"[ ip: {Ip}, port: {Port} ]";
        }
    }
}