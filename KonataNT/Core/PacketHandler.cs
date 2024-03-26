using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Net;
using KonataNT.Core.Network;
using KonataNT.Core.Packet;
using KonataNT.Core.Packet.Service.Oidb;
using KonataNT.Utility;
using KonataNT.Utility.Binary;
using KonataNT.Utility.Crypto;
using KonataNT.Utility.Generator;
using KonataNT.Utility.Network;
using KonataNT.Utility.Sign;

namespace KonataNT.Core;

/// <summary>
/// Packet handler is responsible for encrypt/decrypt/packup/unpack packets
/// </summary>
internal class PacketHandler : ClientListener
{
    private const string Tag = nameof(PacketHandler);
    
    private readonly BaseClient _client;

    private readonly ConcurrentDictionary<int, TaskCompletionSource<byte[]>> _pendingRequests;
    public override uint HeaderSize => 4;
    
    private int _sequence = Random.Shared.Next(5000000, 9900000);

    private readonly ClientListener _tcpClient;
    
    private readonly Signer _signer = new();
    
    public Uri? ServerUri { get; set; }


    public PacketHandler(BaseClient client)
    {
        _client = client;
        _pendingRequests = new ConcurrentDictionary<int, TaskCompletionSource<byte[]>>();
        _tcpClient = new CallbackClientListener(this);
    }
    
    public override uint GetPacketLength(ReadOnlySpan<byte> header) => BinaryPrimitives.ReadUInt32BigEndian(header);

    public override void OnRecvPacket(ReadOnlySpan<byte> packet)
    {
        var reader = new BinaryPacket(packet.ToArray());
        
        uint length = reader.ReadUint();
        uint protocol = reader.ReadUint();
        byte authFlag = reader.ReadByte();
        byte flag = reader.ReadByte();
        string uin = reader.ReadString(Prefix.Uint32 | Prefix.WithPrefix);
        reader = new BinaryPacket(TeaProvider.Decrypt(reader.ReadBytes(), _client.KeyStore.D2Key));
        
        if (protocol == 12)
        {
            uint headLength = reader.ReadUint();
            uint sequence = reader.ReadUint();
            int retCode = reader.ReadInt();
            string extra = reader.ReadString(Prefix.Uint32 | Prefix.WithPrefix);
            string command = reader.ReadString(Prefix.Uint32 | Prefix.WithPrefix);
            reader.ReadString(Prefix.Uint32 | Prefix.WithPrefix); // unknown
            int isCompressed = reader.ReadInt();
            reader.ReadString(Prefix.Uint32 | Prefix.LengthOnly); // sso header

            if (retCode == 0)
            {
                if (_pendingRequests.TryRemove((int)sequence, out var tcs))
                {
                    var payload = isCompressed == 0
                        ? reader.ReadBytes(Prefix.Uint32 | Prefix.WithPrefix).ToArray()
                        : InflatePacket(reader).ToArray();
                    tcs.SetResult(payload);
                    
                    _client.Logger.LogDebug(Tag, $"Received SSOFrame: {sequence} | {command}");
                }
            }
            else
            {
                _client.Logger.LogWarning(Tag, $"Received SSOFrame {sequence} | {command} with non-zero retCode: {retCode} | {extra}");
            }
        }
        else if (protocol == 13)
        {
            // TODO: Implement protocol 13
        }
        else
        {
            _client.Logger.LogWarning(Tag, "Unsupported protocol: {protocol}");
        }
    }

    public Task<byte[]> SendOidb(ushort command, ushort subCommand, byte[] body, bool isUid)
    {
        string cmd = $"OidbSvcTrpcTcp.{command:0x}_{subCommand}";

        var packet = new OidbSvcBase
        {
            Command = command,
            SubCommand = subCommand,
            Body = body,
            IsUid = isUid
        };
        
        return SendPacket(cmd, packet.Serialize());
    }

