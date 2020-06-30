using System;
using System.Threading.Tasks;

namespace VoxCake.Networking
{
    public interface IClient
    {
        byte Id { get; }
        bool IsMine { get; }

        event Action<ServerInfo> OnConnected;
        event Action<DisconnectReasonType> OnDisconnected;

        void Connect<T>(ServerInfo serverInfo) where T : Protocol, new();

        Task SendData(object data);
        Task Send<T>(params object[] data) where T : Packet, new();
        Task<T> ReceiveData<T>() where T : new();
        
        void SetProperty<T>(string key, T value);
        T GetProperty<T>(string key) where T : new();

        void Disconnect(DisconnectReasonType disconnectReason);
    }
}