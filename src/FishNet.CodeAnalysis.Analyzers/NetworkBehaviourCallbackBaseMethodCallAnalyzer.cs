using FishNet.CodeAnalysis.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace FishNet.CodeAnalysis.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class NetworkBehaviourCallbackBaseMethodCallAnalyzer : DiagnosticAnalyzer
{
	private const string DiagnosticId1 = "FN0003";
	private const string Title1 = "NetworkBehaviour callback missing base method call.";
	private const string MessageFormat1 = "base.{0} call is missing.";
	private const string Category1 = "Usage";

	private static readonly DiagnosticDescriptor Descriptor1 = new(DiagnosticId1, Title1, MessageFormat1, Category1, DiagnosticSeverity.Error, true, customTags: new string[]
	{
		WellKnownDiagnosticTags.NotConfigurable,
	});

	private const string DiagnosticId2 = "FN0004";
	private const string Title2 = "Base method call must be the first statement.";
	private const string MessageFormat2 = "base.{0} call must be the first statement.";
	private const string Category2 = "Usage";

	private static readonly DiagnosticDescriptor Descriptor2 = new(DiagnosticId2, Title2, MessageFormat2, Category2, DiagnosticSeverity.Error, true, customTags: new string[]
	{
		WellKnownDiagnosticTags.NotConfigurable,
	});

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Descriptor1, Descriptor2);

	private const string FullyQualifiedNetworkBehaviourTypeName = "global::FishNet.Object.NetworkBehaviour";

	private readonly ImmutableHashSet<string> NetworkBehaviourCallbackMethodNames = ImmutableHashSet.Create(new string[]
	{
		"OnStartNetwork",
		"OnStartServer",
		"OnStartClient",
		"OnOwnershipServer",
		"OnOwnershipClient",
		"OnStopClient",
		"OnStopServer",
		"OnStopNetwork",
	});

	public override void Initialize(AnalysisContext context)
	{
		context.EnableConcurrentExecution();

		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);

		context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration);
	}

	private void Analyze(SyntaxNodeAnalysisContext analysisContext)
	{
		MethodDeclarationSyntax method = (MethodDeclarationSyntax)analysisContext.Node;

		ClassDeclarationSyntax @class = method.FirstAncestorOrSelf<ClassDeclarationSyntax>();

		bool isNetworkBehaviour = false;

		foreach (BaseTypeSyntax baseType in @class.BaseList.Types)
		{
			if (!analysisContext.SemanticModel.GetTypeSymbol(baseType.Type).IsSubtypeOf(FullyQualifiedNetworkBehaviourTypeName)) continue;

			isNetworkBehaviour = true;

			break;
		}

		if (!isNetworkBehaviour) return;

		IMethodSymbol methodSymbol = analysisContext.SemanticModel.GetDeclaredSymbol(method);

		if (!methodSymbol.IsOverride || !NetworkBehaviourCallbackMethodNames.Contains(methodSymbol.Name)) return;

		IMethodSymbol baseMethodSymbol = methodSymbol.OverriddenMethod;

		bool isBaseCalled = false;

		for (int i = 0; i < method.Body.Statements.Count; i++)
		{
			if (method.Body.Statements[i] is not ExpressionStatementSyntax statement) continue;

			if (statement.Expression is not InvocationExpressionSyntax invocation) continue;

			if (analysisContext.SemanticModel.GetSymbol(invocation) is not IMethodSymbol invocationSymbol) continue;

			if (invocationSymbol.OriginalDefinition != baseMethodSymbol) continue;

			isBaseCalled = true;

			if (i > 0)
			{
				analysisContext.ReportDiagnostic(Diagnostic.Create(Descriptor2, Location.Create(invocation.SyntaxTree, invocation.Span), baseMethodSymbol.Name));

				return;
			}
		}

		if (!isBaseCalled) analysisContext.ReportDiagnostic(Diagnostic.Create(Descriptor1, Location.Create(method.SyntaxTree, method.Identifier.Span), methodSymbol.Name));
	}
}
