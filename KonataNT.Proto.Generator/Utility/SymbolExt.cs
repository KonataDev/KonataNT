using Microsoft.CodeAnalysis;

namespace KonataNT.Proto.Generator.Utility;

public static class SymbolExt
{
    public static bool ContainsAttribute(this ISymbol symbol, INamedTypeSymbol attribute)
    {
        return symbol.GetAttributes().Any(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, attribute));
    }

    public static AttributeData? GetAttribute(this ISymbol symbol, INamedTypeSymbol attribute)
    {
        return symbol.GetAttributes().FirstOrDefault(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, attribute));
    }

    public static AttributeData? GetImplAttribute(this ISymbol symbol, INamedTypeSymbol implAttribute)
    {
        return symbol.GetAttributes().FirstOrDefault(x =>
        {
            if (x.AttributeClass == null) return false;
            if (x.AttributeClass.EqualsUnconstructedGenericType(implAttribute)) return true;

            foreach (var item in x.AttributeClass.GetAllBaseTypes())
            {
                if (item.EqualsUnconstructedGenericType(implAttribute))
                {
                    return true;
                }
            }
            return false;
        });
    }

    public static IEnumerable<ISymbol> GetAllMembers(this INamedTypeSymbol symbol, bool withoutOverride = true)
    {
        // Iterate Parent -> Derived
        if (symbol.BaseType != null)
        {
            foreach (var item in GetAllMembers(symbol.BaseType))
            {
                // override item already iterated in parent type
                if (!withoutOverride || !item.IsOverride)
                {
                    yield return item;
                }
            }
        }

        foreach (var item in symbol.GetMembers())
        {
            if (!withoutOverride || !item.IsOverride)
            {
                yield return item;
            }
        }
    }

    public static IEnumerable<ISymbol> GetParentMembers(this INamedTypeSymbol symbol)
    {
        // Iterate Parent -> Derived
        if (symbol.BaseType != null)
        {
            foreach (var item in GetAllMembers(symbol.BaseType))
            {
                // override item already iterated in parent type
                if (!item.IsOverride)
                {
                    yield return item;
                }
            }
        }
    }
    
    public static bool EqualsUnconstructedGenericType(this INamedTypeSymbol left, INamedTypeSymbol right)
    {
        var l = left.IsGenericType ? left.ConstructUnboundGenericType() : left;
        var r = right.IsGenericType ? right.ConstructUnboundGenericType() : right;
        return SymbolEqualityComparer.Default.Equals(l, r);
    }

    public static IEnumerable<INamedTypeSymbol> GetAllBaseTypes(this INamedTypeSymbol symbol)
    {
        var t = symbol.BaseType;
        while (t != null)
        {
            yield return t;
            t = t.BaseType;
        }
    }
}