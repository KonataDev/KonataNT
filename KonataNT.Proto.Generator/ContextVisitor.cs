using KonataNT.Proto.Generator.Meta;
using KonataNT.Proto.Generator.Utility;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace KonataNT.Proto.Generator;

/// <summary>
/// Visit every syntax node in the syntax tree and processing nested context.
/// </summary>
internal class ContextVisitor(IGeneratorContext context) : CSharpSyntaxWalker
{
    private const string Getter = "get;";
    private const string Setter = "set;";
    
    public List<ProtoMemberMeta> List { get; } = [];

    public override void Visit(SyntaxNode? node)
    {
        if (node == null) return;
        
        if (node is PropertyDeclarationSyntax property)
        {
            if (property.Modifiers.Any(SyntaxKind.StaticKeyword)) return;
            
            if (property.AccessorList?.Accessors is { } accessors && accessors.Count != 2)
            {
                var descriptor = DiagnosticDescriptors.MustHaveGetterAndSetter;
                context.ReportDiagnostic(Diagnostic.Create(descriptor, property.Identifier.GetLocation(), property.Identifier));
                return;
            }
            
            var attribute = property.GetAttributes().FirstOrDefault(a => a.Name.ToString() == "ProtoMember");
            if (attribute == null) return;

            var argument = attribute.ArgumentList?.Arguments[0] ?? throw new InvalidOperationException("ProtoMember attribute must have a tag argument.");
            int tag = int.Parse(argument.Expression.ToString());
            
            var meta = new ProtoMemberMeta
            {
                Name = property.Identifier.Text,
                Type = property.Type,
                Tag = tag,
                IsNested = property.Type.IsUserDefinedType()
            };
            
            List.Add(meta);
        }
        base.Visit(node);
    }
    
    private WireType GetPropertyVarIntType(TypeSyntax type)
    {
        if (type.IsUserDefinedType())
        {
            return WireType.LengthDelimited;
        }
        
        return type.ToString() switch
        {
            "int" => WireType.VarInt,
            "uint" => WireType.VarInt,
            "long" => WireType.VarInt,
            "ulong" => WireType.VarInt,
            "short" => WireType.VarInt,
            "ushort" => WireType.VarInt,
            "byte" => WireType.VarInt,
            "sbyte" => WireType.VarInt,
            "string" => WireType.LengthDelimited,
            "byte[]" => WireType.LengthDelimited,
            _ => throw new InvalidOperationException("Invalid type for VarInt.")
        };
    }
}