namespace GCop.ErrorHandling.Rules.Performance
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ThrowStatementAnalyzer : GCopAnalyzer
    {
        private CatchClauseSyntax Catch;
        private ThrowStatementSyntax Throw;

        public bool IsExceptionThrownByInstantiation => Throw.ChildNodes().OfType<ObjectCreationExpressionSyntax>().Any();

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "311",
                Category = Category.Performance,
                Message = "Throw exception without specifying the original exception. Remove {0} from throw statement",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Configure()
        {
            RegisterSyntaxNodeAction(ctx => AnalyzeThrowStatements(ctx), SyntaxKind.ThrowStatement);
        }

        private void AnalyzeThrowStatements(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            Throw = context.Node as ThrowStatementSyntax;
            Catch = Throw.Parent.Parent as CatchClauseSyntax;

            if (Catch == null) return;

            var exceptionIdentifier = Throw.DescendantNodes().OfType<IdentifierNameSyntax>().FirstOrDefault()?.Identifier.ValueText;

            if (Catch.Declaration == null || exceptionIdentifier.IsEmpty()) return;

            if (Catch.Declaration?.Identifier.ValueText == exceptionIdentifier)
            {
                ReportDiagnostic(context, Throw, exceptionIdentifier);
            }
        }
    }
}
