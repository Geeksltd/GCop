namespace GCop.Common.Rules.Usage
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class UseGetCustomAttributeAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
	{
		protected override SyntaxKind Kind => SyntaxKind.InvocationExpression;

		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "526",
				Category = Category.Usage,
				Severity = DiagnosticSeverity.Warning,
				Message = "Use the Generic version instead. Ensure you have 'using System.Reflection;'"
			};
		}

		protected override void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var invocExpres = NodeToAnalyze as InvocationExpressionSyntax;

			var simpleMemeber = invocExpres.ChildNodes().OfKind(SyntaxKind.SimpleMemberAccessExpression).FirstOrDefault();
			if (simpleMemeber == null)
				return;

			//myMemberInfo.GetCustomAttributes();  
			var methodNode = simpleMemeber.ChildNodes().LastOrDefault();
			if (methodNode == null) return;

			if (methodNode.GetIdentifier() != "GetCustomAttributes") return;

			var varibale = simpleMemeber.ChildNodes().FirstOrDefault();
			if (varibale == null) return;

			//myMemberInfo.GetCustomAttributes();
			var variableSymbol = context.SemanticModel.GetSymbolInfo(varibale).Symbol as ILocalSymbol;
			if (variableSymbol == null) return;
			if (variableSymbol.Type == null) return;
			if (variableSymbol.Type.ToString().Contains("System.Reflection"))
			{
				ReportDiagnostic(context, simpleMemeber);
			}
		}
	}
}
