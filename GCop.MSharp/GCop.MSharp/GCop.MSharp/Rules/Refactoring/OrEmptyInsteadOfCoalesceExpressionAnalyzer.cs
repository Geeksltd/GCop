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
    public class OrEmptyInsteadOfCoalesceExpressionAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.CoalesceExpression;
        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "641",
                Category = Category.Usage,
                Severity = DiagnosticSeverity.Warning,
                Message = "Use the OrEmpty() method instead."
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var coalesceExpression = context.Node as BinaryExpressionSyntax;

            if (!coalesceExpression.IsKind(SyntaxKind.CoalesceExpression)) return;

            if (coalesceExpression.Right?.ToString().ToLower().IsNoneOf("\"\"", "string.empty") ?? true) return;

            var left = ExtractIdentifier(context.SemanticModel, coalesceExpression.Left);
            if (left == null) return;

            var leftTypeInfo = context.SemanticModel.GetTypeInfo(left).Type;
            if (leftTypeInfo == null)
            {
                var symbol = context.SemanticModel.GetSymbolInfo(left).Symbol;
                leftTypeInfo = symbol.GetSymbolType();
            }

            if (leftTypeInfo?.ToString() != "string") return;

            ReportDiagnostic(context, coalesceExpression);
        }

        private IdentifierNameSyntax ExtractIdentifier(SemanticModel semanticModel, ExpressionSyntax expression)
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

                var typeInfo = semanticModel.GetTypeInfo(memberAccess).Type as ITypeSymbol;
                if (typeInfo?.ToString() != "string") return null;
                identifier = memberAccess.ChildNodes().OfType<IdentifierNameSyntax>().LastOrDefault();
            }
            return identifier;
        }
    }
}
