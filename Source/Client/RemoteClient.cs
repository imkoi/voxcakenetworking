using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using VoxCake.Common;
using VoxCake.Networking.Common;

namespace VoxCake.Networking
{
    internal class RemoteClient : IClient
    {
        public byte Id { get; internal set; }
        public bool IsMine => false;

        public event Action<ServerInfo> OnConnected;
        public event Action<DisconnectReasonType> OnDisconnected;

        private readonly TcpClient _tcpClient;
        private readonly UdpClient _udpClient;
        private readonly NetworkStream _stream;
        private readonly Protocol _protocol;

        internal Dictionary<string, object> properties;

        internal RemoteClient(TcpClient tcpClient, NetworkStream clientStream, Protocol protocol)
        {
            _tcpClient = tcpClient;
            _stream = clientStream;
            _protocol = protocol;
        }

        public void Connect<T>(ServerInfo serverInfo) where T : Protocol, new()
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

        public T GetProperty<T>(string key) where T : new()
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
            _tcpClient?.Close();
            _udpClient?.Close();
            OnDisconnected?.Invoke(disconnectReason);
        }
    }
}
