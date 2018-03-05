namespace GCop.Common.Rules.Naming
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class UseCalculateForMethodNameAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
	{
		private readonly string FetchPrefix = "Get";
		private readonly short MaximumNumberOfStatement = 25;
		protected override SyntaxKind Kind => SyntaxKind.MethodDeclaration;

		private MethodDeclarationSyntax Method;
		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "207",
				Category = Category.Naming,
				Severity = DiagnosticSeverity.Warning,
				Message = "The logic seems extensive. Rename the method to imply this. E.g: Calculate{0}, Find{0}, Select{0}, Create{0}, Evaluate{0}, etc"
			};
		}

		protected override void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			Method = context.Node as MethodDeclarationSyntax;
			if (Method == null) return;

			var methodName = Method.GetName();
			if (methodName.IsEmpty()) return;

			if (!methodName.StartsWith(FetchPrefix)) return;

			if (methodName.Length > 3 && !char.IsUpper(methodName[3])) return;

			if (Method.Body == null) return;

			if (Method.Body?.ContainsSyntax<ObjectCreationExpressionSyntax>(it => it.ChildNodes().OfType<IdentifierNameSyntax>().FirstOrDefault()?.Identifier.ValueText == "StringBuilder") ?? true) return;

			if (Method.Body.GetCountOfStatements() <= MaximumNumberOfStatement) return;

			//the word Get will be removed
			var newName = methodName?.Remove(0, 3);
			ReportDiagnostic(context, Method.Identifier, newName);
		}
	}
}
