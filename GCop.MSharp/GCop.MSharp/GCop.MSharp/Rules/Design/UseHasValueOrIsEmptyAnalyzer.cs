namespace GCop.MSharp.Rules.Design
{
    using Core;
    using Core.Attributes;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System;
    using System.Linq;

    [MSharpExclusive]
    [ZebbleExclusive]
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UseHasValueOrIsEmptyAnalyzer : GCopAnalyzer
    {
        private bool IsNullComparison = false;
        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "126",
                Category = Category.Design,
                Message = "To handle both null and empty string scenarios, use IsEmpty/HasValue instead{0}",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Configure()
        {
            RegisterSyntaxNodeAction(a => AnalyzeInvocation(a), SyntaxKind.InvocationExpression);
            RegisterSyntaxNodeAction(a => AnalyzeEquals(a), SyntaxKind.EqualsExpression, SyntaxKind.NotEqualsExpression);
        }

        private void AnalyzeEquals(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var binary = (BinaryExpressionSyntax)context.Node;
            var identifier = binary.Left as IdentifierNameSyntax;
            var expression = binary.Right;

            IsNullComparison = false;

            if (identifier == null)
            {
                identifier = binary.Right as IdentifierNameSyntax;
                expression = binary.Left;
            }

            if (identifier == null || expression == null) return;

            if (IsInvalidCondition(context.SemanticModel, identifier, expression))
            {
                if (IsNullComparison)
                    ReportDiagnostic(context, binary.GetLocation(), $".If your logic applies to null, but not empty string, then change the condition to ReferenceEquals({identifier?.ToString().Or("variable")}, null) instead");
                else
                    ReportDiagnostic(context, binary.GetLocation(), string.Empty);
            }
        }

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var invocation = (InvocationExpressionSyntax)context.Node;
            var identifier = invocation.ArgumentList?.Arguments.FirstOrDefault()?.ChildNodes().OfType<IdentifierNameSyntax>().FirstOrDefault();
            var expression = invocation.Expression;

            if (expression == null) return;

            IsNullComparison = false;

            if (IsInvalidCondition(context.SemanticModel, identifier, expression))
            {
                if (IsNullComparison)
                    ReportDiagnostic(context, invocation.GetLocation(), $".If your logic applies to null, but not empty string, then change the condition to ReferenceEquals({identifier?.ToString().Or("variable")}, null) instead");
                else
                    ReportDiagnostic(context, invocation.GetLocation(), string.Empty);
            }
        }

        private bool IsInvalidCondition(SemanticModel model, IdentifierNameSyntax identifier, ExpressionSyntax expression)
        {
            if (identifier != null)
            {
                var symbol = model.GetDeclaredSymbol(identifier);

                if (symbol == null)
                {
                    symbol = model.GetSymbolInfo(identifier).Symbol;
                }

                // get type from symbol
                var type = (symbol as ILocalSymbol)?.Type ?? (symbol as IParameterSymbol)?.Type ?? (symbol as IFieldSymbol)?.Type ?? (symbol as IPropertySymbol)?.Type;

                if (type == null || type.Name != "String")
                {
                    // we only look at types that are string.
                    return false;
                }
            }

            var expressionText = (expression as LiteralExpressionSyntax)?.Token.ValueText ?? expression.ToString();

            if (expression is LiteralExpressionSyntax)
            {
                if (expression.Kind() == SyntaxKind.NullLiteralExpression && expressionText == "null")
                {
                    IsNullComparison = true;
                    return true;
                }
                else if (expression.Kind() == SyntaxKind.StringLiteralExpression && expressionText.IsEmpty()) return true;
            }
            else if (expressionText.IsAnyOf("string.Empty", "string.IsNullOrEmpty", "string.IsNullOrWhiteSpace", "String.IsNullOrEmpty", "String.IsNullOrWhiteSpace"))
            {
                return true;
            }

            return false;
        }
    }
}
