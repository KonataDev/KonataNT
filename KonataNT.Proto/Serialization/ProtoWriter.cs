using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace KonataNT.Proto.Serialization;

public ref struct ProtoWriter
{
    private const int BufferSize = 1024;

    private readonly Span<byte> _span;

    private readonly Stream? _underlyingStream;

    private uint _position;

    private readonly ref byte First => ref MemoryMarshal.GetReference(_span);

    private readonly uint Length => (uint)_span.Length;

    public ProtoWriter(Stream? stream) : this(stream, BufferSize) { }

    public ProtoWriter(Stream? stream, int bufferSize) : this(GC.AllocateUninitializedArray<byte>(bufferSize), 0, bufferSize, stream) { }

    public ProtoWriter(byte[] buffer) : this(buffer, 0, buffer.Length) { }

    public ProtoWriter(byte[] buffer, int offset, int length) : this(buffer, offset, length, null) { }

    private ProtoWriter(byte[] buffer, int offset, int length, Stream? underlyingStream)
    {
        _span = new Span<byte>(buffer, offset, length);
        _underlyingStream = underlyingStream;
        _position = 0;
    }

    public void WriteHead(WireType type, uint tag)
    {
        uint head = (tag << 3) + (uint)type;
        WriteVarInt(head);
    }

    public void WriteBytes(ReadOnlySpan<byte> value)
    {
        if (_position < Length)
        {
            var dst = MemoryMarshal.CreateSpan(ref Unsafe.Add(ref First, _position), value.Length);
            value.CopyTo(dst);
        }
        else
        {
            Flush();
            _underlyingStream!.Write(value);
        }
        _position += (uint)value.Length;
    }

    public unsafe void WriteVarInt(ulong value)
    {
        if (Length - _position >= 10)
        {
            if (value < 0x80)
            {
                WriteRawByteUnchecked((byte)value);
                return;
            }
            WriteVarIntUnchecked(value);
            return;
        }
        WriteVarIntSlowPath(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteVarIntUnchecked(ulong value)
    {
        ref byte first = ref First;
        nuint position = _position;
        while (true)
        {
            if (value < 0x80)
            {
                Unsafe.Add(ref first, position) = (byte)value;
                position++;
                break;
            }
            Unsafe.Add(ref first, position) = (byte)((uint)value | 0xFFFFFF80);
            position++;
            value >>= 7;
        }
        _position = (uint)position;
    }

    public unsafe void WriteVarInt(long value) => WriteVarInt((ulong)value);
    
    public unsafe void WriteVarInt(bool value) => WriteVarInt(value ? 1UL : 0UL);

    public unsafe void WriteLengthDelimited(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        WriteLengthDelimited(bytes.AsSpan());
    }

    public unsafe void WriteLengthDelimited(Span<byte> value)
    {
        int length = value.Length;
        WriteVarInt((uint)length);
        WriteBytes(value);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private unsafe void WriteVarIntSlowPath(ulong value)
    {
        while (value > 127)
        {
            WriteRawByte((byte)((value & 0x7F) | 0x80));
            value >>= 7;
        }
        WriteRawByte((byte)value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteRawByte(byte value)
    {
        if (_position == Length)
        {
            Flush();
        }
        WriteRawByteUnchecked(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteRawByteUnchecked(byte value)
    {
        Unsafe.Add(ref First, _position) = value;
        _position++;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public void Flush()
    {
        if (_underlyingStream == null)
        {
            throw new InvalidOperationException();
        }
        _underlyingStream.Write(MemoryMarshal.CreateReadOnlySpan(ref First, (int)_position));
        _position = 0;
    }

    public void Dispose()
    {
        _underlyingStream?.Dispose();
    }
}