    public async Task<byte[]> SendPacket(string command, byte[] payload, int protocol = 12)
    {
        int sequence = Interlocked.Increment(ref _sequence);
        var sso = new BinaryPacket();

        var sign = _signer.Sign(command, (uint)sequence, payload, out var ver, out var token);
        
        var reserve = new NTDeviceSign
        {
            Trace = StringGen.GenerateTrace(),
            Uid = _client.KeyStore.Uid,
        };
        
        if (ver != null && token != null && sign != null)
        {
            reserve.Sign = new Sign
            {
                Signature = sign,
                Token = token,
                Extra = ver
            };
        }

        if (protocol == 13)
        {
            sso.Barrier(w => w
                .WriteString(command, Prefix.Uint32 | Prefix.WithPrefix)
                .WriteBytes(Array.Empty<byte>(), Prefix.Uint32 | Prefix.WithPrefix) // TODO: Unknown
                .WriteBytes(reserve.Serialize(), Prefix.Uint32 | Prefix.WithPrefix), Prefix.Uint32 | Prefix.WithPrefix);
        }
        else if (protocol == 12)
        {
            sso.Barrier(w => w
                .WriteUint((uint)sequence)
                .WriteUint((uint)_client.AppInfo.SubAppId)
                .WriteUint(2052)
                .WriteBytes("020000000000000000000000".UnHex().AsSpan())
                .WriteBytes(_client.KeyStore.Tgt, Prefix.Uint32 | Prefix.WithPrefix)
                .WriteString(command, Prefix.Uint32 | Prefix.WithPrefix)
                .WriteBytes(Array.Empty<byte>(), Prefix.Uint32 | Prefix.WithPrefix)
                .WriteString(_client.KeyStore.Guid.Hex().ToLower(), Prefix.Uint32 | Prefix.WithPrefix)
                .WriteBytes(Array.Empty<byte>(), Prefix.Uint32 | Prefix.WithPrefix)
                .WriteString(_client.AppInfo.CurrentVersion, Prefix.Uint16 | Prefix.WithPrefix)
                .WriteBytes(reserve.Serialize(), Prefix.Uint32 | Prefix.WithPrefix), Prefix.Uint32 | Prefix.WithPrefix);
            sso.WriteBytes(payload, Prefix.Uint32 | Prefix.WithPrefix);
        }
        else
        {
            _client.Logger.LogWarning(Tag, $"Unsupported protocol: {protocol}");
            return Array.Empty<byte>();
        }

        var encrypted = protocol == 12 
            ? TeaProvider.Encrypt(sso.ToArray(), _client.KeyStore.D2Key)
            : payload;
        sso.WriteBytes(encrypted, Prefix.Uint32 | Prefix.WithPrefix);

        var service = new BinaryPacket();
        service.Barrier(w => w
            .WriteUint((uint)protocol)
            .WriteByte(protocol == 12 ? (byte)(_client.KeyStore.D2.Length == 0 ? 2 : 1) : (byte)0)
            .WriteBytes(_client.KeyStore.D2, Prefix.Uint32 | Prefix.WithPrefix)
            .WriteByte(0)
            .WriteString(_client.KeyStore.Uin.ToString(), Prefix.Uint32 | Prefix.WithPrefix)
            .WriteBytes(encrypted), Prefix.Uint32 | Prefix.WithPrefix);
        
        _client.Logger.LogDebug(Tag, $"Sending SSOFrame: {sequence} | {command}");
        await Send(service.ToArray());
        
        var tcs = new TaskCompletionSource<byte[]>();
        _pendingRequests.TryAdd(sequence, tcs);
        return await tcs.Task;
    }

    public async Task<bool> Connect()
    {
        if (_tcpClient.Connected) return true;

        var servers = await OptimumServer(_client.Config.GetOptimumServer, _client.Config.UseIPv6Network);
        ServerUri = servers.First();
        _client.Logger.LogInformation(Tag, $"Trying to connect to {ServerUri}");
        
        return await _tcpClient.Connect(ServerUri.Host, ServerUri.Port);
    }
    
    public Task<bool> Send(byte[] packet) => _tcpClient.Send(packet);
    
    private async Task<List<Uri>> OptimumServer(bool requestMsf, bool useIPv6Network = false)
    {
        var result = await ResolveDns(useIPv6Network);
        var latencyTasks = result.Select(uri => Icmp.PingAsync(uri)).ToArray();
        var latency = await Task.WhenAll(latencyTasks);
        Array.Sort(latency, result);
        
        var list = result.ToList();
        for (int i = 0; i < list.Count; i++) _client.Logger.LogInformation(Tag, $"Server: {list[i]} Latency: {latency[i]}");
        return list;
    }
    
    private static async Task<Uri[]> ResolveDns(bool useIPv6Network = false)
    {
        string dns = useIPv6Network ? "msfwifiv6.3g.qq.com" : "msfwifi.3g.qq.com";
        var addresses = await Dns.GetHostEntryAsync(dns);
        var result = new Uri[addresses.AddressList.Length];
        
        for (int i = 0; i < addresses.AddressList.Length; i++) result[i] = new Uri($"http://{addresses.AddressList[i]}:8080");

        return result;
    }

    public override void OnDisconnect()
    {
        _client.Logger.LogFatal(Tag, "Socket Disconnected, Scheduling Reconnect");
        
        if (_client.Config.AutoReconnect)
        {
            // TODO: Reconnection
        }
    }

    public override void OnSocketError(Exception e)
    {
        _client.Logger.LogFatal(Tag, $"Socket Error: {e.Message}");
        _tcpClient.Disconnect();
        if (!_tcpClient.Connected) OnDisconnect();
    }
    
    private static BinaryPacket InflatePacket(BinaryPacket original)
    {
        var raw = original.ReadBytes(Prefix.Uint32 | Prefix.WithPrefix);
        var decompressed = CompressionHelper.ZDecompress(raw);
        
        var stream = new MemoryStream();
        stream.Write(decompressed);
        stream.Position = 0;
        
        return new BinaryPacket(stream);
    }
}