using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace FishNet.CodeAnalysis.Extensions;

internal static class AttributeDataExtensions
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T? GetConstructorArgument<T>(this AttributeData thisAttributeData, int argumentIndex)
	{
		ImmutableArray<TypedConstant> constructorArguments = thisAttributeData.ConstructorArguments;

		if (argumentIndex > -1 && argumentIndex < constructorArguments.Length) return (T)constructorArguments[argumentIndex].Value;

		return default(T?);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T? GetNamedArgument<T>(this AttributeData thisAttributeData, int argumentIndex)
	{
		ImmutableArray<KeyValuePair<string, TypedConstant>> namedArguments = thisAttributeData.NamedArguments;

		if (argumentIndex > -1 && argumentIndex < namedArguments.Length) return (T)namedArguments[argumentIndex].Value.Value;

		return default(T?);
	}

	public static T? GetNamedArgument<T>(this AttributeData thisAttributeData, string argumentName)
	{
		foreach (KeyValuePair<string, TypedConstant> namedArgument in thisAttributeData.NamedArguments)
		{
			if (namedArgument.Key == argumentName) return (T)namedArgument.Value.Value;
		}

		return default(T?);
	}
}
