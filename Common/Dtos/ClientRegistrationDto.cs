using System;

namespace VoxCake.Networking.Common
{
    [Serializable]
    internal class ClientRegistrationDto
    {
        public byte clientId;
        public ServerInfoDto serverInfo;

        public override string ToString()
        {
            return $"[ clientId = {clientId}, server = {serverInfo.ip}:{serverInfo.port}]";
        }
    }
}