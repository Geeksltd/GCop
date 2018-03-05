namespace GCop.Common.Rules.Naming
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class ParameterAbbreviationAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
	{
		private readonly string[] NotAllowedAbbreviations = new string[] { "addr", "res" };
		protected override SyntaxKind Kind => SyntaxKind.Parameter;

		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "212",
				Category = Category.Naming,
				Message = "Do not use {0} as abbreviation for method parameter",
                Severity = DiagnosticSeverity.Warning
			};
		}

		protected override void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var parameter = context.Node as ParameterSyntax;

			var parameterName = parameter.Identifier.ValueText;

			if (IsValid(parameterName)) return;

			ReportDiagnostic(context, parameter.Identifier, parameterName);
		}


		private bool IsValid(string name) => NotAllowedAbbreviations.Lacks(name);
	}
}
