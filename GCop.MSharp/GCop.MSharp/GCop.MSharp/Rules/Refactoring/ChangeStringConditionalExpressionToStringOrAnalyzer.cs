namespace GCop.MSharp.Rules.Refactoring
{
    using Core;
    using Core.Attributes;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [MSharpExclusive]
    [ZebbleExclusive]
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ChangeStringConditionalExpressionToStringOrAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        SemanticModel SemanticModel;
        protected override SyntaxKind Kind => SyntaxKind.ConditionalExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "642",
                Category = Category.Usage,
                Severity = DiagnosticSeverity.Warning,
                Message = "Replace with {0}.Or({1})"
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var expression = (ConditionalExpressionSyntax)context.Node;
            if (expression.Condition == null || !expression.Condition.IsKind(SyntaxKind.InvocationExpression)) return;

            SemanticModel = context.SemanticModel;

            var whenTrueReturnType = SemanticModel.GetTypeInfo(expression.WhenTrue).Type;
            if (whenTrueReturnType?.ToString() != "string") return;

            var whenFalseReturnType = SemanticModel.GetTypeInfo(expression.WhenFalse).Type;
            if (whenFalseReturnType?.ToString() != "string") return;

            if (!ConditionIsValid(expression.Condition.As<InvocationExpressionSyntax>())) return;

            ReportDiagnostic(context, expression, expression.WhenTrue.ToString(), expression.WhenFalse.ToString());
        }

        private bool ConditionIsValid(InvocationExpressionSyntax condition)
        {
            if (condition == null) return false;

            if (condition.ToString().Lacks("HasValue"))
                return false;

            var conditionExpression = condition.Expression as MemberAccessExpressionSyntax;
            if (conditionExpression == null) return false;

            var identifier = conditionExpression.GetIdentifierSyntax();

            if (identifier == null) return false;

            var typeInfo = SemanticModel.GetTypeInfo(identifier).Type;
            if (typeInfo == null)
            {
                var symbol = SemanticModel.GetSymbolInfo(identifier).Symbol;
                typeInfo = symbol.GetSymbolType();
            }
            if (typeInfo?.ToString() == "string")
                return true;

            return false;
        }

        //private IdentifierNameSyntax GetInnerMostIdentifier(InvocationExpressionSyntax expression)
        //{
        //    var lastMemberAccess = expression.DescendantNodes().OfType<MemberAccessExpressionSyntax>().LastOrDefault();
        //    return lastMemberAccess?.ChildNodes().OfType<IdentifierNameSyntax>().LastOrDefault();
        //}
    }
}
