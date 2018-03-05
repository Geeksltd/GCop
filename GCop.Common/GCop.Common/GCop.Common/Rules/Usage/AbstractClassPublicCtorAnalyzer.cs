namespace GCop.Common.Rules.Usage
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class AbstractClassPublicCtorAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
	{
		protected override SyntaxKind Kind => SyntaxKind.ConstructorDeclaration;

		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "540",
				Category = Category.Usage,
				Severity = DiagnosticSeverity.Warning,
				Message = "Abastract class should not have public constructors. Make it protected instead."
			};
		}

		protected override void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var ctor = (ConstructorDeclarationSyntax)context.Node;
			if (ctor.Modifiers.Any(it => it.IsKind(SyntaxKind.PublicKeyword)) == false) return;

			var @class = ctor.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
			if (@class == null) return;
			if (@class.Modifiers.Any(it => it.IsKind(SyntaxKind.AbstractKeyword)) == false) return;

			ReportDiagnostic(context, ctor.Modifiers.FirstOrDefault(it => it.IsKind(SyntaxKind.PublicKeyword)).GetLocation() ?? ctor.GetLocation());
		}
	}
}
