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
}