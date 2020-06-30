using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VoxCake.Networking
{
    public interface IServer
    {
        IDictionary<byte, IClient> Clients { get; }
        event Action OnServerReady;

        
        Task Run<T>() where T : Protocol, new();
        

        void SetProperty<T>(string key, T value);
        T GetProperty<T>(string key) where T : new();
        void Shutdown();
    }
}