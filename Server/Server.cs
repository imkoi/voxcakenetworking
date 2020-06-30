﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using VoxCake.Common;
using VoxCake.Networking.Common;

namespace VoxCake.Networking
{
    public sealed class Server : IServer
    {
        IDictionary<byte, IClient> IServer.Clients => _clients;
        
        private readonly Dictionary<byte, IClient> _clients;
        private readonly ServerInfo _info;
        private readonly TcpListener _tcpListener;
        private readonly UdpClient _udpClient;

        private Protocol _protocol;
        private bool _isRunning;
        private readonly Func<IClient, bool> _clientValidation;
        private Dictionary<string, object> _properties;

        public event Action OnServerReady;

        public Server(ServerInfo serverInfo, Func<IClient, bool> clientValidation = null)
        {
            _info = serverInfo;
            _properties = new Dictionary<string, object>();
            
            _clients = new Dictionary<byte, IClient>();

            var endPoint = new IPEndPoint(IPAddress.Parse(serverInfo.Ip), serverInfo.Port);
            _tcpListener = new TcpListener(endPoint);
            _udpClient = new UdpClient(serverInfo.Port);

            _clientValidation = clientValidation;
        }

        async Task IServer.Run<T>()
        {
            if (!_isRunning)
            {
                _isRunning = true;
                
                _protocol = new T();
                _protocol.InitializePackets();
                VoxCakeDebugger.LogInfo($"{typeof(T).Name} is binded to server");
            
                RegisterClients();

                var delay = 1000 / _info.TickRate;
                SendData(delay);
                ReceiveData(delay);

                VoxCakeDebugger.LogInfo("Server is running");
                OnServerReady?.Invoke();
            }
            else
            {
                VoxCakeDebugger.LogError("Server is already running");
            }
            
            while (_isRunning)
            {
                await Task.Delay(100);
            }
        }

        void IServer.SetProperty<T>(string key, T value)
        {
            if (_properties.ContainsKey(key))
            {
                _properties[key] = value;
            }
            else
            {
                _properties.Add(key, value);
            }
        }

        T IServer.GetProperty<T>(string key)
        {
            if (_properties.ContainsKey(key))
            {
                return (T) _properties[key];
            }

            return new T();
        }

        void IServer.Shutdown()
        {
            if (_isRunning)
            {
                _isRunning = false;
                _tcpListener.Stop();
                _udpClient.Close();
                _udpClient.Dispose();

                foreach (var keyValuePair in _clients)
                {
                    var client = keyValuePair.Value;
                    client.Disconnect(DisconnectReasonType.ServerIsShutdown);
                }

                VoxCakeDebugger.LogInfo("Server is shutdown");
                Environment.Exit(0);
            }
            else
            {
                VoxCakeDebugger.LogError("Server is already shutdown");
            }
        }

        private async void SendData(int delay)
        {
            while (_isRunning)
            {
                foreach (var keyValuePair in _clients)
                {
                    var client = keyValuePair.Value;
                    //_udpClient.Send(new byte[1] { 1 }, 1);
                }

                await Task.Delay(delay);
            }
        }

        private async void ReceiveData(int delay)
        {
            while (_isRunning)
            {
                var clientResponse = await _udpClient.ReceiveAsync();
                var clientId = clientResponse.Buffer[0];
                Console.WriteLine($"Recieved UDP response from client[{clientId}]");

                await Task.Delay(delay);
            }
        }
        
        private async void RegisterClients()
        {
            _tcpListener.Start();
            
            while (_isRunning)
            {
                var clientTcp = await _tcpListener.AcceptTcpClientAsync();
                var clientStream = clientTcp.GetStream();
            
                var client = new RemoteClient(clientTcp, clientStream, _protocol);

                var clientInfo = await client.ReceiveData<ClientInfoDto>();
                client.properties = clientInfo.properties;

                VoxCakeDebugger.LogInfo($"{client == null}");

                var clientIsValid = _clientValidation?.Invoke(client) ?? true;
                if (clientIsValid)
                {
                    var clientId = (byte) (_clients.Count + 1);

                    await client.SendData(new ClientRegistrationDto
                    {
                        clientId = clientId,
                        serverInfo = new ServerInfoDto
                        {
                            ip = _info.Ip,
                            port = _info.Port,
                            tickRate = _info.TickRate,
                            properties = _properties
                        }
                    });

                    client.Id = clientId;
                    _clients.Add(clientId, client);
                    VoxCakeDebugger.LogInfo($"Client[{client.Id}] is connected to server");
                }
                else
                {
                    client.Disconnect(DisconnectReasonType.ClientIsInvalid);
                }
            }
        }
    }
}