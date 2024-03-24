using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace KonataNT.Proto.Generator.Meta;

public static class TypeSyntaxFactory
{
    public static TypeSyntax Object => SyntaxFactory.ParseTypeName("object");
}