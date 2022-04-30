using Microsoft.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace FishNet.CodeAnalysis.Extensions;

internal static class SemanticModelExtensions
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ISymbol GetSymbol(this SemanticModel semanticModel, SyntaxNode node)
	{
		return semanticModel.GetSymbolInfo(node).Symbol;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ITypeSymbol GetTypeSymbol(this SemanticModel semanticModel, SyntaxNode node)
	{
		return semanticModel.GetTypeInfo(node).Type;
	}
}
