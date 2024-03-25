using System.Text;
using KonataNT.Common;
using KonataNT.Core.Packet.Login;
using KonataNT.Utility;
using KonataNT.Utility.Binary;
using KonataNT.Utility.Crypto;

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
        0x18 => new BinaryPacket()
            .WriteUshort(0)  // PingVersion
            .WriteUint(5)  // SsoVersion
            .WriteUint(0)  // AppId
            .WriteUint(8001)  // AppClientVersion
            .WriteUint(keystore.Uin)  // Uin
            .WriteUshort(0)  // Field0
            .WriteUshort(0),  // Field1
        
        0x100 => new BinaryPacket()
            .WriteUshort(0)  // DbBufVersion
            .WriteUint(5)  // SsoVersion
            .WriteUint((uint)appInfo.AppId)
            .WriteUint((uint)appInfo.SubAppId)
            .WriteUint(appInfo.AppClientVersion)
            .WriteUint(appInfo.MainSigMap),
        
        0x106 => new BinaryPacket()
            .WriteBytes(keystore.A2 ?? throw new Exception("A2 is not set")),
        
        0x107 => new BinaryPacket()
            .WriteUshort(0x0001)  // PicType
            .WriteByte(0x0D)  // CapType
            .WriteUshort(0x0)  // PicSize
            .WriteByte(0x01),  // RetType
        
        0x116 => new BinaryPacket()
            .WriteByte(0)
            .WriteUint(12058620)  // MiscBitmap
            .WriteUint(appInfo.SubSigMap)
            .WriteByte(0),
        
        0x124 => new BinaryPacket()
            .WriteBytes(new byte[12]),
        
        0x128 => new BinaryPacket()
            .WriteUshort(0)  // Const0
            .WriteByte(0)  // GuidNew
            .WriteByte(0)  // GuidAvailable
            .WriteByte(0)  // GuidChanged
            .WriteUint(0)  // GuidFlag
            .WriteString(appInfo.Os, Prefix.Uint16 | Prefix.LengthOnly)
            .WriteBytes(keystore.Guid.AsSpan(), Prefix.Uint16 | Prefix.LengthOnly)
            .WriteUshort(0),
        
        0x141 => new BinaryPacket()
            .WriteUshort(0)
            .WriteString("Unknown", Prefix.Uint16 | Prefix.LengthOnly)
            .WriteUshort(0)
            .WriteString("", Prefix.Uint16 | Prefix.LengthOnly),
        
        0x142 => new BinaryPacket()
            .WriteUshort(0)
            .WriteString(appInfo.PackageName, Prefix.Uint16 | Prefix.LengthOnly),
        
        0x144 => new BinaryPacket()
            .WriteBytes(TeaProvider.Encrypt(PackUp([0x16E, 0x147, 0x128, 0x124], false).ToArray(), keystore.TgtgtKey).AsSpan()),
        
        0x145 => new BinaryPacket()
            .WriteBytes(keystore.Guid.AsSpan()),
        
        0x147 => new BinaryPacket()
            .WriteUint((uint)appInfo.AppId)
            .WriteString(appInfo.PtVersion, Prefix.Uint16 | Prefix.LengthOnly)
            .WriteString(appInfo.PackageName, Prefix.Uint16 | Prefix.LengthOnly),
        
        0x166 => new BinaryPacket()
            .WriteByte(5),  // ImageType
        
        0x16A => new BinaryPacket()
            .WriteBytes(keystore.NoPicSig.AsSpan()),
        
        0x16E => new BinaryPacket()
            .WriteBytes(Encoding.UTF8.GetBytes(keystore.Name)),
        
        0x177 => new BinaryPacket()
            .WriteByte(1)  // Field1
            .WriteUint(0)  // BuildTime
            .WriteString(appInfo.WtLoginSdk, Prefix.Uint16 | Prefix.LengthOnly),
        
        0x191 => new BinaryPacket()
            .WriteByte(0), // K
        
        0x318 => new BinaryPacket(),
        
        0x521 => new BinaryPacket()
            .WriteUint(0x13)  // ProductType
            .WriteString("basicim", Prefix.Uint16 | Prefix.LengthOnly),  // ProductDesc
        
_ => throw new InvalidDataException()
    };
    
    private BinaryPacket GenerateTransTlvBody(ushort tag) => tag switch
    {
        0x11 => new BinaryPacket()
            .WriteBytes(keystore.UnusualSign ?? throw new InvalidOperationException()),
        
        0x16 => new BinaryPacket()
            .WriteUint(0)
            .WriteUint((uint)appInfo.AppId)
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
        
        0xD1 => new BinaryPacket()
            .WriteBytes(new NTLoginInfo
            {
                SystemInfo = new NTSystemInfo
                {
                    Os = appInfo.Os,
                    DeviceName = keystore.Name
                },
                Type = [0x30, 0x01]
            }.Serialize()),
        
        _ => throw new InvalidDataException()
    };
}