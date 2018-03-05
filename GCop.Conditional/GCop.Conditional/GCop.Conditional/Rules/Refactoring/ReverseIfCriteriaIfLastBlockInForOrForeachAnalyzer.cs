namespace GCop.Conditional.Rules.Refactoring
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ReverseIfCriteriaIfLastBlockInForOrForeachAnalyzer : GCopAnalyzer
    {
        private const int StatementCount = 6;
        protected override void Configure()
        {
            RegisterSyntaxNodeAction(Analyze, SyntaxKind.ForEachStatement, SyntaxKind.ForStatement);
        }

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "616",
                Category = Category.Refactoring,
                Severity = DiagnosticSeverity.Warning,
                Message = "Reverse your IF criteria and use 'continue'. That will eliminate the need for a big IF block and make the code more readable."
            };
        }

        private void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var expression = GetForeachOrForStatementSyntax(NodeToAnalyze);

            if (expression == null)
                return;

            var block = expression.ChildNodes().OfType<BlockSyntax>()?.FirstOrDefault();
            var lastIfStatement = GetLastIfStatementBlock(block);

            if (lastIfStatement == null)
                return;

            if (!HasMoreThanOneStatement(lastIfStatement))
                return;

            ReportDiagnostic(context, lastIfStatement.Condition);
        }

        private SyntaxNode GetForeachOrForStatementSyntax(SyntaxNode nodeToAnalyze)
        {
            SyntaxNode expression = nodeToAnalyze as ForEachStatementSyntax;
            return expression ?? nodeToAnalyze as ForStatementSyntax;
        }

        private IfStatementSyntax GetLastIfStatementBlock(BlockSyntax block)
        {
            var lastIfStatement = block?.Statements.LastOrDefault() as IfStatementSyntax;

            if (lastIfStatement == null)
                return null;

            var elseClause = lastIfStatement.ChildNodes().OfType<ElseClauseSyntax>()?.FirstOrDefault();

            if (elseClause != null)
                return null;

            return lastIfStatement.ChildNodes().OfType<BlockSyntax>()?.FirstOrDefault() != null
                ? lastIfStatement
                : null;
        }

        private bool HasMoreThanOneStatement(IfStatementSyntax ifBlock)
        {
            return (ifBlock.ChildNodes().OfType<BlockSyntax>()?.FirstOrDefault()?.ChildNodes()?.OfType<StatementSyntax>()?.Count() ?? 0) > StatementCount;
        }
    }
}
