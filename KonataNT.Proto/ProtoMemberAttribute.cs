namespace KonataNT.Proto;

[AttributeUsage(AttributeTargets.Property)]
public class ProtoMemberAttribute(int tag) : Attribute
{
    public int Tag { get; } = tag;
}