namespace GCop.Conditional.Rules.Refactoring
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AvoidUsingIfElseForAssigningOrReturningBooleansAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.IfStatement;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "619",
                Category = Category.Refactoring,
                Severity = DiagnosticSeverity.Warning,
                Message = "Should be written \"{0}\"."
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;

            var ifStatement = NodeToAnalyze as IfStatementSyntax;
            var elseClause = ifStatement?.ChildNodes().OfType<ElseClauseSyntax>()?.FirstOrDefault();

            var ifAssignment = GetBooleanAssignment(ifStatement);
            var elseAssignment = GetBooleanAssignment(elseClause);

            if (ifAssignment == null || elseAssignment == null)
            {
                var ifReturnValue = GetBooleanReturnStatement(ifStatement);
                var elseReturnValue = GetBooleanReturnStatement(elseClause);

                if (ifReturnValue == null || elseReturnValue == null || ifReturnValue.IsKind(elseReturnValue.Kind()))
                    return;

                var returnConditionText = ifReturnValue.IsKind(SyntaxKind.TrueLiteralExpression)
                    ? ifStatement.Condition.ToString()
                    : $"!({ifStatement.Condition})";

                ReportDiagnostic(context, ifStatement, $"return {returnConditionText}");
            }

            if (!IsIfElseBooleanVariableAssignment(ifAssignment, elseAssignment, context))
                return;

            var conditionText = ifAssignment.Right.IsKind(SyntaxKind.TrueLiteralExpression)
                ? ifStatement.Condition.ToString()
                : $"!({ifStatement.Condition})";

            ReportDiagnostic(context, ifStatement, $"{ifAssignment.Left} = {conditionText}");
        }

        private bool IsIfElseBooleanVariableAssignment(AssignmentExpressionSyntax ifAssignment, AssignmentExpressionSyntax elseAssignment, SyntaxNodeAnalysisContext context)
        {
            if (ifAssignment == null || elseAssignment == null)
                return false;

            var isSameVariable = ifAssignment.Left.ToString() == elseAssignment.Left.ToString();
            var isOppositeBooleanValues = ifAssignment.Right.Kind() != elseAssignment.Right.Kind();

            var identifier = ifAssignment.GetIdentifierSyntax() ?? ifAssignment.ChildNodes().OfType<MemberAccessExpressionSyntax>().FirstOrDefault()?.ChildNodes().OfType<IdentifierNameSyntax>().LastOrDefault();

            if (identifier == null)
                return false;

            var isNullable = (context.SemanticModel.GetSymbolInfo(identifier).Symbol as ISymbol).IsNullable();

            return isSameVariable && isOppositeBooleanValues && !isNullable;
        }

        private AssignmentExpressionSyntax GetBooleanAssignment(SyntaxNode node)
        {
            if (node == null)
                return null;

            var expression = GetSingleStatementInIfOrElseClause(node);
            var assignmentStatement = expression?.ChildNodes().OfType<AssignmentExpressionSyntax>().FirstOrDefault();
            var booleanValue = assignmentStatement?.ChildNodes().OfType<LiteralExpressionSyntax>().FirstOrDefault(l => l.IsAnyKind(SyntaxKind.TrueLiteralExpression, SyntaxKind.FalseLiteralExpression));

            if (booleanValue == null)
                return null;

            return assignmentStatement;
        }

        private ExpressionSyntax GetBooleanReturnStatement(SyntaxNode node)
        {
            if (node == null)
                return null;

            var expression = GetSingleStatementInIfOrElseClause(node);
            var returnValue = (expression as ReturnStatementSyntax)?.Expression;

            if (!returnValue.IsAnyKind(SyntaxKind.TrueLiteralExpression, SyntaxKind.FalseLiteralExpression))
                return null;

            return returnValue;
        }

        private StatementSyntax GetSingleStatementInIfOrElseClause(SyntaxNode node)
        {
            var blockExpressions = node.ChildNodes().OfType<BlockSyntax>().FirstOrDefault()?.ChildNodes().OfType<StatementSyntax>();

            if (blockExpressions == null || blockExpressions.HasMany())
                return node.ChildNodes().OfType<StatementSyntax>()?.FirstOrDefault();

            return blockExpressions.FirstOrDefault();
        }
    }
}
