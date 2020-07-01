using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using VoxCake.Common;
using VoxCake.Networking.Common;

namespace VoxCake.Networking
{
    internal class RemoteClient : IClient
    {
        public byte Id { get; internal set; }
        bool IClient.IsMine => false;

        public event Action<ServerInfo> OnConnected;
        public event Action<DisconnectReasonType> OnDisconnected;

        private readonly TcpClient _tcpClient;
        private readonly UdpClient _udpClient;
        private readonly NetworkStream _stream;
        private readonly Protocol _protocol;

        internal Dictionary<string, object> properties;

        internal RemoteClient(TcpClient tcpClient, Protocol protocol)
        {
            _tcpClient = tcpClient;
            _udpClient = new UdpClient((IPEndPoint)_tcpClient.Client.LocalEndPoint);
            _stream = tcpClient.GetStream();
            _protocol = protocol;
        }

        void IClient.Connect<T>(ServerInfo serverInfo)
        {
            VoxCakeDebugger.LogWarning("Remote client already connected to server!");
        }

        public async Task Send<T>(params object[] data) where T : Packet, new()
        {
            var packet = _protocol.GetPacket<T>();
            packet.SetSenderId(Id);
            packet.ExecuteInternal(data);

            switch (packet.Type)
            {
                case PacketType.Reliable:
                    await _stream.WriteAsync(packet.data, 0, packet.Size);
                    break;
                case PacketType.Unreliable:
                    await _udpClient.SendAsync(packet.data, packet.Size);
                    break;
            }
        }

        public async Task SendData(object data)
        {
            var rawData = NetworkSerializer.Serialize(data);
            await _stream.WriteAsync(rawData, 0, rawData.Length);
        }

        public async Task<T> ReceiveData<T>() where T : new()
        {
            var rawServerResponse = new byte[1024];
            await _stream.ReadAsync(rawServerResponse, 0, 1024);

            return NetworkSerializer.Deserialize<T>(rawServerResponse);
        }

        T IClient.GetProperty<T>(string key)
        {
            if (properties.ContainsKey(key))
            {
                return (T)properties[key];
            }

            return new T();
        }

        public void SetProperty<T>(string key, T value)
        {
            if (properties.ContainsKey(key))
            {
                properties[key] = value;
            }
            else
            {
                properties.Add(key, value);
            }
        }

        public void Disconnect(DisconnectReasonType disconnectReason)
        {
            _stream?.Close();
            _stream?.Dispose();

            _tcpClient?.Close();
            _tcpClient?.Dispose();

            _udpClient?.Close();
            _udpClient?.Dispose();

            OnDisconnected?.Invoke(disconnectReason);
        }

        public async Task<Packet[]> ReceiveTcpPackets(byte clientId)
        {
            var clientResponse = new byte[256];
            await _stream.ReadAsync(clientResponse, 0, 256);

            if (clientResponse[0] == 0)
            {
                return null;
            }

            var packet = _protocol.GetPacketById(clientResponse[0]);
            packet.SetSenderId(clientId);

            return new Packet[] { packet };
        }

        public async Task<Packet[]> ReceiveUdpPackets(byte clientId)
        {
            var clientResponseResult = await _udpClient.ReceiveAsync();
            var clientResponse = clientResponseResult.Buffer;

            var packet = _protocol.GetPacketById(clientResponse[0]);
            packet.SetSenderId(clientId);

            return new Packet[] { packet };
        }
    }
}
