using FishNet.CodeAnalysis.Annotations;
using FishNet.CodeAnalysis.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace FishNet.CodeAnalysis.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class PreventUsageInsideAnalyzer : DiagnosticAnalyzer
{
	private const string DiagnosticId = DiagnosticIds.FN0007;
	private const string Title = "Usage of member is not allowed here.";
	private const string MessageFormat = "Usage of {0} is not allowed inside {1}. {2}";
	private const string Category = "Usage";

	private static readonly DiagnosticDescriptor Descriptor = new(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, true, customTags: WellKnownDiagnosticTags.NotConfigurable);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Descriptor);

	public override void Initialize(AnalysisContext context)
	{
		context.EnableConcurrentExecution();

		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);

		SyntaxKind[] syntaxKinds =
		[
			SyntaxKind.MethodDeclaration,
			SyntaxKind.GetAccessorDeclaration,
			SyntaxKind.SetAccessorDeclaration,
			SyntaxKind.AddAccessorDeclaration,
			SyntaxKind.RemoveAccessorDeclaration,
		];

		context.RegisterSyntaxNodeAction(Analyze, syntaxKinds);
	}

	private static void Analyze(SyntaxNodeAnalysisContext context)
	{
		ISymbol syntaxNodeSymbol = context.SemanticModel.GetDeclaredSymbol(context.Node);

		foreach (SyntaxNode descendantSyntaxNode in context.Node.DescendantNodes())
		{
			if (descendantSyntaxNode is not IdentifierNameSyntax) continue;

			if (context.SemanticModel.GetSymbol(descendantSyntaxNode) is not { } identifierNameSymbol) continue;

			foreach (AttributeData attribute in identifierNameSymbol.GetAttributes(typeof(PreventUsageInsideAttribute).GetFullyQualifiedName()))
			{
				if (attribute.GetConstructorArgument<string>(0) is not { } typeName || string.IsNullOrWhiteSpace(typeName)) continue;

				if (!syntaxNodeSymbol.ContainingType.IsSubtypeOf(typeName)) continue;

				if (attribute.GetConstructorArgument<string>(1) is not { } memberName || string.IsNullOrWhiteSpace(memberName) || memberName != syntaxNodeSymbol.Name) continue;

				context.ReportDiagnostic(Diagnostic.Create(Descriptor, descendantSyntaxNode.GetLocation(), identifierNameSymbol?.Name, syntaxNodeSymbol.Name, attribute.GetConstructorArgument<string>(2)));
			}
		}
	}
}
