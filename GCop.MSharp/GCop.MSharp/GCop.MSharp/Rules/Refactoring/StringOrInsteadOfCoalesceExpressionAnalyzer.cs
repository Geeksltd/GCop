namespace GCop.MSharp.Rules.Refactoring
{
    using Core;
    using Core.Attributes;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [MSharpExclusive]
    [ZebbleExclusive]
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StringOrInsteadOfCoalesceExpressionAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.CoalesceExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "646",
                Category = Category.Usage,
                Severity = DiagnosticSeverity.Warning,
                Message = "Use the Or() method instead, so the empty string case is also replaced."
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var coalesceExpression = context.Node as BinaryExpressionSyntax;
            if (coalesceExpression.Right == null) return;

            if (coalesceExpression.Right?.ToString().ToLower().IsAnyOf("\"\"", "string.empty") ?? true) return;

            var left = ExtractIdentifier(coalesceExpression.Left);
            if (left == null) return;

            //We are supposed to look at string types
            var leftTypeInfo = context.SemanticModel.GetTypeInfo(left).Type;
            if (leftTypeInfo?.ToString() != "string") return;

            if (coalesceExpression.Right.Kind() == SyntaxKind.StringLiteralExpression)
            {
                ////We have another rule to handle this kind of condition
                //if ((coalesceExpression.Right as LiteralExpressionSyntax)?.Token.ToString() == "\"\"") return;
                ReportDiagnostic(context, coalesceExpression);
                return;
            }

            var right = ExtractIdentifier(coalesceExpression.Right);
            if (right == null) return;

            var rightTypeInfo = context.SemanticModel.GetTypeInfo(right).Type;
            if (rightTypeInfo == null)
            {
                var symbol = context.SemanticModel.GetSymbolInfo(right).Symbol;
                rightTypeInfo = symbol.GetSymbolType();
            }

            if (rightTypeInfo?.ToString() != "string") return;

            ReportDiagnostic(context, coalesceExpression);
        }

        private IdentifierNameSyntax ExtractIdentifier(ExpressionSyntax expression)
        {
            if (expression == null) return null;

            var identifier = expression as IdentifierNameSyntax;
            if (identifier == null)
            {
                MemberAccessExpressionSyntax memberAccess = null;
                if (expression.Kind() == SyntaxKind.InvocationExpression)
                {
                    memberAccess = (expression as InvocationExpressionSyntax).Expression as MemberAccessExpressionSyntax;
                }
                else
                {
                    memberAccess = expression as MemberAccessExpressionSyntax;
                }

                if (memberAccess == null) return null;

                //We have another rule to handle this kind of condition, So definitely we have to ignore the rule 
                if (memberAccess.ToString().IsAnyOf("string.Empty", "String.Empty")) return null;

                identifier = memberAccess.ChildNodes().OfType<IdentifierNameSyntax>().LastOrDefault();
            }

            return identifier;
        }
    }
}
