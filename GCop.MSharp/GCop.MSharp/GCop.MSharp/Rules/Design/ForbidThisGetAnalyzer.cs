namespace GCop.MSharp.Rules.Design
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
    public class ForbidThisGetAnalyzer : GCopAnalyzer
    {
        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "107",
                Category = Category.Design,
                Message = "Do not use {0}. 'this' is never null",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Configure()
        {
            RegisterSyntaxNodeAction(context => Analyze(context), SyntaxKind.SimpleMemberAccessExpression, SyntaxKind.ConditionalAccessExpression);
        }

        protected void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;

            if (context.Node is MemberAccessExpressionSyntax memberAccessNode)
            {
                var isThisKeyword = memberAccessNode.Expression?.IsKind(SyntaxKind.ThisExpression) ?? false;

                if (!isThisKeyword)
                    return;

                var isGetMethod = (memberAccessNode.Name as IdentifierNameSyntax)?.Identifier.ValueText == "Get";

                if (isGetMethod)
                {
                    ReportDiagnostic(context, memberAccessNode.GetLocation(), "this.Get()");
                }
            }
            else
            {
                var conditionalAccessExpression = context.Node as ConditionalAccessExpressionSyntax;
                if (conditionalAccessExpression == null) return;

                if (conditionalAccessExpression.ChildNodes().OfType<ThisExpressionSyntax>().Any())
                {
                    ReportDiagnostic(context, conditionalAccessExpression.OperatorToken, $"this?{conditionalAccessExpression.WhenNotNull?.ToString()}");
                }
            }
        }
    }
}
