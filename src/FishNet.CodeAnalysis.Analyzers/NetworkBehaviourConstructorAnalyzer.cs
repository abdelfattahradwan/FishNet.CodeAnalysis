using FishNet.CodeAnalysis.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace FishNet.CodeAnalysis.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class NetworkBehaviourConstructorAnalyzer : DiagnosticAnalyzer
{
	private const string DiagnosticId1 = DiagnosticIds.FN0005;
	private const string Title1 = "NetworkBehaviour constructors are not allowed.";
	private const string MessageFormat1 = "NetworkBehaviour constructors are not allowed.";
	private const string Category1 = "Usage";

	private static readonly DiagnosticDescriptor Descriptor1 = new(DiagnosticId1, Title1, MessageFormat1, Category1, DiagnosticSeverity.Error, true, customTags: WellKnownDiagnosticTags.NotConfigurable);

	private const string FullyQualifiedNetworkBehaviourTypeName = "global::FishNet.Object.NetworkBehaviour";

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Descriptor1);

	public override void Initialize(AnalysisContext context)
	{
		context.EnableConcurrentExecution();

		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);

		context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ConstructorDeclaration);
	}

	private static void Analyze(SyntaxNodeAnalysisContext context)
	{
		if (context.Node.FirstAncestorOrSelf<BaseTypeDeclarationSyntax>()?.BaseList is not BaseListSyntax baseListSyntax) return;

		foreach (BaseTypeSyntax baseTypeSyntax in baseListSyntax.Types)
		{
			if (!context.SemanticModel.GetTypeSymbol(baseTypeSyntax.Type).IsSubtypeOf(FullyQualifiedNetworkBehaviourTypeName)) continue;

			context.ReportDiagnostic(Diagnostic.Create(Descriptor1, context.Node.GetLocation()));

			return;
		}
	}
}
