using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace KonataNT.Proto.Generator;

internal interface IGeneratorContext
{
    CancellationToken CancellationToken { get; }
    void ReportDiagnostic(Diagnostic diagnostic);
    void AddSource(string hintName, string source);
    LanguageVersion LanguageVersion { get; }
    bool IsNet7OrGreater { get; }
    bool IsForUnity { get; }
}

internal static class GeneratorContextExtensions
{
    public static bool IsCSharp9OrGreater(this IGeneratorContext context)
    {
        return (int)context.LanguageVersion >= 900; // C# 9 == 900
    }

    public static bool IsCSharp10OrGreater(this IGeneratorContext context)
    {
        return (int)context.LanguageVersion >= 1000; // C# 10 == 1000
    }

    public static bool IsCSharp11OrGreater(this IGeneratorContext context)
    {
        return (int)context.LanguageVersion >= 1100; // C# 11 == 1100
    }
}

internal class GeneratorContext(SourceProductionContext context, LanguageVersion languageVersion, bool isNet70OrGreater) : IGeneratorContext
{
    public CancellationToken CancellationToken => context.CancellationToken;

    public LanguageVersion LanguageVersion { get; } = languageVersion;

    public bool IsNet7OrGreater { get; } = isNet70OrGreater;

    public bool IsForUnity => false;

    public void AddSource(string hintName, string source)
    {
        context.AddSource(hintName, source);
    }

    public void ReportDiagnostic(Diagnostic diagnostic)
    {
        context.ReportDiagnostic(diagnostic);
    }
}

class Comparer : IEqualityComparer<(TypeDeclarationSyntax, Compilation)>
{
    public static readonly Comparer Instance = new Comparer();

    public bool Equals((TypeDeclarationSyntax, Compilation) x, (TypeDeclarationSyntax, Compilation) y)
    {
        return x.Item1.Equals(y.Item1);
    }

    public int GetHashCode((TypeDeclarationSyntax, Compilation) obj)
    {
        return obj.Item1.GetHashCode();
    }
}