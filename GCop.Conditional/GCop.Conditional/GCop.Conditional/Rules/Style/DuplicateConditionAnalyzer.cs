namespace GCop.Conditional.Rules.Style
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System;
    using System.Collections.Immutable;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DuplicateConditionAnalyzer : GCopAnalyzer
    {
        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "423",
                Category = Category.Style,
                Severity = DiagnosticSeverity.Warning,
                Message = "This condition was just checked on line {0}."
            };
        }

        protected override void Configure()
        {
            RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
            RegisterSyntaxNodeAction(AnalyzeSwitchStatement, SyntaxKind.SwitchStatement);
        }

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            CheckMatchingExpressionsInSucceedingStatements((IfStatementSyntax)NodeToAnalyze, syntax => syntax.Condition, context);
        }

        private void AnalyzeSwitchStatement(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            CheckMatchingExpressionsInSucceedingStatements((SwitchStatementSyntax)NodeToAnalyze, syntax => syntax.Expression, context);
        }

        private void CheckMatchingExpressionsInSucceedingStatements<T>(T statement,
            Func<T, ExpressionSyntax> expressionSelector, SyntaxNodeAnalysisContext context) where T : StatementSyntax
        {
            var previousStatement = GetPrecedingStatement(statement) as T;
            if (previousStatement == null)
            {
                return;
            }

            var currentExpression = expressionSelector(statement);
            var previousExpression = expressionSelector(previousStatement);

            if (currentExpression.IsEquivalent(previousExpression) &&
                !ContainsPossibleUpdate(previousStatement, currentExpression, context.SemanticModel))
            {
                ReportDiagnostic(context, currentExpression.GetLocation(), previousExpression.GetLineNumberToReport().ToString());
            }
        }

        private bool ContainsPossibleUpdate(StatementSyntax statement, ExpressionSyntax expression,
            SemanticModel semanticModel)
        {
            var checkedSymbols = expression.DescendantNodesAndSelf()
                .Select(node => semanticModel.GetSymbolInfo(node).Symbol)
                .Where(symbol => symbol != null)
                .ToImmutableHashSet();

            var statementDescendents = statement.DescendantNodesAndSelf().ToList();
            var assignmentExpressions = statementDescendents
                .OfType<AssignmentExpressionSyntax>()
                .Any(expressionSyntax =>
                {
                    var symbol = semanticModel.GetSymbolInfo(expressionSyntax.Left).Symbol;
                    return symbol != null && checkedSymbols.Contains(symbol);
                });

            if (assignmentExpressions)
            {
                return true;
            }

            var postfixUnaryExpression = statementDescendents
                .OfType<PostfixUnaryExpressionSyntax>()
                .Any(expressionSyntax =>
                {
                    var symbol = semanticModel.GetSymbolInfo(expressionSyntax.Operand).Symbol;
                    return symbol != null && checkedSymbols.Contains(symbol);
                });

            if (postfixUnaryExpression)
            {
                return true;
            }

            var prefixUnaryExpression = statementDescendents
                .OfType<PrefixUnaryExpressionSyntax>()
                .Any(expressionSyntax =>
                {
                    var symbol = semanticModel.GetSymbolInfo(expressionSyntax.Operand).Symbol;
                    return symbol != null && checkedSymbols.Contains(symbol);
                });

            return prefixUnaryExpression;
        }

        private StatementSyntax GetPrecedingStatement(StatementSyntax currentStatement)
        {
            var statements = currentStatement.Parent.ChildNodes().OfType<StatementSyntax>().ToList();

            var index = statements.IndexOf(currentStatement);

            return index == 0 ? null : statements[index - 1];
        }
    }
}
