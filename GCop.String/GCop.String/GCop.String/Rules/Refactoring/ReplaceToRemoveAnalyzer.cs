namespace GCop.String.Rules.Refactoring
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ReplaceToRemoveAnalyzer : GCopAnalyzer
    {
        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "648",
                Category = Category.Refactoring,
                Severity = DiagnosticSeverity.Warning,
                Message = "Use {0}.Remove({1}) instead"
            };
        }

        protected override void Configure()
        {
            RegisterSyntaxNodeAction(AnalyzeExpression, SyntaxKind.InvocationExpression);
        }

        void AnalyzeExpression(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var invocation = NodeToAnalyze as InvocationExpressionSyntax;

            if (invocation.ArgumentList == null) return;
            if (invocation.ArgumentList.Arguments == null) return;
            if (invocation.ArgumentList.Arguments.None()) return;
            if (invocation.ArgumentList.Arguments.Count != 2) return;

            var memberAccess = invocation.Expression as MemberAccessExpressionSyntax;
            if (memberAccess == null) return;

            var identifierSysntax = memberAccess.GetIdentifierSyntax();
            if (identifierSysntax == null) return;

            var type = context.SemanticModel.GetTypeInfo(identifierSysntax).Type as ITypeSymbol;
            if (type == null) return;

            if (type.Name != "String") return;

            if (memberAccess.Name.ToString() != "Replace") return;

            var firstElement = invocation.ArgumentList.Arguments.FirstOrDefault()?.Expression;
            if (firstElement == null) return;

            var firstArgumentType = context.SemanticModel.GetTypeInfo(firstElement).Type as ITypeSymbol;
            if (firstArgumentType == null) return;

            if (firstArgumentType.Name != "String") return;

            var secondElement = invocation.ArgumentList.Arguments.LastOrDefault();
            if (secondElement == null) return;
            if (secondElement.Expression == null) return;

            if (
                secondElement.Expression.Kind() == SyntaxKind.NullLiteralExpression ||
                secondElement.ToString().IsAnyOf("\"\"", "string.Empty"))

                ReportDiagnostic(context, invocation, new string[] { memberAccess.GetIdentifier(), firstElement.ToString() });
        }
    }
}
