using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace FishNet.CodeAnalysis.Extensions;

internal static class ITypeSymbolExtensions
{
	public static IEnumerable<ITypeSymbol> EnumerateTypeHierarchy(this ITypeSymbol type)
	{
		ITypeSymbol current = type;

		while (current != null)
		{
			yield return current;

			current = current.BaseType;
		}
	}

	public static bool IsSubtypeOf(this ITypeSymbol type, string fullyQualifiedSupertypeName)
	{
		foreach (ITypeSymbol supertype in type.EnumerateTypeHierarchy())
		{
			if (supertype.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Equals(fullyQualifiedSupertypeName, StringComparison.OrdinalIgnoreCase)) return true;
		}

		return false;
	}
}
