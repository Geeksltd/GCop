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
    public class TimeSpanFromAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.SimpleMemberAccessExpression;

        private readonly string[] FromMethods =
           {
                "FromYears",
                "FromDays",
                "FromHours",
                "FromMinutes",
                "FromSeconds",
                "FromMilliseconds",
                "FromTicks"
            };

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "120",
                Category = Category.Design,
                Severity = DiagnosticSeverity.Warning,
                Message = "Use {0}.{1}() instead of {2}",
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var expression = context.Node as MemberAccessExpressionSyntax;
            if (expression == null) return;

            var methodName = expression.Name.ToString();

            var isFrom = FromMethods.Contains(methodName);
            if (!isFrom) return;

            var symbolInfo = context.SemanticModel.GetSymbolInfo(expression.Expression, context.CancellationToken);
            if (symbolInfo.Symbol == null) return;
            if (symbolInfo.Symbol.ToString() != "System.TimeSpan") return;

            var invocationExpression = expression.Parent as InvocationExpressionSyntax;
            if (invocationExpression == null) return;

            var argumentExpression = invocationExpression.ArgumentList.Arguments[0].Expression;
            var argumentTypeInfo = context.SemanticModel.GetTypeInfo(argumentExpression, context.CancellationToken);

            if (argumentTypeInfo.Type.Name == "Int32")
            {
                var diagnostic = Diagnostic.Create(
                    Description,
                    expression.GetLocation(),
                    argumentExpression,
                    methodName.Replace("From", newValue: string.Empty),
                    expression.Parent.ToString());

                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}