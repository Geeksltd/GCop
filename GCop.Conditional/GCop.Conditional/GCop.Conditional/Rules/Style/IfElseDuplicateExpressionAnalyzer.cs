namespace GCop.Conditional.Rules.Style
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class IfElseDuplicateExpressionAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.IfStatement;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "412",
                Category = Category.Style,
                Severity = DiagnosticSeverity.Warning,
                Message = "{0} should go outside the if condition."
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;

            var @if = NodeToAnalyze as IfStatementSyntax;
            if (@if.Else == null) return;

            var ifBody = @if.Statement as BlockSyntax;
            if (ifBody == null) return;

            ExpressionStatementSyntax elseLastExpression = null;
            var elseBody = @if.Else.Statement as BlockSyntax;
            if (elseBody == null)
                elseLastExpression = @if.Else.Statement as ExpressionStatementSyntax;
            else
                elseLastExpression = elseBody.ChildNodes().OfType<ExpressionStatementSyntax>().LastOrDefault();

            var ifLastExpression = ifBody.ChildNodes().OfType<ExpressionStatementSyntax>().LastOrDefault();

            if (ifLastExpression == null || elseLastExpression == null) return;

            if (@if.Parent?.Kind() == SyntaxKind.ElseClause)
            {
                var parentLastExpression = @if.Parent?.Parent?.As<IfStatementSyntax>()?.Statement?.ChildNodes().OfType<ExpressionStatementSyntax>().LastOrDefault();
                if (parentLastExpression == null) return;
                if (ifLastExpression.ToString().Trim() == elseLastExpression.ToString().Trim() && ifLastExpression.ToString().Trim() == parentLastExpression.ToString().Trim())
                {
                    ReportDiagnostic(context, ifLastExpression, ifLastExpression.ToString());
                    ReportDiagnostic(context, elseLastExpression, elseLastExpression.ToString());
                    ReportDiagnostic(context, parentLastExpression, parentLastExpression.ToString());
                }
                return;
            }

            if (ifLastExpression.ToString() == elseLastExpression.ToString())
            {
                ReportDiagnostic(context, ifLastExpression, ifLastExpression.ToString());
                ReportDiagnostic(context, elseLastExpression, elseLastExpression.ToString());
            }
        }
    }
}
