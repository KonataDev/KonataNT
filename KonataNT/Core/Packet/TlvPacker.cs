using KonataNT.Common;
using KonataNT.Core.Packet.Login;
using KonataNT.Utility.Binary;

namespace KonataNT.Core.Packet;

internal class TlvPacker(BotKeystore keystore, BotAppInfo appInfo)
{
    public BinaryPacket PackUp(List<ushort> tags, bool transEmp)
    {
        var writer = new BinaryPacket().WriteUshort((ushort)tags.Count);
        
        foreach (var tag in tags)
        {
            var body = transEmp
                ? GenerateTransTlvBody(tag)
                : GenerateTlvBody(tag);
            
            writer.WriteUshort(tag);
            writer.WriteUshort((ushort)body.Length);
            writer.WritePacket(body);
        }

        return writer;
    }

    private BinaryPacket GenerateTlvBody(ushort tag) => tag switch
    {
        
    };
    
    private BinaryPacket GenerateTransTlvBody(ushort tag) => tag switch
    {
        0x11 => new BinaryPacket()
            .WriteBytes(keystore.UnusualSign ?? throw new InvalidOperationException()),
        
        0x16 => new BinaryPacket()
            .WriteUint(0)
            .WriteUint((uint)appInfo.SubAppId)
            .WriteUint((uint)appInfo.AppIdQrCode)
            .WriteBytes(keystore.Guid.AsSpan())
            .WriteString(appInfo.PackageName, Prefix.Uint16 | Prefix.LengthOnly)
            .WriteString(appInfo.PtVersion, Prefix.Uint16 | Prefix.LengthOnly)
            .WriteString(appInfo.PackageName, Prefix.Uint16 | Prefix.LengthOnly),
        
        0x1B => new BinaryPacket()
            .WriteUint(0)  // Micro
            .WriteUint(0)  // Version
            .WriteUint(3)  // Size
            .WriteUint(4)  // Margin
            .WriteUint(72) // Dpi
            .WriteUint(2)  // EcLevel
            .WriteUint(2)  // Hint
            .WriteUshort(0), // Field0
        
        0x1D => new BinaryPacket()
            .WriteByte(1)
            .WriteUint((uint)appInfo.MiscBitmap)
            .WriteUint(0)
            .WriteByte(0),
        
        0x33 => new BinaryPacket()
            .WriteBytes(keystore.Guid.AsSpan()),
        
        0x35 => new BinaryPacket()
            .WriteInt(appInfo.PtOsVersion),
        
        0x66 => new BinaryPacket()
            .WriteInt(appInfo.PtOsVersion),
        
        0xD2 => new BinaryPacket()
            .WriteBytes(new NTLoginInfo
            {
                SystemInfo = new NTSystemInfo
                {
                    Os = appInfo.Os,
                    DeviceName = "Lagrange"
                },
                Type = [0x30, 0x01]
            }.Serialize()),
        
        _ => throw new InvalidDataException()
    };
}