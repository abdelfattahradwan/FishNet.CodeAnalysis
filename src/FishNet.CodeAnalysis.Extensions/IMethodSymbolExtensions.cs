using Microsoft.CodeAnalysis;

namespace FishNet.CodeAnalysis.Extensions;

internal static class IMethodSymbolExtensions
{
	public static bool HasAttribute(this IMethodSymbol thisMethodSymbol, string fullyQualifiedAttributeName)
	{
		foreach (AttributeData attributeData in thisMethodSymbol.GetAttributes())
		{
			if (attributeData.AttributeClass.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == fullyQualifiedAttributeName) return true;
		}

		return false;
	}

	public static bool HasAttribute(this IMethodSymbol thisMethodSymbol, params string[] fullyQualifiedAttributeNames)
	{
		foreach (string fullyQualifiedAttributeName in fullyQualifiedAttributeNames)
		{
			if (thisMethodSymbol.HasAttribute(fullyQualifiedAttributeName)) return true;
		}

		return false;
	}

	public static bool HasAttributes(this IMethodSymbol thisMethodSymbol, string[] fullyQualifiedAttributeNames)
	{
		foreach (string fullyQualifiedAttributeName in fullyQualifiedAttributeNames)
		{
			if (!thisMethodSymbol.HasAttribute(fullyQualifiedAttributeName)) return false;
		}

		return true;
	}

	public static AttributeData? GetAttribute(this IMethodSymbol thisMethodSymbol, string fullyQualifiedAttributeName)
	{
		foreach (AttributeData attributeData in thisMethodSymbol.GetAttributes())
		{
			if (attributeData.AttributeClass.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == fullyQualifiedAttributeName) return attributeData;
		}

		return null;
	}
}