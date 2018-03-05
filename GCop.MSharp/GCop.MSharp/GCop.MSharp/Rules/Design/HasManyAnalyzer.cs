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
    public class HasManyAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.GreaterThanExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "109",
                Message = "Use '{0}.HasMany()' instead of '{1}'",
                Category = Category.Design,
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var be = context.Node as BinaryExpressionSyntax;
            if (be == null || !be.OperatorToken.IsKind(SyntaxKind.GreaterThanToken)) return;

            var memberAccessExpressionSyntax = be.Left?.DescendantNodesAndSelf().OfType<MemberAccessExpressionSyntax>().FirstOrDefault();
            if (memberAccessExpressionSyntax == null) return;

            var methodNameSyntax = memberAccessExpressionSyntax?.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>().LastOrDefault();
            if (methodNameSyntax?.ToString() != "Count" && methodNameSyntax?.ToString() != "Length") return;

            var objectCreationSyntax = memberAccessExpressionSyntax?.DescendantNodesAndSelf().OfType<ObjectCreationExpressionSyntax>().FirstOrDefault();

            if (objectCreationSyntax != null)
            {
                var variable = objectCreationSyntax;
                context.ReportDiagnostic(Diagnostic.Create(Description,
                    memberAccessExpressionSyntax.GetLocation(), variable, be));
            }
            else
            {
                var invocationExpressionSyntax = memberAccessExpressionSyntax.Parent as InvocationExpressionSyntax;
                if (invocationExpressionSyntax?.ArgumentList.Arguments.Count > 0) return;

                var rightExpressionSyntax = be.Right as LiteralExpressionSyntax;
                if ((int?)rightExpressionSyntax?.Token.Value != 1) return;
                var variable = memberAccessExpressionSyntax.DescendantNodes().First();
                context.ReportDiagnostic(Diagnostic.Create(Description, memberAccessExpressionSyntax.GetLocation(), variable, be));
            }
        }
    }
}