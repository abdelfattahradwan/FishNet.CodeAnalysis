using FishNet.CodeAnalysis.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace FishNet.CodeAnalysis.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class RemoteProcedureCallAnalyzer : DiagnosticAnalyzer
{
	private const string DiagnosticId = DiagnosticIds.FN0006;
	private const string Title = "Base calls are not allowed inside Remote procedure calls (ServerRpc, ObserversRpc, TargetRpc).";
	private const string MessageFormat = "Base calls are not allowed inside Remote procedure calls (ServerRpc, ObserversRpc, TargetRpc).";
	private const string Category = "Usage";

	private static readonly DiagnosticDescriptor Descriptor = new(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, true, customTags: WellKnownDiagnosticTags.NotConfigurable);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Descriptor);

	private const string FullyQualifiedServerRpcAttributeName = "global::FishNet.Object.ServerRpcAttribute";
	private const string FullyQualifiedObserversRpcAttributeName = "global::FishNet.Object.ObserversRpcAttribute";
	private const string FullyQualifiedTargetRpcAttributeName = "global::FishNet.Object.TargetRpcAttribute";

	public override void Initialize(AnalysisContext context)
	{
		context.EnableConcurrentExecution();

		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);

		context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration);
	}

	private static void Analyze(SyntaxNodeAnalysisContext context)
	{
		MethodDeclarationSyntax methodDeclarationSyntax = (MethodDeclarationSyntax)context.Node;

		IMethodSymbol methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclarationSyntax);

		if (!methodSymbol.IsOverride) return;

		IMethodSymbol overriddenMethodSymbol = methodSymbol.OverriddenMethod;

		if (!overriddenMethodSymbol.HasAttribute(FullyQualifiedServerRpcAttributeName, FullyQualifiedObserversRpcAttributeName, FullyQualifiedTargetRpcAttributeName)) return;

		if (methodDeclarationSyntax.Body is { } bodyBlockSyntax)
		{
			foreach (SyntaxNode syntaxNode in bodyBlockSyntax.DescendantNodes())
			{
				if (syntaxNode is not InvocationExpressionSyntax invocationExpressionSyntax) continue;

				if (invocationExpressionSyntax.Expression is not MemberAccessExpressionSyntax memberAccessExpressionSyntax) continue;

				if (memberAccessExpressionSyntax.Expression is not BaseExpressionSyntax) continue;

				IMethodSymbol invocationMethodSymbol = ((IMethodSymbol)context.SemanticModel.GetSymbol(invocationExpressionSyntax)).OriginalDefinition;

				if (!invocationMethodSymbol.HasAttribute(FullyQualifiedServerRpcAttributeName, FullyQualifiedObserversRpcAttributeName, FullyQualifiedTargetRpcAttributeName)) continue;

				context.ReportDiagnostic(Diagnostic.Create(Descriptor, invocationExpressionSyntax.GetLocation()));
			}
		}
		else if (methodDeclarationSyntax.ExpressionBody is { } expressionBodyArrowExpressionClauseSyntax)
		{
			foreach (SyntaxNode syntaxNode in expressionBodyArrowExpressionClauseSyntax.DescendantNodes())
			{
				if (syntaxNode is not InvocationExpressionSyntax invocationExpressionSyntax) continue;

				if (invocationExpressionSyntax.Expression is not MemberAccessExpressionSyntax memberAccessExpressionSyntax) continue;

				if (memberAccessExpressionSyntax.Expression is not BaseExpressionSyntax) continue;

				IMethodSymbol invocationMethodSymbol = ((IMethodSymbol)context.SemanticModel.GetSymbol(invocationExpressionSyntax)).OriginalDefinition;

				if (!invocationMethodSymbol.HasAttribute(FullyQualifiedServerRpcAttributeName, FullyQualifiedObserversRpcAttributeName, FullyQualifiedTargetRpcAttributeName)) continue;

				context.ReportDiagnostic(Diagnostic.Create(Descriptor, invocationExpressionSyntax.GetLocation()));
			}
		}
	}
}
