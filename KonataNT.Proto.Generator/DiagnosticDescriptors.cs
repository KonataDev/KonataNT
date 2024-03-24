using Microsoft.CodeAnalysis;

namespace KonataNT.Proto.Generator;

public class DiagnosticDescriptors
{
    private const string Category = "GenerateProto";
    
    public static readonly DiagnosticDescriptor MustBePartial = new(
        id: "PROTO001",
        title: "ProtoContract object must be partial",
        messageFormat: "The ProtoContract object '{0}' must be partial",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
    
    public static readonly DiagnosticDescriptor MustHaveGetterAndSetter = new(
        id: "PROTO002",
        title: "ProtoMember property must have both getters and setters",
        messageFormat: "The ProtoMember property '{0}' must have both public getters and setters",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}