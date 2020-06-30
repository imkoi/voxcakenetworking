using VoxCake.Common;

namespace VoxCake.Networking
{
    public abstract class Protocol
    {
        public int Size => Packets.Length;
        protected abstract Packet[] Packets { get; }


        internal void InitializePackets()
        {
            var count = Packets.Length;
            for (byte i = 0; i < count; i++)
            {
                var packet = Packets[i];
                packet.Initialize(i);
                VoxCakeDebugger.LogInfo($"{packet.GetType().Name} is binded to {GetType().Name}");
            }
        }

        public Packet GetPacket<T>() where T : Packet
        {
            var packetType = typeof(T);
            var count = Packets.Length;
            for (var i = 0; i < count; i++)
            {
                var suspectPacket = Packets[i];
                if (suspectPacket.GetType() == packetType)
                {
                    return suspectPacket;
                }
            }

            VoxCakeDebugger.LogError($"{packetType.Name} not found in {GetType().Name}");
            return null;
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