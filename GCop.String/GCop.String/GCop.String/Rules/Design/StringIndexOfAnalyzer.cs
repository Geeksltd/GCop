namespace GCop.String.Rules.Design
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StringIndexOfAnalyzer : GCopAnalyzer
    {
        SemanticModel SemanticModel;
        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "165",
                Category = Category.Design,
                Message = "Instead of .IndexOf({0}) > -1 use .Contains({0}).",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Configure()
        {
            RegisterSyntaxNodeAction(context => Analyze(context), SyntaxKind.GreaterThanExpression, SyntaxKind.EqualsExpression, SyntaxKind.LessThanExpression);
        }

        private void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var expression = context.Node as BinaryExpressionSyntax;
            if (expression == null || expression.Right == null || expression.Left == null) return;

            SemanticModel = context.SemanticModel;

            if (expression.Right.IsKind(SyntaxKind.UnaryMinusExpression) || expression.Right.IsKind(SyntaxKind.NumericLiteralExpression))
            {
                if (IsOtherSideIndexOfMethod(expression.Left))
                    ReportDiagnostic(context, expression.Left, (expression.Left as InvocationExpressionSyntax).ArgumentList.Arguments.FirstOrDefault()?.ToString());
            }
            else if (expression.Left.IsKind(SyntaxKind.NumericLiteralExpression) || expression.Left.IsKind(SyntaxKind.UnaryMinusExpression))
            {
                if (IsOtherSideIndexOfMethod(expression.Right))
                    ReportDiagnostic(context, expression.Right, (expression.Right as InvocationExpressionSyntax).ArgumentList.Arguments.FirstOrDefault()?.ToString());
            }
        }

        private bool IsOtherSideIndexOfMethod(ExpressionSyntax leftSide)
        {
            if (!leftSide.IsKind(SyntaxKind.InvocationExpression)) return false;

            var method = SemanticModel.GetSymbolInfo(leftSide).Symbol as IMethodSymbol;
            if (method == null) return false;
            //                                  method.Parameters.Count()==1
            return method.Name == "IndexOf" && method.Parameters.IsSingle() && method.Parameters.Any(it => it.Type.Name == "String");
        }
    }
}
