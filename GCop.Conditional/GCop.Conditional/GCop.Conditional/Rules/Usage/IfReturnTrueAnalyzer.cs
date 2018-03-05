namespace GCop.Conditional.Rules.Usage
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class IfReturnTrueAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.IfStatement;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "542",
                Category = Category.Usage,
                Severity = DiagnosticSeverity.Warning,
                Message = "Since the condition is already a boolean it can be returned directly."
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var ifStatement = context.Node as IfStatementSyntax;
            if (ifStatement?.Else == null) return;
            var statementInsideIf = GetSingleStatementFromPossibleBlock(ifStatement.Statement);
            if (statementInsideIf == null) return;
            var statementInsideElse = GetSingleStatementFromPossibleBlock(ifStatement.Statement);
            if (statementInsideElse == null) return;
            var returnIf = statementInsideIf as ReturnStatementSyntax;
            var returnElse = statementInsideElse as ReturnStatementSyntax;
            if (returnIf == null || returnElse == null) return;
            if ((returnIf.Expression is LiteralExpressionSyntax && returnIf.Expression.IsKind(SyntaxKind.TrueLiteralExpression) &&
                returnElse.Expression is LiteralExpressionSyntax && returnElse.Expression.IsKind(SyntaxKind.FalseLiteralExpression)) ||
                (returnIf.Expression is LiteralExpressionSyntax && returnIf.Expression.IsKind(SyntaxKind.FalseLiteralExpression) &&
                returnElse.Expression is LiteralExpressionSyntax && returnElse.Expression.IsKind(SyntaxKind.TrueLiteralExpression)))
            {
                ReportDiagnostic(context, ifStatement.IfKeyword.GetLocation());
            }
        }

        private StatementSyntax GetSingleStatementFromPossibleBlock(StatementSyntax statement)
        {
            if (statement is BlockSyntax block)
            {
                if (block.Statements.Count != 1) return null;
                return block.Statements.Single();
            }
            else
            {
                return statement;
            }
        }
    }
}
