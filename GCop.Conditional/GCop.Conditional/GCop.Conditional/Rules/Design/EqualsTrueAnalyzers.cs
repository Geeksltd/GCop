namespace GCop.Conditional.Rules.Design
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EqualsTrueAnalyzers : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.EqualsExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "161",
                Category = Category.Design,
                Severity = DiagnosticSeverity.Warning,
                Message = "Since the {0} side of condition is just Bool, then \" == true\" is unnecessary"
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var equalsClause = context.Node as BinaryExpressionSyntax;
            if (equalsClause == null) return;

            SyntaxNode nodeToWarn = null;
            ExpressionSyntax otherSide = null;
            var sideName = "left";

            if (equalsClause.Left.IsKind(SyntaxKind.TrueLiteralExpression))
            {
                sideName = "right";
                nodeToWarn = equalsClause.Left;
                otherSide = equalsClause.Right;
            }
            else if (equalsClause.Right.IsKind(SyntaxKind.TrueLiteralExpression))
            {
                nodeToWarn = equalsClause.Right;
                otherSide = equalsClause.Left;
            }

            if (otherSide == null) return;

            var symbol = context.SemanticModel.GetSymbolInfo(otherSide).Symbol;
            if (symbol == null) return;

            if (symbol is IMethodSymbol)
            {
                if ((symbol as IMethodSymbol).ReturnType.Name == "Boolean")
                {
                    Report(context, nodeToWarn, sideName);
                }
            }

            if (symbol is IPropertySymbol)
            {
                if ((symbol as IPropertySymbol).Type.Name == "Boolean")
                {
                    Report(context, nodeToWarn, sideName);
                }
            }

            if (symbol is IFieldSymbol)
            {
                if ((symbol as IFieldSymbol).Type.Name == "Boolean")
                {
                    Report(context, nodeToWarn, sideName);
                }
            }

            if (symbol is ILocalSymbol)
            {
                if ((symbol as ILocalSymbol).Type.Name == "Boolean")
                {
                    Report(context, nodeToWarn, sideName);
                }
            }
        }
        void Report(SyntaxNodeAnalysisContext context, SyntaxNode nodeToWarn, string sideName) => ReportDiagnostic(context, nodeToWarn.GetLocation(), sideName);
    }
}
