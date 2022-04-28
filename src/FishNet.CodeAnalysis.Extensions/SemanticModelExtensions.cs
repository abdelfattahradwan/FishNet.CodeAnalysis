using Microsoft.CodeAnalysis;

namespace FishNet.CodeAnalysis.Extensions;

internal static class SemanticModelExtensions
{
	public static ISymbol GetSymbol(this SemanticModel semanticModel, SyntaxNode node)
	{
		return semanticModel.GetSymbolInfo(node).Symbol;
	}

	public static ITypeSymbol GetTypeSymbol(this SemanticModel semanticModel, SyntaxNode node)
	{
		return semanticModel.GetTypeInfo(node).Type;
	}
}
