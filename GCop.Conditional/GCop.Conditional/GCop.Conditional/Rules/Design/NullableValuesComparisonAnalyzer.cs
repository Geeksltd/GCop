namespace GCop.Conditional.Rules.Design
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NullableValuesComparisonAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.LogicalAndExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "169",
                Category = Category.Design,
                Message = "Remove {0} which is redundant. If {0} is null, then '{2}' will be false anyway.",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var expression = context.Node as BinaryExpressionSyntax;
            if (expression == null || !expression.IsKind(SyntaxKind.LogicalAndExpression)) return;

            if (expression.Left?.Kind() != SyntaxKind.SimpleMemberAccessExpression && expression.Left?.Kind() != SyntaxKind.NotEqualsExpression) return;

            bool expressionIsNotEquals = false;

            ExpressionSyntax invocationOfHasValue = null;
            if (expression.Left.Kind() == SyntaxKind.NotEqualsExpression && expression.Left.ChildNodes().Any(it => it.IsKind(SyntaxKind.NullLiteralExpression)))
            {
                expressionIsNotEquals = true;
                invocationOfHasValue = expression.Left;
            }
            else
                invocationOfHasValue = expression.Left as MemberAccessExpressionSyntax;

            if (invocationOfHasValue == null) return;

            string variableName = null;
            if (invocationOfHasValue.ChildNodes().OfType<MemberAccessExpressionSyntax>().Any())
                variableName = invocationOfHasValue.ChildNodes().OfType<MemberAccessExpressionSyntax>().LastOrDefault()?.GetIdentifier();
            else
                variableName = invocationOfHasValue.GetIdentifier();

            if (variableName.IsEmpty()) return;

            if (expressionIsNotEquals)
            {
                var identifier = invocationOfHasValue.GetIdentifierSyntax();
                if (identifier == null)
                    return;
                var property = context.SemanticModel.GetSymbolInfo(identifier).Symbol;
                if (property != null && !property.IsNullable())
                    return;
            }

            var propertyInfo = context.SemanticModel.GetSymbolInfo(invocationOfHasValue).Symbol as IPropertySymbol;
            var expressionIsHasValue = propertyInfo?.Name == "HasValue" && (propertyInfo?.ContainingType.ToString().IsAnyOf("int?", "double?", "decimal?", "System.DateTime?", "bool?") ?? false);

            if (expressionIsNotEquals == false && expressionIsHasValue == false) return;

            var rightHand = expression.Right as BinaryExpressionSyntax;
            if (rightHand == null) return;

            ExpressionSyntax leftHand = null;

            //Because of some cases just like this.XXX we need to check for MemeberAccessExpression.
            leftHand = rightHand.Left.DescendantNodesAndSelf().OfType<MemberAccessExpressionSyntax>().FirstOrDefault();
            if (leftHand == null)
                leftHand = rightHand.Left as IdentifierNameSyntax;

            //It means the second condition is related to HasValue property 
            if (leftHand?.GetIdentifier() != variableName) return;


            //to handle cases like (x.HasValue && x == y) when both x and y are nullable
            if (rightHand.Right is IdentifierNameSyntax secondConditionRightHand)
            {
                var typeInfo = context.SemanticModel.GetTypeInfo(secondConditionRightHand).Type;
                if (typeInfo.IsClassOrNullable())
                    return;
            }
            else
            {
                //to handle case like (x.HasValue && x == something.y)
                secondConditionRightHand = (rightHand.Right as MemberAccessExpressionSyntax)?.ChildNodes().OfType<IdentifierNameSyntax>().LastOrDefault();
                if (secondConditionRightHand != null)
                {
                    var typeInfo = context.SemanticModel.GetTypeInfo(secondConditionRightHand).Type;
                    if (typeInfo.IsClassOrNullable())
                        return;
                }
                else
                {
                    //to handle case like (x.HasValue && x == Something())
                    var rightHandInvocatoin = rightHand.Right as InvocationExpressionSyntax;
                    if (rightHandInvocatoin == null)
                        return;

                    var typeInfo = context.SemanticModel.GetTypeInfo(rightHandInvocatoin).Type;
                    if (typeInfo.IsClassOrNullable()) return;
                }
            }
            var message = expressionIsNotEquals ? variableName + " != null" : variableName + ".HasValue";
            ReportDiagnostic(context, invocationOfHasValue, message, variableName, expression.Right.ToString());
        }
    }
}
