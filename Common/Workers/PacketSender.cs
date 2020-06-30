using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace VoxCake.Networking.Common
{
    public class PacketSender : Worker
    {
        private readonly TcpListener _tcpListener;
        private readonly Dictionary<byte, IClient> _clients;
        
        public PacketSender(TcpListener tcpListener, Dictionary<byte, IClient> clients)
        {
            _tcpListener = tcpListener;
            _clients = clients;
        }

        protected override Action Work => async () =>
        {
            
        };
    }
}