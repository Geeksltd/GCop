namespace GCop.Common.Rules.Naming
{
	using Core;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using Microsoft.CodeAnalysis.Diagnostics;
	using System;
	using System.Linq;

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class GetXxxCountAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
	{
		//GetCount is minimum lenght for this rule 
		static int BaseLenght = "GetCount".Length;

		protected override SyntaxKind Kind => SyntaxKind.MethodDeclaration;
		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "215",
				Category = Category.Naming,
				Severity = DiagnosticSeverity.Warning,
				Message = "Rename the method to \"Count{0}\" as it's shorter and more readable."
			};
		}

		protected override void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var methodDeclaration = (MethodDeclarationSyntax)context.Node;

			var methodName = methodDeclaration.Identifier.ValueText;
			if (methodName.IsEmpty()) return;

			if (methodName.Length < BaseLenght) return;

			//For example : GetCustomersCount is a name of method 
			// Check the Get
			var part1 = methodName.Substring(0, 3);
			if (part1 != "Get") return;

			//Check the Count is end of the name of method
			var part2 = methodName.Substring(methodName.Length - 5, 5);
			if (part2 != "Count") return;

			// find customer in this sample : GetCustomersCount
			var part3 = methodName.Substring(3, methodName.Length - BaseLenght);
			if (part3.IsEmpty() || part3.FirstOrDefault().IsLower()) return;

			ReportDiagnostic(context, methodDeclaration.Identifier, part3);
		}
	}
}
