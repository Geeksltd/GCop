namespace GCop.Conditional.Rules.Refactoring
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Collections.Immutable;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ConditionalReductionAnalyzer : GCopAnalyzer
    {
        internal const string IsNullCoalescingKey = "isNullCoalescing";
        private readonly SyntaxKind[] EqualsOrNotEquals = { SyntaxKind.EqualsExpression, SyntaxKind.NotEqualsExpression };
        private static readonly ExpressionSyntax NullExpression = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);

        private readonly int MaxCharacters = 80;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "620",
                Category = Category.Refactoring,
                Severity = DiagnosticSeverity.Warning,
                Message = "To simplify the condition use the \"{0}\"."
            };
        }

        protected override void Configure()
        {
            RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
            RegisterSyntaxNodeAction(AnalyzeConditionalExpression, SyntaxKind.ConditionalExpression);
        }

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            var ifStatement = (IfStatementSyntax)context.Node;
            if (ifStatement.Else == null ||
                ifStatement.Parent is ElseClauseSyntax)
            {
                return;
            }

            var whenTrue = ExtractSingleStatement(ifStatement.Statement);
            var whenFalse = ExtractSingleStatement(ifStatement.Else.Statement);

            if (whenTrue == null || whenFalse == null || whenTrue.IsEquivalent(whenFalse))
            {
                //<see cref="ConditionalStructureSameImplementation"/>
                return;
            }

            if (whenTrue.ToString().Length > MaxCharacters || whenFalse.ToString().Length > MaxCharacters) return;

            var getExpression = TryGetExpressionComparedToNull(ifStatement.Condition);
            var possiblyNullCoalescing = getExpression.ReturnValue &&
                ExpressionCanBeNull(getExpression.Compared, context.SemanticModel);

            var simplify = CanBeSimplified(whenTrue, whenFalse, possiblyNullCoalescing ? getExpression.Compared : null, context.SemanticModel, getExpression.ComparedIsNullInTrue);
            if (simplify.ReturnValue)
            {
                if (!simplify.IsNullCoalescing) return;
                context.ReportDiagnostic(
                    Diagnostic.Create(Description, ifStatement.IfKeyword.GetLocation(),
                    ImmutableDictionary<string, string>.Empty.Add(IsNullCoalescingKey,
                    simplify.IsNullCoalescing.ToString()), simplify.IsNullCoalescing ? "??" : "?:"));
            }
        }

        private void AnalyzeConditionalExpression(SyntaxNodeAnalysisContext context)
        {
            var conditional = (ConditionalExpressionSyntax)context.Node;

            var condition = RemoveParentheses(conditional.Condition);
            var whenTrue = RemoveParentheses(conditional.WhenTrue);
            var whenFalse = RemoveParentheses(conditional.WhenFalse);

            if (whenTrue.IsEquivalent(whenFalse))
            {
                ///<see cref="TernaryOperatorPointless"/>
                return;
            }

            var getExpression = TryGetExpressionComparedToNull(condition);
            if (!getExpression.ReturnValue || !ExpressionCanBeNull(getExpression.Compared, context.SemanticModel))
            {
                // expression not compared to null, or can't be null
                return;
            }

            if (CanExpressionBeNullCoalescing(whenTrue, whenFalse, getExpression.Compared, context.SemanticModel, getExpression.ComparedIsNullInTrue))
            {
                ReportDiagnostic(context, conditional.GetLocation(), "??");
            }
        }

        private bool AreTypesCompatible(ExpressionSyntax expression1, ExpressionSyntax expression2, SemanticModel semanticModel)
        {
            var type1 = semanticModel.GetTypeInfo(expression1).Type;
            var type2 = semanticModel.GetTypeInfo(expression2).Type;

            if (type1 is IErrorTypeSymbol || type2 is IErrorTypeSymbol)
            {
                return false;
            }

            if (type1 == null || type2 == null)
            {
                return true;
            }

            return type1.Equals(type2);
        }

        private Simplify CanBeSimplified(StatementSyntax statement1, StatementSyntax statement2,
            ExpressionSyntax comparedToNull,
             SemanticModel semanticModel,
            bool comparedIsNullInTrue)
        {
            var simplify = new Simplify
            {
                IsNullCoalescing = false
            };
            var return1 = statement1 as ReturnStatementSyntax;
            var return2 = statement2 as ReturnStatementSyntax;

            if (return1 != null && return2 != null)
            {
                var retExpr1 = RemoveParentheses(return1.Expression);
                var retExpr2 = RemoveParentheses(return2.Expression);

                if (!AreTypesCompatible(return1.Expression, return2.Expression, semanticModel))
                {
                    simplify.ReturnValue = false;
                }

                if (comparedToNull != null &&
                    CanExpressionBeNullCoalescing(retExpr1, retExpr2, comparedToNull, semanticModel, comparedIsNullInTrue))
                {
                    simplify.IsNullCoalescing = true;
                }
                simplify.ReturnValue = true;
            }

            var expressionStatement1 = statement1 as ExpressionStatementSyntax;
            var expressionStatement2 = statement2 as ExpressionStatementSyntax;

            if (expressionStatement1 == null || expressionStatement2 == null)
            {
                simplify.ReturnValue = false;
            }

            var expression1 = RemoveParentheses(expressionStatement1.Expression);
            var expression2 = RemoveParentheses(expressionStatement2.Expression);

            var candidateSimplify = AreCandidateAssignments(expression1, expression2, comparedToNull,
                    semanticModel, comparedIsNullInTrue);
            if (candidateSimplify.ReturnValue)
            {
                simplify.IsNullCoalescing = candidateSimplify.IsNullCoalescing;
                simplify.ReturnValue = true;
            }

            if (comparedToNull != null &&
                CanExpressionBeNullCoalescing(expression1, expression2, comparedToNull, semanticModel, comparedIsNullInTrue))
            {
                simplify.IsNullCoalescing = true;
                simplify.ReturnValue = true;
            }

            if (AreCandidateInvocationsForTernary(expression1, expression2, semanticModel))
            {
                simplify.ReturnValue = true;
            }

            return simplify;
        }

        private Simplify AreCandidateAssignments(ExpressionSyntax expression1, ExpressionSyntax expression2,
            ExpressionSyntax compared, SemanticModel semanticModel, bool comparedIsNullInTrue)
        {
            var simplify = new Simplify
            {
                IsNullCoalescing = false,
                ReturnValue = true
            };
            var assignment1 = expression1 as AssignmentExpressionSyntax;
            var assignment2 = expression2 as AssignmentExpressionSyntax;
            var canBeSimplified =
                assignment1 != null &&
                assignment2 != null &&
                assignment1.Left.IsEquivalent(assignment2.Left) &&
                assignment1.Kind() == assignment2.Kind();

            if (!canBeSimplified)
            {
                simplify.ReturnValue = false;
            }

            if (!AreTypesCompatible(assignment1.Right, assignment2.Right, semanticModel))
            {
                simplify.ReturnValue = false;
            }

            if (compared != null &&
                CanExpressionBeNullCoalescing(assignment1.Right, assignment2.Right, compared, semanticModel, comparedIsNullInTrue))
            {
                simplify.IsNullCoalescing = true;
            }

            return simplify;
        }

        private StatementSyntax ExtractSingleStatement(StatementSyntax statement)
        {
            if (statement is BlockSyntax block)
            {
                if (block.Statements.Count != 1)
                {
                    return null;
                }
                return block.Statements.First();
            }

            return statement;
        }

        private bool AreCandidateInvocationsForNullCoalescing(ExpressionSyntax expression1, ExpressionSyntax expression2,
            ExpressionSyntax comparedToNull, SemanticModel semanticModel, bool comparedIsNullInTrue)
        {
            return AreCandidateInvocations(expression1, expression2, comparedToNull, semanticModel, comparedIsNullInTrue);
        }

        private bool AreCandidateInvocationsForTernary(ExpressionSyntax expression1, ExpressionSyntax expression2,
            SemanticModel semanticModel)
        {
            return AreCandidateInvocations(expression1, expression2, null, comparedIsNullInTrue: false, semanticModel: semanticModel);
        }

        private bool AreCandidateInvocations(ExpressionSyntax expression1, ExpressionSyntax expression2, ExpressionSyntax comparedToNull, SemanticModel semanticModel, bool comparedIsNullInTrue)
        {
            var methodCall1 = expression1 as InvocationExpressionSyntax;
            var methodCall2 = expression2 as InvocationExpressionSyntax;

            if (methodCall1 == null || methodCall2 == null)
            {
                return false;
            }

            var methodSymbol1 = semanticModel.GetSymbolInfo(methodCall1).Symbol;
            var methodSymbol2 = semanticModel.GetSymbolInfo(methodCall2).Symbol;

            if (methodSymbol1 == null ||
                methodSymbol2 == null ||
                !methodSymbol1.Equals(methodSymbol2))
            {
                return false;
            }

            if (methodCall1.ArgumentList == null ||
                methodCall2.ArgumentList == null ||
                methodCall1.ArgumentList.Arguments.Count != methodCall2.ArgumentList.Arguments.Count)
            {
                return false;
            }

            var numberOfDifferences = 0;
            var numberOfComparisonsToCondition = 0;
            for (int i = 0; i < methodCall1.ArgumentList.Arguments.Count; i++)
            {
                var arg1 = methodCall1.ArgumentList.Arguments[i];
                var arg2 = methodCall2.ArgumentList.Arguments[i];

                if (!arg1.Expression.IsEquivalent(arg2.Expression))
                {
                    numberOfDifferences++;

                    if (comparedToNull != null)
                    {
                        var arg1IsComparedToNull = arg1.Expression.IsEquivalent(comparedToNull);
                        var arg2IsComparedToNull = arg2.Expression.IsEquivalent(comparedToNull);

                        if (arg1IsComparedToNull && !comparedIsNullInTrue)
                        {
                            numberOfComparisonsToCondition++;
                        }

                        if (arg2IsComparedToNull && comparedIsNullInTrue)
                        {
                            numberOfComparisonsToCondition++;
                        }

                        if (!AreTypesCompatible(arg1.Expression, arg2.Expression, semanticModel))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    if (comparedToNull != null && arg1.Expression.IsEquivalent(comparedToNull))
                    {
                        return false;
                    }
                }
            }

            return numberOfDifferences == 1 && (comparedToNull == null || numberOfComparisonsToCondition == 1);
        }

        private bool CanExpressionBeNullCoalescing(ExpressionSyntax whenTrue, ExpressionSyntax whenFalse, ExpressionSyntax comparedToNull, SemanticModel semanticModel, bool comparedIsNullInTrue)
        {
            if (whenTrue.IsEquivalent(comparedToNull))
            {
                return !comparedIsNullInTrue;
            }

            if (whenFalse.IsEquivalent(comparedToNull))
            {
                return comparedIsNullInTrue;
            }

            return AreCandidateInvocationsForNullCoalescing(whenTrue, whenFalse, comparedToNull, semanticModel, comparedIsNullInTrue);
        }

        private GetExpression TryGetExpressionComparedToNull(ExpressionSyntax expression)
        {
            var getExpression = new GetExpression
            {
                Compared = null,
                ComparedIsNullInTrue = false,
                ReturnValue = true
            };
            var binary = expression as BinaryExpressionSyntax;
            if (binary == null || EqualsOrNotEquals.Lacks(binary.Kind()))
            {
                getExpression.ReturnValue = false;
            }

            getExpression.ComparedIsNullInTrue = binary.IsKind(SyntaxKind.EqualsExpression);

            if (binary.Left.IsEquivalent(NullExpression))
            {
                getExpression.Compared = binary.Right;
                getExpression.ReturnValue = true;
            }

            if (binary.Right.IsEquivalent(NullExpression))
            {
                getExpression.Compared = binary.Left;
                getExpression.ReturnValue = true;
            }

            return getExpression;
        }

        private bool ExpressionCanBeNull(ExpressionSyntax expression, SemanticModel semanticModel)
        {
            var expressionType = semanticModel.GetTypeInfo(expression).Type;
            return expressionType != null &&
                   (expressionType.IsReferenceType ||
                    expressionType.SpecialType == SpecialType.System_Nullable_T);
        }

        private ExpressionSyntax RemoveParentheses(ExpressionSyntax expression)
        {
            var currentExpression = expression;
            var parentheses = expression as ParenthesizedExpressionSyntax;
            while (parentheses != null)
            {
                currentExpression = parentheses.Expression;
                parentheses = currentExpression as ParenthesizedExpressionSyntax;
            }
            return currentExpression;
        }
    }

    class Simplify
    {
        public bool ReturnValue { get; set; }
        public bool IsNullCoalescing { get; set; }
    }

    class GetExpression
    {
        public bool ReturnValue { get; set; }
        public ExpressionSyntax Compared { get; set; }
        public bool ComparedIsNullInTrue { get; set; }
    }
}
