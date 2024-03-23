namespace KonataNT.Proto.Serialization;

internal class ProtoWriter(Stream stream)
{
    public void WriteHead(WireType type, uint tag)
    {
        uint head = tag << 3 | (byte)type;
        WriteVarInt(head);
    }

    private void WriteVarInt(ulong value)
    {
        if (value >= 127)
        {
            int len = 0;
            Span<byte> buffer = stackalloc byte[10];

            do
            {
                buffer[len] = (byte)((value & 127) | 128);
                value >>= 7;
                ++len;
            } while (value > 127);

            buffer[len] = (byte)value;
            stream.Write(buffer[..(len + 1)]);
        }
        else 
        {
            stream.WriteByte((byte)value);
        }
    }
}