using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using KonataNT.Proto.Exceptions;

namespace KonataNT.Proto.Serialization;

public ref struct ProtoReader
{
    private readonly ReadOnlySpan<byte> _span;

    private uint _position;
    
    public bool EndOfStream => _position == Length;

    private readonly ref byte First => ref MemoryMarshal.GetReference(_span);

    private readonly uint Length => (uint)_span.Length;

    public ProtoReader(byte[] buffer) : this(buffer, 0, buffer.Length)
    {

    }

    public ProtoReader(byte[] buffer, int offset, int length) : this(buffer.AsSpan(offset, length))
    {
        
    }

    public ProtoReader(ReadOnlySpan<byte> span)
    {
        _span = span;
        _position = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe T ReadVarInt<T>() where T : unmanaged, INumber<T>, IBitwiseOperators<T, T, T>
    {
        nuint position = _position;
        if (Length - (uint)position >= (uint)(sizeof(T) + 1 + (sizeof(T) >> 3)))
        {
            ref byte ptr = ref First;
            T value = T.CreateTruncating(Unsafe.Add(ref ptr, position));
            if (value < T.CreateTruncating(0x80u))
            {
                position = (uint)position + 1;
                goto succeeded;
            }
            if (sizeof(T) == 1)
            {
                position = (uint)position + 2;
                goto succeeded;
            }
            value &= T.CreateTruncating(0x7Fu);
            int shift = 7;
            do
            {
                position++;
                byte b = Unsafe.Add(ref ptr, position);
                value += T.CreateTruncating((ulong)(uint)(b & 0x7F) << shift);
                if (b < 0x80)
                {
                    goto succeeded;
                }
                shift += 7;
            }
            while (shift < (sizeof(T) << 3));
            ThrowMalformedMessage();
        succeeded:
            _position = (uint)position;
            return value;
        }
        return ReadVarIntSlowPath<T>();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private unsafe T ReadVarIntSlowPath<T>() where T : unmanaged, INumber<T>
    {
        int shift = 0;
        T result = T.Zero;
        do
        {
            byte b = ReadRawInteger<byte>();
            result += T.CreateTruncating((ulong)((uint)b & 0x7F) << shift);
            if (b < 0x80)
            {
                return result;
            }
            shift += 7;
        }
        while (shift < sizeof(T) << 3);
        ThrowMalformedMessage();
        return result;
    }

    public T ReadRawInteger<T>() where T : unmanaged, INumber<T>
    {
        ref T ptr = ref Unsafe.As<byte, T>(ref First);
        uint position = _position;
        if (position == Length)
        {
            ThrowMalformedMessage();
        }
        return ReadRawIntegerUnchecked(ref ptr, position);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private T ReadRawIntegerUnchecked<T>() where T : unmanaged, INumber<T>
    {
        ref T ptr = ref Unsafe.As<byte, T>(ref First);
        nuint position = _position;
        return ReadRawIntegerUnchecked(ref ptr, position);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe T ReadRawIntegerUnchecked<T>(ref T ptr, nuint position) where T : unmanaged, INumber<T>
    {
        T result = Unsafe.AddByteOffset(ref ptr, position);
        Volatile.Write(ref _position, (uint)position + (uint)sizeof(T));
        return result;
    }

    public ReadOnlySpan<byte> ReadRawBytesSpan(uint length)
    {
        if (Length - _position < length)
        {
            ThrowMalformedMessage();
        }
        var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.Add(ref First, _position), (int)length);
        _position += length;
        return span;
    }

    public byte[] ReadRawBytes(uint length)
    {
        return ReadRawBytesSpan(length).ToArray();
    }

    public uint ReadTag()
    {
        return ReadVarInt<uint>() >> 3;
    }

    [DoesNotReturn]
    public static void ThrowMalformedMessage()
    {
        throw new MalformedProtoMessageException();
    }
}
