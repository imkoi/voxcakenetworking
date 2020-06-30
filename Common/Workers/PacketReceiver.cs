using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace VoxCake.Networking.Common
{
    public class PacketReceiver : Worker
    {
        private readonly TcpListener _tcpListener;
        private readonly Dictionary<byte, IClient> _tcpClients;
        
        public PacketReceiver(TcpListener tcpListener, Dictionary<byte, IClient> clients)
        {
            _tcpListener = tcpListener;
            _tcpClients = clients;
        }

        protected override Action Work => () =>
        {
            foreach (var keyValuePair in _tcpClients)
            {
                var client = keyValuePair.Value;
                var clientId = client.Id;
                //client.
            }
        };
    }
}