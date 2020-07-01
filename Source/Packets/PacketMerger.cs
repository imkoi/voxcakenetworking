using System.Collections.Generic;

namespace VoxCake.Networking
{
    internal static class PacketMerger
    {
		internal static byte[] GetMergedPacketsData(List<Packet> packets, int packetSizeLimit)
		{
			var packetIndex = 0;
			var resultedPacketSize = 0;
			var packetsToMerge = new List<Packet>();
			var packetsCount = packets.Count;
			Packet packet;

			while (packetSizeLimit > 0 && packetIndex < packetsCount)
			{
				packet = packets[packetIndex];
				if (packet.Size <= packetSizeLimit)
				{
					packetsToMerge.Add(packet);
					resultedPacketSize += packet.Size;
					packetSizeLimit -= packet.Size;
				}
				packetIndex++;
			}

			return GetMergedDataFromPackets(packetsToMerge, resultedPacketSize);
		}

		internal static Packet[] GetPacketsByData(byte[] data, Protocol protocol, byte clientId)
		{
			var size = data.Length;
			var index = 0;
			
			Packet packet;
			var packetID = 0;
			var packetSize = 0;
			var packets = new List<Packet>();
			byte[] packetData;
			
			while (index != size)
			{
				packet = protocol.GetPacketById(data[index]);
				packetSize = packet.Size;
				packetData = new byte[packetSize];
				
				for (var i = 0; i < packetSize; i++)
				{
					packetData[i] = data[index];
					index++;
				}
				
				packet.SenderId = clientId;
				packet.data = packetData;
				packets.Add(packet);
			}
			
			return packets.ToArray();
		}

		private static byte[] GetMergedDataFromPackets(List<Packet> packets, int resultedPacketSize)
		{
			var packetData = new byte[resultedPacketSize];
			var packetCount = packets.Count;

			Packet packet;
			int packetSize;

			var index = 0;
			for (var i = 0; i < packetCount; i++)
			{
				packet = packets[i];
				packetSize = packet.Size;
				if (packet.Size > 1)
				{
					for (var j = 0; j < packetSize; j++)
					{
						packetData[index] = packet.data[j];
						index++;
					}
				}
			}

			return packetData;
		}

		internal static int GetPacketIndexInCollection(Packet packet, List<Packet> collection)
		{
			var packetsCount = collection.Count;
			for (var index = 0; index < packetsCount; index++)
			{
				if (collection[index].GetType() == packet.GetType())
				{
					return index;
				}
			}
			return -1;
		}
	}
}
