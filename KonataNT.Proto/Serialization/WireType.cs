namespace KonataNT.Proto.Serialization;

public enum WireType : byte
{
    VarInt = 0,
    FixedInt64 = 1,
    LengthDelimited = 2,
    FixedInt32 = 5
}