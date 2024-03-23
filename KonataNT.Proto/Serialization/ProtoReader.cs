using KonataNT.Proto.Utility;

namespace KonataNT.Proto.Serialization;

internal class ProtoReader(Stream stream)
{
    private uint ReadProtoUInt32()
    {
        uint value = 0;
        int count = 0;
        byte b;
        
        do
        {
            if (stream.IsAvailable(1))
            {
                b = (byte)stream.ReadByte();
                value |= (b & 0b01111111u) << (count * 7);
                ++count;
            }
            else
            {
                throw new Exception();
            }
        }
        while ((b & 0b10000000) > 0);

        return value;
    }
    
    private ulong ReadProtoUInt64()
    {
        ulong value = 0;
        int count = 0;
        byte b;
        
        do
        {
            if (stream.IsAvailable(1))
            {
                b = (byte)stream.ReadByte();
                value |= (b & 0b01111111u) << (count * 7);
                ++count;
            }
            else
            {
                throw new Exception();
            }
        }
        while ((b & 0b10000000) > 0);

        return value;
    }
    
    private ulong ReadProtoUInt16()
    {
        ushort value = 0;
        int count = 0;
        byte b;
        
        do
        {
            if (stream.IsAvailable(1))
            {
                b = (byte)stream.ReadByte();
                value = (ushort)(value | (b & 0b01111111u) << (count * 7));
                ++count;
            }
            else
            {
                throw new Exception();
            }
        }
        while ((b & 0b10000000) > 0);

        return value;
    }
    
    private ulong ReadProtoUInt8()
    {
        byte value = 0;
        int count = 0;
        byte b;
        
        do
        {
            if (stream.IsAvailable(1))
            {
                b = (byte)stream.ReadByte();
                value = (byte)(value | (byte)((b & 0b01111111u) << (count * 7)));
                ++count;
            }
            else
            {
                throw new Exception();
            }
        }
        while ((b & 0b10000000) > 0);

        return value;
    }
    
    public (WireType type, int tag) ReadHead()
    {
        return (0, 0);
    }
}