using System.Buffers.Binary;
using System.Collections.Concurrent;
using KonataNT.Core.Network;
using KonataNT.Core.Packet;
using KonataNT.Utility;
using KonataNT.Utility.Binary;
using KonataNT.Utility.Crypto;

namespace KonataNT.Core;

/// <summary>
/// Packet handler is responsible for encrypt/decrypt/packup/unpack packets
/// </summary>
internal class PacketHandler : ClientListener
{
    private const string Tag = nameof(PacketHandler);

    private readonly ConcurrentDictionary<int, TaskCompletionSource<byte[]>> _pendingRequests;
    
    public override uint HeaderSize => 4;
    
    private int _sequence = Random.Shared.Next(5000000, 9900000);

    private readonly ClientListener _tcpClient;
    
    private readonly BaseClient _client;

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
                    tcs.SetResult(isCompressed == 0 ? reader.ToArray() : InflatePacket(reader).ToArray());
                    _client.Logger.LogDebug(Tag, $"Received SSOFrame: {sequence} | {command}");
                }
            }
            else
            {
                _client.Logger.LogWarning(Tag, $"Received SSOFrame with non-zero retCode: {retCode} | {extra}");
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

    public Task<byte[]> SendPacket(string command, byte[] payload, int protocol = 12)
    {
        int sequence = Interlocked.Increment(ref _sequence);
        var sso = new BinaryPacket();
        var reserve = new NTDeviceSign
        {

        };

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
        }
        else
        {
            _client.Logger.LogWarning(Tag, $"Unsupported protocol: {protocol}");
            return Task.FromResult(Array.Empty<byte>());
        }

        var encrypted = protocol == 12 
            ? TeaProvider.Encrypt(sso.ToArray(), _client.KeyStore.D2Key)
            : payload;

        var service = new BinaryPacket();
        service.Barrier(w => w
            .WriteUint((uint)protocol)
            .WriteByte(protocol == 12 ? (byte)(_client.KeyStore.D2.Length == 0 ? 2 : 1) : (byte)0)
            .WriteBytes(_client.KeyStore.D2, Prefix.Uint32 | Prefix.WithPrefix)
            .WriteByte(0)
            .WriteString(_client.KeyStore.Uin.ToString(), Prefix.Uint32 | Prefix.WithPrefix)
            .WriteBytes(encrypted, Prefix.Uint32 | Prefix.WithPrefix), Prefix.Uint32 | Prefix.WithPrefix);
        
        
        var tcs = new TaskCompletionSource<byte[]>();
        _pendingRequests.TryAdd(sequence, tcs);
        return tcs.Task;
    }

    public override void OnDisconnect()
    {
        throw new NotImplementedException();
    }

    public override void OnSocketError(Exception e)
    {
        throw new NotImplementedException();
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