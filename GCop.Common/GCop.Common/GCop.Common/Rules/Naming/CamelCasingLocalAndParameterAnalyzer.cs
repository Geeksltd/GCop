namespace GCop.Common.Rules.Naming
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class CamelCasingLocalAndParameterAnalyzer : GCopAnalyzer
	{
		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "201",
				Category = Category.Naming,
				Message = "Use camelCasing when declaring {0}",
                Severity = DiagnosticSeverity.Warning
			};
		}

		protected override void Configure()
		{
			RegisterSyntaxNodeAction(c => Analyze(c), SyntaxKind.LocalDeclarationStatement, SyntaxKind.Parameter);
		}

		protected void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			SyntaxToken identifier;
			var isParameter = true;

			//If node is any part of delegate declaration we have to ignore the rule
			if (context.Node.Ancestors().Any(it => it.IsKind(SyntaxKind.DelegateDeclaration))) return;

			if (context.Node is ParameterSyntax)
			{
				identifier = ((ParameterSyntax)context.Node).Identifier;
				var method = context.Node.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();
				if (method != null)
				{
					if (method.AttributeLists.Any(it => it.Attributes.Any(attr => attr.Name.ToString() == "DllImport"))) return;
				}
				isParameter = true;
			}
			else
			{
				if (context.Node.ChildTokens().Any(it => it.IsKind(SyntaxKind.ConstKeyword))) return;

				var dec = context.Node.DescendantNodes().OfType<VariableDeclaratorSyntax>().FirstOrDefault();
				if (dec == null) return;

				identifier = dec.Identifier;
				isParameter = false;
			}

			var identifierText = identifier.ToString();
			if (identifierText.IsEmpty()) return;

			if (identifierText.None(@char => @char != '_') && isParameter) return;

			if (identifierText.IsCamelCase() || identifierText[0] == '@') return;
			//if (!identifierText.IsCamelCase() && identifierText[0] != '@')

			var msg = isParameter ? "a parameter" : "local variables";
			var diagnostic = Diagnostic.Create(Description, identifier.GetLocation(), msg);
			context.ReportDiagnostic(diagnostic);
		}
	}
}