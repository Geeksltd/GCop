namespace GCop.Conditional.Rules.Design
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class LongIfConditionAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        string[] ExcludedMethods = new[] { "ValidateProperties" };
        protected override SyntaxKind Kind => SyntaxKind.IfStatement;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "175",
                Category = Category.Design,
                Severity = DiagnosticSeverity.Warning,
                Message = "This condition is very long. Either refactor that into a method, or define interim variables to 'document' your purpose, and use the variable(s) in the IF clause. "
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;

            var ifStatementSyntax = (IfStatementSyntax)NodeToAnalyze;
            if (ifStatementSyntax == null) return;

            var method = ifStatementSyntax.GetSingleAncestor<MethodDeclarationSyntax>();
            if (method?.GetName()?.ToLower().IsAnyOf(ExcludedMethods.ToLower()) ?? true) return;

            // skip the Database.anything inside of if(condition)             

            var allInvocations = ifStatementSyntax.Condition.DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>();

            if (allInvocations.Where(it => it.Expression != null).TrueForAtLeastOnce(it =>
            {
                var methodIdentifier = it.Expression.GetIdentifierSyntax();
                if (methodIdentifier == null) return true;
                var methodInfo = context.SemanticModel.GetSymbolInfo(methodIdentifier).Symbol as INamedTypeSymbol;
                if (methodInfo == null) return true;
                return methodInfo?.Name == "Database" && methodInfo?.ContainingNamespace.ToString() == "MSharp.Framework";
            })) return;

            var logicals = ifStatementSyntax.Condition.DescendantNodesAndSelf().OfType<BinaryExpressionSyntax>()
                .Where(it => it.IsKind(
                    SyntaxKind.LogicalAndExpression,
                    SyntaxKind.LogicalOrExpression,
                    SyntaxKind.LogicalNotExpression
                    ));

            if (logicals.Count() > 4)
            {
                ReportDiagnostic(context, ifStatementSyntax.Condition);
            }
        }
    }
}
