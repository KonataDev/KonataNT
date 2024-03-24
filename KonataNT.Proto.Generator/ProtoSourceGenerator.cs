using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace KonataNT.Proto.Generator;

[Generator(LanguageNames.CSharp)]
public partial class ProtoSourceGenerator : IIncrementalGenerator
{
    private const string ProtoContractAttributeFullName = "KonataNT.Proto.ProtoContractAttribute";
    
    private static void RegisterProtoContract(IncrementalGeneratorInitializationContext context)
    {
        var logProvider = context.AnalyzerConfigOptionsProvider
            .Select((configOptions, _) =>
            {
                if (configOptions.GlobalOptions.TryGetValue("build_property.ProtoGenerator_SerializationInfoOutputDirectory", out string? path)) return path;

                return null;
            });
        
        var typeDeclarations = context.SyntaxProvider.ForAttributeWithMetadataName(
            ProtoContractAttributeFullName,
            predicate: static (node, _) => node is ClassDeclarationSyntax or StructDeclarationSyntax,
            transform: static (context, _) => (TypeDeclarationSyntax)context.TargetNode);
        
        var parseOptions = context.ParseOptionsProvider.Select((parseOptions, _) =>
        {
            var csOptions = (CSharpParseOptions)parseOptions;
            var langVersion = csOptions.LanguageVersion;
            bool net7 = csOptions.PreprocessorSymbolNames.Contains("NET7_0_OR_GREATER");
            return (langVersion, net7);
        });
        
        var source = typeDeclarations
                .Combine(context.CompilationProvider)
                .WithComparer(Comparer.Instance)
                .Combine(logProvider)
                .Combine(parseOptions);

        context.RegisterSourceOutput(source, static (context, source) =>
        {
            var (typeDeclaration, compilation) = source.Left.Item1;
            string? logPath = source.Left.Item2;
            var (langVersion, net7) = source.Right;

            Generate(typeDeclaration, compilation, logPath, new GeneratorContext(context, langVersion, net7));
        });
    }
    
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        RegisterProtoContract(context);
    }
}