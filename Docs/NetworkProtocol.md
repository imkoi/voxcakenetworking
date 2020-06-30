NetworkStream
{
	ClientResponse[clientsCount] clientsResponse;
}

ClientResponse
{
	byte clientId;
	PacketData[] packets;
}

PacketData
{
	byte packetId;
	byte[] packetData;
}

var clientOffset = 0;
var clientId = data[clientOffset];

for(var i = 1; i < data.Lenght; i++)
{
	var packetId = data[i];
	var packet = Protocol.GetPacket(packetId);
	var packetSize = packet.Size;
	for(var j = 0; j < packetSize; j++)
	{
	
	}

}