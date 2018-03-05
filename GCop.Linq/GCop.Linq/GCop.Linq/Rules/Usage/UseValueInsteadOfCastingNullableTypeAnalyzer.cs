namespace GCop.Linq.Rules.Usage
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UseValueInsteadOfCastingNullableTypeAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.CastExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "512",
                Category = Category.Usage,
                Severity = DiagnosticSeverity.Warning,
                Message = "Use \"{0}.Value\" instead."
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var expression = NodeToAnalyze as CastExpressionSyntax;

            if (expression == null)
                return;

            var identifier = expression.ChildNodes().OfType<IdentifierNameSyntax>()?.LastOrDefault();

            if (identifier == null)
                return;

            var symbol = context.SemanticModel.GetSymbolInfo(identifier).Symbol as ISymbol;

            if (symbol == null || !symbol.IsNullable())
                return;

            if (symbol.GetSymbolType().ToString().TrimEnd("?") == expression.Type.ToString())
                ReportDiagnostic(context, expression, identifier.Identifier.ValueText);
        }
    }
}
