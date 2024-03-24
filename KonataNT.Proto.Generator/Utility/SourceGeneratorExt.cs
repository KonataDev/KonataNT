using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace KonataNT.Proto.Generator.Utility;

public static class SourceGeneratorExt
{
    public static bool IsPartial(this TypeDeclarationSyntax typeDeclaration) => 
        typeDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));

    public static string GetVisibility(this TypeDeclarationSyntax typeDeclaration)
    {
        var modifiers = typeDeclaration.Modifiers;
        
        if (modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword))) return "public";
        if (modifiers.Any(m => m.IsKind(SyntaxKind.InternalKeyword))) return "internal";
        if (modifiers.Any(m => m.IsKind(SyntaxKind.ProtectedKeyword))) return "protected";
        return modifiers.Any(m => m.IsKind(SyntaxKind.PrivateKeyword)) ? "private" : "internal";
    }
    
    public static bool IsUserDefinedType(this TypeSyntax typeSyntax) => 
        typeSyntax is IdentifierNameSyntax or GenericNameSyntax or QualifiedNameSyntax;
    
    
    public static IEnumerable<AttributeSyntax> GetAttributes(this SyntaxNode syntaxNode) => 
        syntaxNode.DescendantNodes().OfType<AttributeSyntax>();
}