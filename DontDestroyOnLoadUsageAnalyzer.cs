using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;

namespace FishNet.Analyzers
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	internal sealed class DontDestroyOnLoadUsageAnalyzer : DiagnosticAnalyzer
	{
		private const string DiagnosticId1 = "FN0001";
		private const string Title1 = "Using DontDestroyOnLoad on a NetworkObject or a NetworkBehaviour isn't allowed.";
		private const string MessageFormat1 = "Using DontDestroyOnLoad on a {0} isn't allowed.";
		private const string Category1 = "Usage";

		private static readonly DiagnosticDescriptor Descriptor1 = new(DiagnosticId1, Title1, MessageFormat1, Category1, DiagnosticSeverity.Error, true, customTags: new string[]
		{
			WellKnownDiagnosticTags.NotConfigurable,
		});

		private const string DiagnosticId2 = "FN0002";
		private const string Title2 = "Don't call DontDestroyOnLoad from inside NetworkBehaviour or a class deriving from it.";
		private const string MessageFormat2 = "Don't call DontDestroyOnLoad from inside NetworkBehaviour or a class deriving from it.";
		private const string Category2 = "Usage";

		private static readonly DiagnosticDescriptor Descriptor2 = new(DiagnosticId2, Title2, MessageFormat2, Category2, DiagnosticSeverity.Error, true, customTags: new string[]
		{
			WellKnownDiagnosticTags.NotConfigurable,
		});

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Descriptor1, Descriptor2);

		private const string FullyQualifiedDontDestroyOnLoadMethodName = "global::UnityEngine.Object.DontDestroyOnLoad";
		private const string FullyQualifiedNetworkObjectTypeName = "global::FishNet.Object.NetworkObject";
		private const string FullyQualifiedNetworkBehaviourTypeName = "global::FishNet.Object.NetworkBehaviour";

		public override void Initialize(AnalysisContext context)
		{
			context.EnableConcurrentExecution();

			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);

			context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.InvocationExpression);
		}

		private void Analyze(SyntaxNodeAnalysisContext analysisContext)
		{
			InvocationExpressionSyntax invocation = (InvocationExpressionSyntax)analysisContext.Node;

			if (analysisContext.SemanticModel.GetSymbolInfo(invocation).Symbol is not IMethodSymbol method) return;

			string fullyQualifiedMethodName = $"{method.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}.{method.ContainingType.Name}.{method.Name}";

			if (fullyQualifiedMethodName.Equals(FullyQualifiedDontDestroyOnLoadMethodName, StringComparison.InvariantCultureIgnoreCase))
			{
				ClassDeclarationSyntax @class = invocation.FirstAncestorOrSelf<ClassDeclarationSyntax>();

				foreach (BaseTypeSyntax baseType in @class.BaseList.Types)
				{
					if (Helpers.IsSubtypeOf(analysisContext.SemanticModel.GetTypeSymbol(baseType.Type), FullyQualifiedNetworkBehaviourTypeName))
					{
						Diagnostic diagnostic = Diagnostic.Create(Descriptor2, Location.Create(analysisContext.Node.SyntaxTree, invocation.Span));

						analysisContext.ReportDiagnostic(diagnostic);

						return;
					}
				}

				foreach (ArgumentSyntax argument in invocation.ArgumentList.Arguments)
				{
					foreach (ITypeSymbol supertype in Helpers.EnumerateTypeHierarchy(analysisContext.SemanticModel.GetTypeSymbol(argument.Expression)))
					{
						string fullyQualifiedSupertypeName = supertype.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

						if (fullyQualifiedSupertypeName.Equals(FullyQualifiedNetworkObjectTypeName, StringComparison.InvariantCultureIgnoreCase))
						{
							Diagnostic diagnostic = Diagnostic.Create(Descriptor1, Location.Create(analysisContext.Node.SyntaxTree, argument.Span), "NetworkObject");

							analysisContext.ReportDiagnostic(diagnostic);

							return;
						}
						else if (fullyQualifiedSupertypeName.Equals(FullyQualifiedNetworkBehaviourTypeName, StringComparison.InvariantCultureIgnoreCase))
						{
							Diagnostic diagnostic = Diagnostic.Create(Descriptor1, Location.Create(analysisContext.Node.SyntaxTree, argument.Span), "NetworkBehaviour");

							analysisContext.ReportDiagnostic(diagnostic);

							return;
						}
					}
				}
			}
		}
	}
}
