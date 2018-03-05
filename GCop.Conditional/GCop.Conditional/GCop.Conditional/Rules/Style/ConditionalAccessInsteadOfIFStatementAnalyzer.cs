namespace GCop.Conditional.Rules.Style
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ConditionalAccessInsteadOfIFStatementAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.IfStatement;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "405",
                Category = Category.Style,
                Severity = DiagnosticSeverity.Warning,
                Message = "You should use {0}"
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var @if = context.Node as IfStatementSyntax;

            if (@if.Else != null) return;

            var notEqualsToNullExpression = @if?.Condition as BinaryExpressionSyntax;
            if (notEqualsToNullExpression == null || notEqualsToNullExpression.Kind() != SyntaxKind.NotEqualsExpression) return;

            if (notEqualsToNullExpression.Right?.Kind() != SyntaxKind.NullLiteralExpression) return;

            //If left-hand side of expression is anything except variable name, rule  should be ignored
            var leftHandIndentifier = notEqualsToNullExpression.Left as IdentifierNameSyntax;
            if (leftHandIndentifier == null) return;

            var leftHandSymbol = context.SemanticModel.GetTypeInfo(leftHandIndentifier).Type;
            if (leftHandSymbol.IsNullable()) return;

            var expressions = @if.Statement.ChildNodes().OfType<StatementSyntax>().ToList();
            if (expressions.None())
            {
                expressions = @if.ChildNodes().OfType<StatementSyntax>().ToList();
            }
            if (expressions.None() || expressions.HasMany()) return;

            var allInvocation = expressions.FirstOrDefault()?.ChildNodes().OfType<InvocationExpressionSyntax>().ToList();
            if (allInvocation.None() || allInvocation.HasMany()) return;

            var invocation = allInvocation.Single();
            if (invocation.Ancestors().OfType<ReturnStatementSyntax>().Any()) return;
            if (invocation.Ancestors().OfType<YieldStatementSyntax>().Any()) return;

            var memberAccess = invocation.Expression as MemberAccessExpressionSyntax;
            if (memberAccess == null) return;

            var identifier = memberAccess.GetIdentifier();
            if (identifier.IsEmpty()) return;

            if (identifier != leftHandIndentifier.Identifier.ValueText) return;

            var methodName = memberAccess.ChildNodes().OfType<IdentifierNameSyntax>().LastOrDefault();
            if (methodName == null) return;
            //If methodInfo is null, it means that the symbol is not method.
            if (context.SemanticModel.GetSymbolInfo(methodName).Symbol is IMethodSymbol methodInfo)
            {
                ReportDiagnostic(context, @if, expressions.First().ToString().Replace(leftHandIndentifier.Identifier.ValueText + ".", leftHandIndentifier.Identifier.ValueText + "?."));
            }
        }
    }
}
