namespace GCop.Collections.Rules.Performance
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class IEnumerableMultipleEvaluationsAnalyzer : GCopAnalyzer
	{
		private readonly string[] MethodNames = { "Aggregate", "All", "Any", "Count", "Where", "Select", "SelectMany" };

		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "321",
				Category = Category.Performance,
				Severity = DiagnosticSeverity.Warning,
				Message = "{0}"
			};
		}

		protected override void Configure()
		{
			RegisterSyntaxNodeAction(AnalyzeExpression, SyntaxKind.InvocationExpression);
		}

		void AnalyzeExpression(SyntaxNodeAnalysisContext context)
		{
			string message = "";
			NodeToAnalyze = context.Node;

			var evaluate = EvaluateReturn(context, message);
			if (evaluate.Statue) return;

			var forEach = evaluate.Invocation.GetSingleAncestor<ForEachStatementSyntax>();
			if (forEach != null && evaluate.Invocation.SpanStart > forEach?.Statement?.SpanStart)
			{
				if (evaluate.MemberSymbol is ILocalSymbol)
				{
					if (forEach.DescendantNodes().OfType<LocalDeclarationStatementSyntax>()
						.Any(x => x.Declaration.ChildNodes().OfType<VariableDeclaratorSyntax>().FirstOrDefault()?.Identifier.ToString() == evaluate.MemberAccess.GetIdentifier()))
						return;
				}
				ReportDiagnostic(context, evaluate.MemberAccessIdentifier, message);
			}

			var parentInvocation = evaluate.Invocation.GetSingleAncestor<InvocationExpressionSyntax>();
			if (parentInvocation == null) return;

			var parentMemberAccess = parentInvocation.Expression as MemberAccessExpressionSyntax;
			if (parentMemberAccess == null) return;

			if (MethodNames.Lacks(parentMemberAccess.Name.ToString())) return;

			var parentIdentifierSyntax = parentMemberAccess.GetIdentifierSyntax();
			if (parentIdentifierSyntax == null) return;

			var parentTypeInfo = context.SemanticModel.GetTypeInfo(parentIdentifierSyntax).Type as ITypeSymbol;
			if (parentTypeInfo == null) return;
			if (parentTypeInfo?.Name != "IEnumerable") return;

			ReportDiagnostic(context, evaluate.MemberAccessIdentifier, message);
		}

		private EvaluateStructure EvaluateReturn(SyntaxNodeAnalysisContext context, string message)
		{
			var evaluateStructure = new EvaluateStructure { Statue = false };
			var invocation = NodeToAnalyze as InvocationExpressionSyntax;

			var memberAccess = invocation.Expression as MemberAccessExpressionSyntax;
			if (memberAccess == null) evaluateStructure.Statue = true;

			var memberAccessIdentifier = memberAccess.GetIdentifierSyntax();
			if (memberAccessIdentifier == null) evaluateStructure.Statue = true;

			var typeInfo = context.SemanticModel.GetTypeInfo(memberAccessIdentifier).Type as ITypeSymbol;
			if (typeInfo == null) evaluateStructure.Statue = true;
			if (typeInfo?.Name != "IEnumerable") evaluateStructure.Statue = true;

			var symbolInfo = context.SemanticModel.GetSymbolInfo(memberAccessIdentifier).Symbol;
			if (symbolInfo == null) evaluateStructure.Statue = true;

			if (symbolInfo is ILocalSymbol)
				message = $"The variable '{ memberAccess.GetIdentifier() }' is enumerated multiple times. To improve performance, call \".ToList()\" where it's defined.";
			else if (symbolInfo is IFieldSymbol || symbolInfo is IPropertySymbol)
				message = $"'{memberAccess.GetIdentifier() }' is enumerated multiple times" +
						  $"To improve performance, define a variable before the loop and set to '{memberAccess.GetIdentifier()}.ToList()' and use that variable in the loop.";

			if (MethodNames.Lacks(memberAccess.Name.ToString())) evaluateStructure.Statue = true;
			evaluateStructure.Invocation = invocation;
			evaluateStructure.MemberAccess = memberAccess;
			evaluateStructure.MemberSymbol = symbolInfo;
			evaluateStructure.MemberAccessIdentifier = memberAccessIdentifier;
			return evaluateStructure;
		}
	}

	class EvaluateStructure
	{
		public bool Statue { get; set; }
		public InvocationExpressionSyntax Invocation { get; set; }
		public MemberAccessExpressionSyntax MemberAccess { get; set; }
		public ISymbol MemberSymbol { get; set; }
		public IdentifierNameSyntax MemberAccessIdentifier { get; set; }
	}
}