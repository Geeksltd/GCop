namespace GCop.Conditional.Rules.Design
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class IfStatementLineBreakAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.IfStatement;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "110",
                Category = Category.Design,
                Severity = DiagnosticSeverity.Warning,
                Message = "There must be at least a single line break before IF statement unless the body of the IF is written in the same line"
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            return;

            #region [Disabled]
            //var @if = (IfStatementSyntax)context.Node;

            //var ifKeyword = @if.GetFirstToken();
            //if (ifKeyword == null) return;

            //var numberOfLineBreaks = ifKeyword.GetAllTrivia().Where(it => it.IsKind(SyntaxKind.EndOfLineTrivia)).Count();

            //if (numberOfLineBreaks == NotAllowedNumber)
            //{
            //    var parentChildNodes = @if.Parent.ChildNodesAndTokens().ToList();
            //    var previous = parentChildNodes[parentChildNodes.IndexOf(@if) - 1];
            //    if (previous == null) return;
            //    if (previous.IsKind(SyntaxKind.OpenBraceToken) || previous.IsKind(SyntaxKind.ElseKeyword)) return;

            //    var closeParen = @if.DescendantTokens().FirstOrDefault(it => it.IsKind(SyntaxKind.CloseParenToken));
            //    if (closeParen.GetAllTrivia().Any(it => it.IsKind(SyntaxKind.EndOfLineTrivia)))
            //    {
            //        ReportDiagnostic(context, @if);
            //    }
            //}
            #endregion
        }

        private void ReportDiagnostic(SyntaxNodeAnalysisContext context, SyntaxNode node)
        {
            var diagnostic = Diagnostic.Create(Description, node.GetFirstToken().GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }
}
