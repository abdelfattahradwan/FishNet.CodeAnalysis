using Microsoft.CodeAnalysis;

namespace FishNet.Analyzers
{
	internal static class Extensions
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
}
