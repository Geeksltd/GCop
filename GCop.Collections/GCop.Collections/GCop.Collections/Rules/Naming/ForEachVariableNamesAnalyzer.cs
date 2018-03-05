namespace GCop.Collections.Rules.Naming
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class ForEachVariableNamesAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
	{
		private readonly short AllowedNumbers = 5;
		protected override SyntaxKind Kind => SyntaxKind.ForEachStatement;

		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "203",
				Category = Category.Naming,
				Message = "Use meaningful names instead of single character for foreach identifier",
                Severity = DiagnosticSeverity.Warning
			};
		}

		protected override void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var forEachStatement = (ForEachStatementSyntax)context.Node;

			var forEachBody = forEachStatement.DescendantNodes().OfType<BlockSyntax>().FirstOrDefault();
			if (forEachBody == null) return;

			if (forEachBody.Statements.Count < AllowedNumbers) return;

			if (forEachStatement.Identifier.ValueText.IsSingleCharacter() == false) return;

			var method = forEachStatement.GetParent<MethodDeclarationSyntax>();
			if (MethodHasDllImportAttribute(method as MethodDeclarationSyntax)) return;

			ReportDiagnostic(context, forEachStatement.Identifier);
		}

		private bool MethodHasDllImportAttribute(MethodDeclarationSyntax method)
		{
			return method != null && method.AttributeLists.SelectMany(it => it.Attributes).Any(it => it.Name.ToString() == "DllImport");
		}
	}
}
