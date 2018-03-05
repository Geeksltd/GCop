namespace GCop.Common.Rules.Naming
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class GetXXXMethodNameAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
	{
		protected override SyntaxKind Kind => SyntaxKind.MethodDeclaration;
		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "216",
				Category = Category.Naming,
				Severity = DiagnosticSeverity.Warning,
				Message = "A method named `{0}` is expected return a value. If it's meant to be void, then use a verb other than `Get` such as Read, Download, Sync, ... "
			};
		}

		protected override void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var methodDeclaration = (MethodDeclarationSyntax)context.Node;

			var methodName = methodDeclaration.Identifier.ValueText;
			if (methodName.IsEmpty()) return;

			if (methodName.Length < 4) return;

			var part1 = methodName.Substring(0, 3);
			if (part1 != "Get") return;

			var checkUpperCase = methodName.Remove("Get");

			//if a method name starts with Get and then an upper case letter, then if it's void, show a warning to say: 
			var firstLetter = checkUpperCase.Substring(0, 1);
			if (firstLetter.FirstOrDefault().IsLower()) return;

			// then if it's void
			var methodInfo = context.SemanticModel.GetDeclaredSymbol(methodDeclaration) as IMethodSymbol;
			if (methodInfo == null) return;

			if (methodInfo.ReturnsVoid)
				ReportDiagnostic(context, methodDeclaration.Identifier, methodName);//, checkUpperCase);
		}
	}
}
