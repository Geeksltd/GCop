namespace GCop.Common.Rules.Refactoring
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class ConvertToLambdaExpressionAnalyzer : GCopAnalyzer
	{
		const int MethodBodylenght = 150;
		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "638",
				Category = Category.Refactoring,
				Severity = DiagnosticSeverity.Warning,
				Message = "Shorten this method by defining it as expression-bodied."
			};
		}

		protected override void Configure()
		{
			RegisterSyntaxNodeAction(AnalyzeMethods, SyntaxKind.MethodDeclaration);
		}

		void AnalyzeMethods(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var method = (MethodDeclarationSyntax)context.Node;
			var classDeclaration = method.GetParent<ClassDeclarationSyntax>();
			if (classDeclaration?.As<ClassDeclarationSyntax>()?.Identifier.ValueText == "TaskManager") return;

			var methodInfo = context.SemanticModel.GetDeclaredSymbol(method);
			if (methodInfo.IsVirtual) return;
			if (method.Body == null) return;
			if (method.ToString().Length > MethodBodylenght) return;
			if (method.Body.ChildNodes().OfType<StatementSyntax>().IsSingle() == false) return;

			if (method.Body.ChildNodes().OfType<UsingStatementSyntax>().Any()) return;

			var firstChild = method.Body.ChildNodes().OfType<StatementSyntax>().First().ToString();

			if (firstChild.StartsWith("yield")) return;
			if (firstChild.StartsWith("throw")) return;
			if (firstChild.StartsWith("if")) return;
			if (!firstChild.StartsWith("return")) return;

			ReportDiagnostic(context, method.Identifier.GetLocation());
		}
	}
}
