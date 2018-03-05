namespace GCop.Collections.Rules.Performance
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class EvaluateIEnumerableToListAnalyzer : GCopAnalyzer
	{
		private readonly string SaveConstructed = "MSharp.Framework.Database.Save<T>";
		private readonly string UpdateConstructed = "MSharp.Framework.Database.Update<T>";

		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "320",
				Category = Category.Performance,
				Severity = DiagnosticSeverity.Warning,
				Message = "While {0} is being enumerated at run-time, saving of each of its items can potentially affect the source expression from which this IEnumerable object is define upon. To avoid unintended side effects, evaluate myobjects into an Array or List before passing it to Database.XXX.",
			};
		}

		protected override void Configure()
		{
			RegisterSyntaxNodeAction(AnalyzeExpression, SyntaxKind.InvocationExpression);
		}

		void AnalyzeExpression(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var invocation = context.Node as InvocationExpressionSyntax;
			if (invocation.ArgumentList.Arguments.Count != 1) return;

			var memberAccess = invocation.Expression as MemberAccessExpressionSyntax;
			if (memberAccess == null) return;

			var method = context.SemanticModel.GetSymbolInfo(memberAccess).Symbol as IMethodSymbol;
			if (method == null) return;

			if (!method.ConstructedFrom.ToString().StartsWith(SaveConstructed) &&
				!method.ConstructedFrom.ToString().StartsWith(UpdateConstructed)) return;

			var argType = context.SemanticModel.GetTypeInfo(invocation.ArgumentList.Arguments[0].Expression).Type as ITypeSymbol;

			if (argType.Name == "IEnumerable")
				ReportDiagnostic(context, invocation, new string[] { invocation.ArgumentList.Arguments[0].Expression.ToString() });
		}
	}
}
