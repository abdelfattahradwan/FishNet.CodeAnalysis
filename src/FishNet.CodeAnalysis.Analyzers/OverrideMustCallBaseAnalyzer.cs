using FishNet.CodeAnalysis.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using FishNet.CodeAnalysis.Annotations;

namespace FishNet.CodeAnalysis.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class OverrideMustCallBaseAnalyzer : DiagnosticAnalyzer
{
	private const string DiagnosticId1 = DiagnosticIds.FN0003;
	private const string Title1 = "Override must call base.";
	private const string MessageFormat1 = "Missing base.{0} call.";
	private const string Category1 = "Usage";

	private static readonly DiagnosticDescriptor Descriptor1 = new(DiagnosticId1, Title1, MessageFormat1, Category1, DiagnosticSeverity.Error, true, customTags: WellKnownDiagnosticTags.NotConfigurable);

	private const string DiagnosticId2 = DiagnosticIds.FN0004;
	private const string Title2 = "Base call must be the first statement.";
	private const string MessageFormat2 = "base.{0} call must be the first statement.";
	private const string Category2 = "Usage";

	private static readonly DiagnosticDescriptor Descriptor2 = new(DiagnosticId2, Title2, MessageFormat2, Category2, DiagnosticSeverity.Error, true, customTags: WellKnownDiagnosticTags.NotConfigurable);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Descriptor1, Descriptor2);

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

		string fullyQualifiedAttributeName = typeof(OverrideMustCallBaseAttribute).GetGlobalPrefixedFullName();

		if (overriddenMethodSymbol.GetAttribute(fullyQualifiedAttributeName) is not AttributeData overrideMustCallBaseAttributeData) return;

		bool baseCallMustBeFirst = overrideMustCallBaseAttributeData.GetNamedArgument<bool>(0);

		bool isBaseCalled = false;

		if (methodDeclarationSyntax.Body is BlockSyntax bodyBlockSyntax)
		{
			for (int i = 0; i < bodyBlockSyntax.Statements.Count; i++)
			{
				if (bodyBlockSyntax.Statements[i] is not ExpressionStatementSyntax expressionStatementSyntax) continue;

				if (expressionStatementSyntax.Expression is not InvocationExpressionSyntax invocationExpressionSyntax) continue;

				if (context.SemanticModel.GetSymbol(invocationExpressionSyntax) is not IMethodSymbol invocationMethodSymbol) continue;

				if (invocationMethodSymbol.OriginalDefinition != overriddenMethodSymbol) continue;

				isBaseCalled = true;

				if (baseCallMustBeFirst && i > 0)
				{
					context.ReportDiagnostic(Diagnostic.Create(Descriptor2, invocationExpressionSyntax.GetLocation(), overriddenMethodSymbol.Name));

					return;
				}
			}
		}
		else if (methodDeclarationSyntax.ExpressionBody is ArrowExpressionClauseSyntax expressionBodyArrowExpressionClauseSyntax)
		{
			isBaseCalled = expressionBodyArrowExpressionClauseSyntax.Expression is InvocationExpressionSyntax invocationExpressionSyntax
				&& context.SemanticModel.GetSymbol(invocationExpressionSyntax) is IMethodSymbol invocationMethodSymbol
				&& invocationMethodSymbol.OriginalDefinition == overriddenMethodSymbol;
		}

		if (!isBaseCalled) context.ReportDiagnostic(Diagnostic.Create(Descriptor1, methodDeclarationSyntax.GetLocation(), methodSymbol.Name));
	}
}
