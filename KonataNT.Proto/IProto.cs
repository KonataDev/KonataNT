namespace KonataNT.Proto;

public interface IProto
{
    public byte[] Serialize();

    public static abstract IProto Deserialize(byte[] data);
}