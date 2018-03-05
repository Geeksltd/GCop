namespace GCop.Conditional.Rules.Refactoring
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ReverseIfCriteriaIfLastBlockInVoidMethodAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.MethodDeclaration;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "622",
                Category = Category.Refactoring,
                Severity = DiagnosticSeverity.Warning,
                Message = "Reverse your IF condition and return. Then move the nested statements to after the IF."
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var expression = NodeToAnalyze as MethodDeclarationSyntax;

            if (expression == null || expression.ReturnType.ToString() != "void" || IsRuleException(expression.Identifier.ToString()))
                return;

            var block = expression.ChildNodes().OfType<BlockSyntax>().FirstOrDefault();
            var lastIfStatement = GetLastIfStatementBlock(block);

            if (lastIfStatement == null || !HasMoreThanTwoStatements(lastIfStatement))
                return;

            ReportDiagnostic(context, lastIfStatement.Condition);
        }

        private bool IsRuleException(string methodName)
        {
            var exceptions = new[]
            {
                "OnSaving",
                "OnSaved",
                "OnDeleting",
                "OnDeleted",
                "OnValidating",
                "Validate"
            };

            return methodName.IsAnyOf(exceptions);
        }

        private IfStatementSyntax GetLastIfStatementBlock(BlockSyntax block)
        {
            var lastIfStatement = block?.Statements.LastOrDefault() as IfStatementSyntax;
            var elseClause = lastIfStatement?.ChildNodes().OfType<ElseClauseSyntax>().FirstOrDefault();

            if (lastIfStatement == null || elseClause != null)
                return null;

            return lastIfStatement.ChildNodes().OfType<BlockSyntax>().FirstOrDefault() != null
                ? lastIfStatement
                : null;
        }

        private bool HasMoreThanTwoStatements(IfStatementSyntax ifBlock)
        {
            return (ifBlock.ChildNodes().OfType<BlockSyntax>().FirstOrDefault()?.ChildNodes().OfType<StatementSyntax>().Count() ?? 0) > 2;
        }
    }
}
