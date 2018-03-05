namespace GCop.Linq.Rules.Design
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class OrderByAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.InvocationExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "163",
                Category = Category.Design,
                Message = "Subsequent OrderBy or OrderByDescending cancel each other out. Instead ThenBy or ThenByDescending should be called.",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var invocation = context.Node as InvocationExpressionSyntax;
            if (invocation == null) return;

            var memberAccess = invocation.Expression as MemberAccessExpressionSyntax;
            if (memberAccess == null) return;

            var method = context.SemanticModel.GetSymbolInfo(memberAccess).Symbol as IMethodSymbol;
            if (method == null || (method.Name != "OrderBy" && method.Name != "OrderByDescending")) return;

            var previousInvocation = memberAccess.ChildNodes().OfType<InvocationExpressionSyntax>().FirstOrDefault();
            if (previousInvocation != null)
            {
                if (SymbolReturnsOrderedEnumerable(previousInvocation, context.SemanticModel))
                {
                    ReportDiagnostic(context, memberAccess);
                }
            }
            else
            {
                var node = memberAccess.DescendantNodes().OfType<IdentifierNameSyntax>().FirstOrDefault(it => it.Identifier.ValueText != "OrderBy" && it.Identifier.ValueText != "OrderByDescending");
                if (node != null)
                {
                    if (SymbolReturnsOrderedEnumerable(node, context.SemanticModel))
                    {
                        ReportDiagnostic(context, memberAccess);
                    }
                }
                else if (memberAccess.Parent != null)
                {
                    if (SymbolReturnsOrderedEnumerable(memberAccess.Parent, context.SemanticModel))
                    {
                        ReportDiagnostic(context, memberAccess);
                    }
                }
            }
        }

        private bool SymbolReturnsOrderedEnumerable(SyntaxNode node, SemanticModel semanticModel)
        {
            var symbol = semanticModel.GetSymbolInfo(node).Symbol;

            return symbol.Is("IOrderedEnumerable");
        }
    }
}
