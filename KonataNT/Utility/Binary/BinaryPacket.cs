using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace KonataNT.Utility.Binary;

internal unsafe class BinaryPacket : IDisposable
{
    private readonly MemoryStream _stream;

    private readonly BinaryReader _reader;
    
    public long Length => _stream.Length;
    
    public long Remaining => _stream.Length - _stream.Position;
    
    public BinaryPacket()
    {
        _stream = new MemoryStream();
        _reader = new BinaryReader(_stream);
    }
    
    public BinaryPacket(byte[] data)
    {
        _stream = new MemoryStream(data);
        _reader = new BinaryReader(_stream);
    }
    
    public BinaryPacket(MemoryStream stream)
    {
        _stream = stream;
        _reader = new BinaryReader(_stream);
    }

    public BinaryPacket WriteBytes(ReadOnlySpan<byte> value, Prefix flag, int addition = 0)
    {
        WriteLength(value.Length, flag, addition);
        return WriteBytes(value);
    }

    private BinaryPacket WriteLength(int origin, Prefix flag, int addition)
    {
        bool lengthCounted = (flag & Prefix.WithPrefix) > 0;
        int prefixLength = (byte)flag & 0b0111;

        int length = (lengthCounted ? prefixLength + origin : origin) + addition;
        _ = length switch
        {
            1 => WriteByte((byte)length),
            2 => WriteUshort((ushort)length),
            4 => WriteUint((uint)length),
            _ => throw new InvalidDataException("Invalid Prefix is given")
        };

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BinaryPacket WriteBytes(ReadOnlySpan<byte> value)
    {
        _stream.Write(value);
        return this;
    }

    private BinaryPacket WritePacket(BinaryPacket value)
    {
        value._stream.Seek(0, SeekOrigin.Begin);
        value._stream.CopyTo(_stream);
        return this;
    }
    
    public BinaryPacket WriteBool(bool value)
    {
        _stream.WriteByte(value ? (byte)1 : (byte)0);
        return this;
    }
    
    public BinaryPacket WriteByte(byte value)
    {
        _stream.WriteByte(value);
        return this;
    }
    
    public BinaryPacket WriteUshort(ushort value)
    {
        value = BinaryPrimitives.ReverseEndianness(value);
        var buffer = new ReadOnlySpan<byte>((byte*)&value, sizeof(ushort));
        return WriteBytes(buffer);
    }

    public BinaryPacket WriteUint(uint value)
    {
        value = BinaryPrimitives.ReverseEndianness(value);
        var buffer = new ReadOnlySpan<byte>((byte*)&value, sizeof(uint));
        return WriteBytes(buffer);
    }

    public BinaryPacket WriteUlong(ulong value)
    {
        value = BinaryPrimitives.ReverseEndianness(value);
        var buffer = new ReadOnlySpan<byte>((byte*)&value, sizeof(ulong));
        return WriteBytes(buffer);
    }

    public BinaryPacket WriteSbyte(sbyte value)
    {
        var buffer = new ReadOnlySpan<byte>((byte*)&value, sizeof(sbyte));
        return WriteBytes(buffer);
    }

    public BinaryPacket WriteShort(short value)
    {
        value = BinaryPrimitives.ReverseEndianness(value);
        var buffer = new ReadOnlySpan<byte>((byte*)&value, sizeof(short));
        return WriteBytes(buffer);
    }

    public BinaryPacket WriteInt(int value)
    {
        value = BinaryPrimitives.ReverseEndianness(value);
        var buffer = new ReadOnlySpan<byte>((byte*)&value, sizeof(int));
        return WriteBytes(buffer);
    }

    public BinaryPacket WriteLong(ulong value)
    {
        value = BinaryPrimitives.ReverseEndianness(value);
        var buffer = new ReadOnlySpan<byte>((byte*)&value, sizeof(ulong));
        return WriteBytes(buffer);
    }

    public BinaryPacket Barrier(Action<BinaryPacket> writer, Prefix flag, int addition = 0)
    {
        var packet = new BinaryPacket();
        writer(packet);
        return WriteLength((int)packet.Length, flag, addition).WritePacket(packet);
    }
    
    public void Skip(int length) => _reader.BaseStream.Seek(length, SeekOrigin.Current);
    
    public bool IsAvailable(int length) => _stream.Length - _stream.Position >= length;

    public byte[] ToArray() => _stream.ToArray();


    public void Dispose()
    {
        _stream.Dispose();
        _reader.Dispose();
    }
}

[Flags]
public enum Prefix : byte
{
    Uint8 = 0b0001,
    Uint16 = 0b0010,
    Uint32 = 0b0100,
    LengthOnly = 0,
    WithPrefix = 0b1000,
}