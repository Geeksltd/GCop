namespace GCop.Common.Rules.Naming
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class UnderscoreAnalyzer : GCopAnalyzer
	{
		//public const string A_B = "10";
		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "206",
				Category = Category.Naming,
				Message = "Avoid using underscores in {0}",
                Severity = DiagnosticSeverity.Warning
			};
		}

		protected override void Configure()
		{
			RegisterSyntaxNodeAction(c => AnalyzeName(c), SyntaxKind.ClassDeclaration, SyntaxKind.LocalDeclarationStatement);
		}

		private void AnalyzeName(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var isClassDeclaration = true;
			var identifier = (context.Node as ClassDeclarationSyntax)?.Identifier;

			if (identifier == null)
			{
				if (context.Node.ChildTokens().Any(it => it.IsKind(SyntaxKind.ConstKeyword))) return;
				identifier = context.Node.DescendantNodes().OfType<VariableDeclaratorSyntax>().FirstOrDefault()?.Identifier;
				isClassDeclaration = false;
			}
			else
			{
				var classDeclaration = context.Node as ClassDeclarationSyntax;
				foreach (var item in classDeclaration.AttributeLists)
				{
					if (item.Attributes.Any(x => x.Name.ToString() == "EscapeGCop")) return;
				}
			}

			if (identifier == null) return;

			var decName = identifier.Value.ToString();
			if (decName.ToCharArray().Lacks('_')) return;

			var msgArg = isClassDeclaration ? "the class name" : "a local method variable declaration";
			var diagnostic = Diagnostic.Create(Description, identifier.Value.GetLocation(), msgArg);

			context.ReportDiagnostic(diagnostic);
		}
	}
}
