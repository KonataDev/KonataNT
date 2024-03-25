using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace KonataNT.Proto.Generator.Meta;

public class ProtoMemberMeta
{
    public int Tag { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public TypeSyntax Type { get; set; } = TypeSyntaxFactory.Object;
    
    public bool IsNested { get; set; }
    
    public bool IsEnumerable { get; set; }
    
    public bool IsNullable { get; set; }
    
    public bool IsValueType { get; set; }
    
    public WireType WireType { get; set; }
}

public enum WireType
{
    Invalid = -1,
    VarInt = 0,
    Fixed64 = 1,
    Fixed32 = 5,
    LengthDelimited = 2
}