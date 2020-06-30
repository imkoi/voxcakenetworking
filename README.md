# VoxCake.Networking
Currenty unusable :)

### Usage example
``` csharp
public class ServerExample
{
    private const string ProtocolHashKey = "protocol_hash";
    private readonly int _protocolHash = new ProtocolExample().GetHashCode();

    private IServer _server;

    public ServerExample()
    {
        _server = new Server(new ServerInfo("127.0.0.1", 8888), ClientValidation);

        _server.SetProperty(ProtocolHashKey, _protocolHash);

        _server.Run<ProtocolExample>();
    }

    private bool ClientValidation(IClient client)
    {
        var clientProtocolHash = client.GetProperty<int>(ProtocolHashKey);
        var serverProtocolHash = _server.GetProperty<int>(ProtocolHashKey);

        return clientProtocolHash == serverProtocolHash;
    }
}

public class ClientExample
{
    private const string ProtocolHashKey = "protocol_hash";
    private readonly int _protocolHash = new ProtocolExample().GetHashCode();

    private readonly IClient _client;

    public ClientExample()
    {
        _client = new Client();
        _client.OnConnected += SendExamplePacket;
        _client.OnDisconnected += Disconnect;

        _client.SetProperty(ProtocolHashKey, _protocolHash);

        _client.Connect<ProtocolExample>(new ServerInfo("127.0.0.1", 8888));
    }

    private void SendExamplePacket(ServerInfo serverInfo)
    {
        _client.Send<PacketExample>(128);
    }

    private void Disconnect(DisconnectReasonType disconnectReason)
    {
        _client.Disconnect(disconnectReason);
    }
}

public class ProtocolExample : Protocol
{
    protected override Packet[] Packets => _packets;

    private readonly Packet[] _packets = new Packet[]
    {
        new PacketExample()
    };
}

public class PacketExample : Packet
{
    public override PacketType Type => PacketType.Reliable;

    protected override void VariableBindings(PacketVariableBinder variableBinder)
    {
        variableBinder.Bind<int>();
    }

    protected override void Execute(PacketVariableGetter variables)
    {
        var variableReceivedFromServer = variables.Get<int>();
    }
}
```