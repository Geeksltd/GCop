namespace GCop.ErrorHandling.Rules.Design
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StaticConstructorExceptionAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.ConstructorDeclaration;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "173",
                Category = Category.Design,
                Severity = DiagnosticSeverity.Warning,
                Message = "Don't throw exception inside static constructors."
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var ctor = (ConstructorDeclarationSyntax)context.Node;

            if (ctor.Modifiers.Any(SyntaxKind.StaticKeyword) == false) return;

            if (ctor.Body == null) return;

            var @throw = ctor.Body.ChildNodes().OfType<ThrowStatementSyntax>().FirstOrDefault();

            if (@throw == null) return;

            ReportDiagnostic(context, @throw.GetLocation(), ctor.Identifier.Text);
        }
    }
}
