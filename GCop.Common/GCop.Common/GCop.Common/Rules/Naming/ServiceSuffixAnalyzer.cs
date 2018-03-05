namespace GCop.Common.Rules.Naming
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class ServiceSuffixAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
	{
		private readonly string Directory = "Service";
		protected override SyntaxKind Kind => SyntaxKind.ClassDeclaration;

		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "210",
				Category = Category.Naming,
				Message = "Suffix name of a service class with Service",
                Severity = DiagnosticSeverity.Warning
			};
		}

		protected override void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var @class = (ClassDeclarationSyntax)context.Node;

			if (!@class.IsInDirectory(Directory)) return;

			if (!@class.Identifier.ValueText.EndsWith("Service"))
				ReportDiagnostic(context, @class.Identifier);
		}
	}
}
