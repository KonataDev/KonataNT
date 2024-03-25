using KonataNT.Utility.Binary;

namespace KonataNT.Core.Packet;

internal class TlvUnPacker
{
    public Dictionary<ushort, byte[]> TlvMap { get; } = new();
    
    public TlvUnPacker(BinaryPacket reader)
    {
        short tlvCount = reader.ReadShort();

        while (reader.Remaining > 0)
        {
            ushort tag = reader.ReadUshort();
            ushort length = reader.ReadUshort();
            var value = reader.ReadBytes(length);
            TlvMap[tag] = value.ToArray();
        }
    }
}