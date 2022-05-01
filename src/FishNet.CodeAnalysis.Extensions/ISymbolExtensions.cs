using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace FishNet.CodeAnalysis.Extensions;

internal static class ISymbolExtensions
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string GetFullyQualifiedName(this ISymbol thisSymbol)
	{
		return thisSymbol.ContainingType is null ? $"global::{thisSymbol.Name}" : $"{thisSymbol.ContainingType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}.{thisSymbol.Name}";
	}

	public static bool HasAttribute(this ISymbol thisSymbol, string fullyQualifiedAttributeName)
	{
		foreach (AttributeData attribute in thisSymbol.GetAttributes())
		{
			if (attribute.AttributeClass.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == fullyQualifiedAttributeName) return true;
		}

		return false;
	}

	public static bool HasAttribute(this ISymbol thisSymbol, params string[] fullyQualifiedAttributeNames)
	{
		foreach (string fullyQualifiedAttributeName in fullyQualifiedAttributeNames)
		{
			if (thisSymbol.HasAttribute(fullyQualifiedAttributeName)) return true;
		}

		return false;
	}

	public static bool HasAttributes(this ISymbol thisSymbol, string[] fullyQualifiedAttributeNames)
	{
		foreach (string fullyQualifiedAttributeName in fullyQualifiedAttributeNames)
		{
			if (!thisSymbol.HasAttribute(fullyQualifiedAttributeName)) return false;
		}

		return true;
	}

	public static AttributeData? GetAttribute(this ISymbol thisSymbol, string fullyQualifiedAttributeName)
	{
		foreach (AttributeData attribute in thisSymbol.GetAttributes())
		{
			if (attribute.AttributeClass.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == fullyQualifiedAttributeName) return attribute;
		}

		return null;
	}

	public static ImmutableArray<AttributeData> GetAttributes(this ISymbol thisSymbol, string fullyQualifiedAttributeName)
	{
		ImmutableArray<AttributeData>.Builder attributes = ImmutableArray.CreateBuilder<AttributeData>();

		foreach (AttributeData attribute in thisSymbol.GetAttributes())
		{
			if (attribute.AttributeClass.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == fullyQualifiedAttributeName) attributes.Add(attribute);
		}

		return attributes.ToImmutable();
	}
}