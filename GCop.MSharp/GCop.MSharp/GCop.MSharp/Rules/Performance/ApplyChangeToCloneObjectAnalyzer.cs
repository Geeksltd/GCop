namespace GCop.MSharp.Rules.Performance
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ApplyChangeToCloneObjectAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        private readonly string Constructed = "MSharp.Framework.Database.Save<T>(T)";
        protected override SyntaxKind Kind => SyntaxKind.InvocationExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "304",
                Category = Category.Performance,
                Message = "You should clone the object first, then apply the changes to the clone, and then save the cloned variable",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var invocation = (InvocationExpressionSyntax)context.Node;

            var memberAccess = invocation.Expression as MemberAccessExpressionSyntax;
            if (memberAccess == null) return;

            var method = context.SemanticModel.GetSymbolInfo(memberAccess).Symbol as IMethodSymbol;
            if (method == null)
            {
                if (invocation.ArgumentList.ToString().Contains("Clone()"))
                    ReportDiagnostic(context, invocation);
                return;
            }

            if (method.ConstructedFrom.ToString() != Constructed) return;

            if (invocation.ArgumentList.Arguments.Where(argument => argument.Expression is InvocationExpressionSyntax).TrueForAtLeastOnce(argument =>
            {
                if ((argument.Expression as InvocationExpressionSyntax)?.Expression is MemberAccessExpressionSyntax memberAccessExpression)
                {
                    var invokedMethod = context.SemanticModel.GetSymbolInfo(memberAccessExpression).Symbol as IMethodSymbol;
                    return invokedMethod?.Name == "Clone";
                }
                return false;
            }))
            {
                ReportDiagnostic(context, invocation);
            }
        }
    }
}
