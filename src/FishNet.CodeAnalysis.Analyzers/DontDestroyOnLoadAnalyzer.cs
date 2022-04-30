using FishNet.CodeAnalysis.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace FishNet.CodeAnalysis.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class DontDestroyOnLoadAnalyzer : DiagnosticAnalyzer
{
	private const string DiagnosticId1 = DiagnosticIds.FN0001;
	private const string Title1 = "Using DontDestroyOnLoad on a NetworkObject or a NetworkBehaviour isn't allowed.";
	private const string MessageFormat1 = "Using DontDestroyOnLoad on a {0} isn't allowed.";
	private const string Category1 = "Usage";

	private static readonly DiagnosticDescriptor Descriptor1 = new(DiagnosticId1, Title1, MessageFormat1, Category1, DiagnosticSeverity.Error, true, customTags: WellKnownDiagnosticTags.NotConfigurable);

	private const string DiagnosticId2 = DiagnosticIds.FN0002;
	private const string Title2 = "Don't call DontDestroyOnLoad from inside NetworkBehaviour or a class deriving from it.";
	private const string MessageFormat2 = "Don't call DontDestroyOnLoad from inside NetworkBehaviour or a class deriving from it.";
	private const string Category2 = "Usage";

	private static readonly DiagnosticDescriptor Descriptor2 = new(DiagnosticId2, Title2, MessageFormat2, Category2, DiagnosticSeverity.Error, true, customTags: WellKnownDiagnosticTags.NotConfigurable);

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

	private static void Analyze(SyntaxNodeAnalysisContext context)
	{
		InvocationExpressionSyntax invocationExpressionSyntax = (InvocationExpressionSyntax)context.Node;

		if (context.SemanticModel.GetSymbolInfo(invocationExpressionSyntax).Symbol is not IMethodSymbol method) return;

		string fullyQualifiedMethodName = $"{method.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}.{method.ContainingType.Name}.{method.Name}";

		if (fullyQualifiedMethodName != FullyQualifiedDontDestroyOnLoadMethodName) return;

		if (invocationExpressionSyntax.FirstAncestorOrSelf<BaseTypeDeclarationSyntax>()?.BaseList is not BaseListSyntax baseListSyntax) return;

		foreach (BaseTypeSyntax baseTypeSyntax in baseListSyntax.Types)
		{
			if (!context.SemanticModel.GetTypeSymbol(baseTypeSyntax.Type).IsSubtypeOf(FullyQualifiedNetworkBehaviourTypeName)) continue;

			context.ReportDiagnostic(Diagnostic.Create(Descriptor2, invocationExpressionSyntax.GetLocation()));

			return;
		}

		foreach (ArgumentSyntax argumentSyntax in invocationExpressionSyntax.ArgumentList.Arguments)
		{
			if (!context.SemanticModel.GetTypeSymbol(argumentSyntax.Expression).IsSubtypeOf(ImmutableHashSet.Create(FullyQualifiedNetworkObjectTypeName, FullyQualifiedNetworkBehaviourTypeName), out ITypeSymbol? typeSymbol)) continue;

			context.ReportDiagnostic(Diagnostic.Create(Descriptor1, argumentSyntax.GetLocation(), typeSymbol?.Name));

			return;
		}
	}
}
