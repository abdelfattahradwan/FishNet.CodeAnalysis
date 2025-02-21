using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace FishNet.CodeAnalysis.Extensions;

internal static class ITypeSymbolExtensions
{
	public static IEnumerable<ITypeSymbol> EnumerateTypeHierarchy(this ITypeSymbol thisTypeSymbol)
	{
		for (ITypeSymbol typeSymbol = thisTypeSymbol; typeSymbol != null; typeSymbol = typeSymbol.BaseType)
		{
			yield return typeSymbol;
		}
	}

	public static bool IsSubtypeOf(this ITypeSymbol thisTypeSymbol, string fullyQualifiedTypeName)
	{
		return thisTypeSymbol.EnumerateTypeHierarchy().Any(typeSymbol => typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == fullyQualifiedTypeName);
	}

	public static bool IsSubtypeOf(this ITypeSymbol thisTypeSymbol, ImmutableHashSet<string> fullyQualifiedTypeNames, out ITypeSymbol? result)
	{
		result = null;

		foreach (ITypeSymbol typeSymbol in thisTypeSymbol.EnumerateTypeHierarchy())
		{
			if (!fullyQualifiedTypeNames.Contains(typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))) continue;

			result = typeSymbol;

			return true;
		}

		return false;
	}
}
