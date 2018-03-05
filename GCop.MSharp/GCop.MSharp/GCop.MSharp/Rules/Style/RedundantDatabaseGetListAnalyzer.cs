namespace GCop.MSharp.Rules.Style
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RedundantDatabaseGetListAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.MethodDeclaration;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "425",
                Category = Category.Style,
                Severity = DiagnosticSeverity.Warning,
                Message = "This method is unnecessary. The caller can just as well call Database.GetList<{0}>()."
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var method = NodeToAnalyze as MethodDeclarationSyntax;
            //if (method.Modifiers.Any(it => it.IsKind(SyntaxKind.VirtualKeyword) || it.IsKind(SyntaxKind.StaticKeyword))) return;
            InvocationExpressionSyntax firstInvocation = null;

            if (method.Body != null)
            {
                var returnStatement = method.Body.ChildNodes().FirstOrDefault() as ReturnStatementSyntax;
                if (returnStatement == null) return;
                firstInvocation = returnStatement.ChildNodes().FirstOrDefault() as InvocationExpressionSyntax;
                if (firstInvocation == null) return;

                if (method.Body.DescendantTrivia().Any(it => it.Kind() == SyntaxKind.SingleLineCommentTrivia || it.Kind() == SyntaxKind.MultiLineCommentTrivia)) return;

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
            if (invocation == null || invocation.ArgumentList.Arguments.Any()) return false;
            var method = semanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
            return method?.ToString() == "MSharp.Framework.Database.GetList<";
        }
    }
}
