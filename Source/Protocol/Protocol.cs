using VoxCake.Common;

namespace VoxCake.Networking
{
    public abstract class Protocol
    {
        public int Size => Packets.Length;
        protected abstract Packet[] Packets { get; }

        private Packet[] _packets;

        internal void InitializePackets()
        {
            var count = Packets.Length;
            _packets = new Packet[count + 1];
            for (byte i = 0; i < count; i++)
            {
                var iPlus = (byte)(i + 1);

                var packet = Packets[i];
                packet.Initialize(iPlus);
                _packets[iPlus] = packet;
                VoxCakeDebugger.LogInfo($"{packet.GetType().Name} is binded to {GetType().Name}");
            }
        }

        internal Packet GetPacket<T>() where T : Packet
        {
            var packetType = typeof(T);
            var count = _packets.Length;
            for (var i = 1; i < count; i++)
            {
                var suspectPacket = _packets[i];
                if (suspectPacket.GetType() == packetType)
                {
                    return suspectPacket;
                }
            }

            VoxCakeDebugger.LogError($"{packetType.Name} not found in {GetType().Name}");
            return null;
        }

        internal Packet GetPacketById(int packetId)
        {
            return _packets[packetId];
        }

        public override int GetHashCode()
        {
            var hash = 0;

            var count = Packets.Length;
            for (byte i = 0; i < count; i++)
            {
                var packet = Packets[i];
                packet.Initialize(i);
                hash += packet.GetHashCode();
            }

            return 0;
        }
    }
}