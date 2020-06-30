using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using VoxCake.Common;
using VoxCake.Networking.Common;

namespace VoxCake.Networking
{
    public sealed class Client : IClient
    {
        private const int ConnectRetryDelay = 5000;

        public byte Id { get; internal set; }
        public bool IsMine => _isMine;
        

        public event Action<ServerInfo> OnConnected;
        public event Action<DisconnectReasonType> OnDisconnected;

        private readonly TcpClient _tcpClient;
        private readonly UdpClient _udpClient;
        private IPEndPoint _serverEndPoint;
        private NetworkStream _stream;
        private Protocol _protocol;
        private bool _isConnected;
        private bool _isRunning;
        private bool _isMine;
        private ClientModeType _clientMode;

        internal EndPoint Ip => _tcpClient.Client.RemoteEndPoint;
        internal Dictionary<string, object> properties;

        public Client(ClientModeType clientMode = ClientModeType.Online)
        {
            _clientMode = clientMode;
            _isMine = true;
            OnConnected += serverInfo => { _isConnected = true; };
            OnDisconnected += reason => { _isConnected = false; };

            properties = new Dictionary<string, object>();

            if (clientMode == ClientModeType.Online)
            {
                _tcpClient = new TcpClient();
                _udpClient = new UdpClient();
            }
        }

        async void IClient.Connect<T>(ServerInfo serverInfo)
        {
            if (!_isConnected)
            {
                _isRunning = true;

                _protocol = new T();
                _protocol.InitializePackets();

                if(_clientMode == ClientModeType.Online)
                {
                    if (await TryConnectToTcpServer(serverInfo))
                    {
                        _stream = _tcpClient.GetStream();

                        await SendData(new ClientInfoDto
                        {
                            ip = "178.121.82.197",
                            port = 8888,
                            properties = properties
                        });

                        var clientRegistrationData = await ReceiveData<ClientRegistrationDto>();
                        Id = clientRegistrationData.clientId;

                        var serverInfoResponse = new ServerInfo(clientRegistrationData.serverInfo);
                        OnConnected?.Invoke(serverInfoResponse);
                    }
                }
                else
                {
                    OnConnected?.Invoke(serverInfo);
                }
            }
            else
            {
                VoxCakeDebugger.LogWarning("Cannot connect to server, because you are already connected!");
            }
        }

        async Task IClient.Send<T>(params object[] data)
        {
            var packetName = typeof(T).Name;

            if (_isConnected)
            {
                var packet = _protocol.GetPacket<T>();
                packet.SetSenderId(Id);
                packet.ExecuteInternal(data);

                if (_clientMode == ClientModeType.Online)
                {
                    switch (packet.Type)
                    {
                        case PacketType.Reliable:
                            await _stream.WriteAsync(packet.data, 0, packet.Size);
                            VoxCakeDebugger.LogInfo($"Packet \"{packetName}\" sended to {_serverEndPoint.Address} via TCP protocol");
                            break;
                        case PacketType.Unreliable:
                            await _udpClient.SendAsync(packet.data, packet.Size, _serverEndPoint);
                            VoxCakeDebugger.LogInfo($"Packet \"{packetName}\" sended to {_serverEndPoint.Address} via UDP protocol");
                            break;
                    }
                }
            }
            else
            {
                VoxCakeDebugger.LogWarning($"Cannot send {packetName} because you are not connected to any server!");
            }
        }

        async Task IClient.SendData(object data)
        {
            var dataName = data.GetType().Name;

            if (_isConnected)
            {
                await SendData(data);
                VoxCakeDebugger.LogInfo($"Data \"{dataName}\" sended to {_serverEndPoint.Address} via TCP protocol");
            }
            else
            {
                VoxCakeDebugger.LogWarning($"Cannot send {dataName} because you are not connected to any server!");
            }
        }

        async Task<T> IClient.ReceiveData<T>()
        {
            var data = new T();

            if (_isConnected)
            {
                data = await ReceiveData<T>();

                VoxCakeDebugger.LogInfo($"Data \"{data.GetType().Name}\" received from {_serverEndPoint.Address} via TCP protocol");
            }

            VoxCakeDebugger.LogError($"Cannot receive data because you are not connected to any server!");
            return data;
        }

        void IClient.SetProperty<T>(string key, T value)
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

        T IClient.GetProperty<T>(string key)
        {
            if (properties.ContainsKey(key))
            {
                return (T)properties[key];
            }

            return new T();
        }

        void IClient.Disconnect(DisconnectReasonType disconnectReason)
        {
            _stream?.Close();
            _tcpClient?.Close();
            _udpClient?.Close();
            OnDisconnected?.Invoke(disconnectReason);

            _isConnected = false;
            _isRunning = false;
        }

        private async Task<bool> TryConnectToTcpServer(ServerInfo serverInfo)
        {
            while (_isRunning)
            {
                try
                {
                    await _tcpClient.ConnectAsync(serverInfo.Ip, serverInfo.Port);
                    _serverEndPoint = new IPEndPoint(IPAddress.Parse(serverInfo.Ip), serverInfo.Port);

                    VoxCakeDebugger.LogInfo("Client connected to tcp server");

                    return true;
                }
                catch
                {
                    VoxCakeDebugger.LogWarning("Cannot connect to tcp server, retry in 5 seconds...");
                    await Task.Delay(ConnectRetryDelay);

                    if (!_isRunning)
                    {
                        return false;
                    }
                }
            }

            return false;
        }

        private async Task<T> ReceiveData<T>() where T : new()
        {
            var response = new T();

            if (_clientMode == ClientModeType.Online)
            {
                var rawServerResponse = new byte[1024];
                await _stream.ReadAsync(rawServerResponse, 0, 1024);

                response = NetworkSerializer.Deserialize<T>(rawServerResponse);
            }

            return response;
        }

        private async Task SendData(object data)
        {
            var rawData = NetworkSerializer.Serialize(data);

            if (_clientMode == ClientModeType.Online)
            {
                await _stream.WriteAsync(rawData, 0, rawData.Length);
            }
        }

        private async void SendTCPDataToServer()
        {

        }

        private async void SendUDPDataToServer()
        {

        }

        private async void ReceiveTCPDataFromServer()
        {

        }

        private async void ReceiveUDPDataFromServer()
        {

        }
    }
}