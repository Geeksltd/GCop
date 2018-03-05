namespace GCop.Linq.Rules.Style
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CountRedundantCallerAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.MethodDeclaration;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "424",
                Category = Category.Style,
                Severity = DiagnosticSeverity.Warning,
                Message = "This method is redundant. Callers of this method can just call {0}.Count() which is as clean."
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var method = NodeToAnalyze as MethodDeclarationSyntax;
            if (method.Modifiers.Any(it => it.IsKind(SyntaxKind.VirtualKeyword) || it.IsKind(SyntaxKind.StaticKeyword))) return;
            InvocationExpressionSyntax firstInvocation = null;

            if (method.Body != null)
            {
                var returnStatement = method.Body.ChildNodes().FirstOrDefault() as ReturnStatementSyntax;
                if (returnStatement == null) return;
                firstInvocation = returnStatement.ChildNodes().FirstOrDefault() as InvocationExpressionSyntax;
                if (firstInvocation == null) return;

                if (IsJustReturningCount(firstInvocation, context.SemanticModel))
                {
                    ReportDiagnostic(context, method.Identifier, firstInvocation.ToString().Remove(".Count()"));
                }
                return;
            }

            if (method.ExpressionBody == null) return;
            firstInvocation = method.ExpressionBody.Expression as InvocationExpressionSyntax;
            if (!IsJustReturningCount(firstInvocation, context.SemanticModel)) return;

            ReportDiagnostic(context, method.Identifier, firstInvocation.ToString().Remove(".Count()"));
        }

        bool IsJustReturningCount(InvocationExpressionSyntax invocation, SemanticModel semanticModel)
        {
            if (invocation == null) return false;
            //If any parameter is provided to Count(), the rule should be skipped. 
            if (invocation.ArgumentList.Arguments.Any()) return false;

            var memberAccessExpression = invocation.Expression as MemberAccessExpressionSyntax;
            if (memberAccessExpression == null) return false;
            var propertyIdentifier = memberAccessExpression.GetIdentifierSyntax();
            if (propertyIdentifier == null) return false;
            var property = semanticModel.GetSymbolInfo(propertyIdentifier).Symbol as IPropertySymbol;
            if (property == null || property.DeclaredAccessibility != Accessibility.Public) return false;
            var method = semanticModel.GetSymbolInfo(memberAccessExpression).Symbol as IMethodSymbol;

            return method != null && method.Name == "Count" && method.ReturnType.Name.IsAnyOf("Int32", "Int64");
        }
    }
}
