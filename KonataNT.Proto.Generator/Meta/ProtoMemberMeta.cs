using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace KonataNT.Proto.Generator.Meta;

public class ProtoMemberMeta
{
    public int Tag { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public TypeSyntax Type { get; set; } = TypeSyntaxFactory.Object;
    
    public bool IsNested { get; set; }
    
    public WireType WireType { get; set; }
}

public enum WireType
{
    VarInt = 0,
    Fixed64 = 1,
    Fixed32 = 5,
    LengthDelimited = 2
}