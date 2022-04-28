using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace FishNet.Analyzers
{
	internal static class Helpers
	{
		public static IEnumerable<ITypeSymbol> EnumerateTypeHierarchy(ITypeSymbol type)
		{
			ITypeSymbol current = type;

			while (current != null)
			{
				yield return current;

				current = current.BaseType;
			}
		}

		public static bool IsSubtypeOf(ITypeSymbol type, string fullyQualifiedSupertypeName)
		{
			foreach (ITypeSymbol supertype in EnumerateTypeHierarchy(type))
			{
				if (supertype.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Equals(fullyQualifiedSupertypeName, StringComparison.OrdinalIgnoreCase)) return true;
			}

			return false;
		}
	}
}
