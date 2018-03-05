namespace GCop.MSharp.Rules.Style
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CompareEntityDirectlyWithAnIdAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.EqualsExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "429",
                Category = Category.Style,
                Severity = DiagnosticSeverity.Warning,
                Message = "Change to {0} == {1} as you can compare Guid with Entity directly. It handles null too."
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var equalsExpression = NodeToAnalyze as BinaryExpressionSyntax;

            var rightHand = equalsExpression.Right as MemberAccessExpressionSyntax;
            if (rightHand == null) return;
            if (!equalsExpression.Left.ToString().EndsWith("Id")) return;

            var variable = rightHand.DescendantNodesAndSelf().OfType<MemberAccessExpressionSyntax>().LastOrDefault()?.GetIdentifierSyntax();
            var idProperty = rightHand.DescendantNodesAndSelf().OfType<MemberAccessExpressionSyntax>().FirstOrDefault()?.ChildNodes().OfType<IdentifierNameSyntax>().LastOrDefault();
            if (variable == null || idProperty == null || idProperty.ToString() != "ID") return;

            ISymbol variableInfo = null;

            variableInfo = context.SemanticModel.GetSymbolInfo(variable).Symbol as ILocalSymbol;
            variableInfo = variableInfo ?? context.SemanticModel.GetSymbolInfo(variable).Symbol as IParameterSymbol;
            if (variableInfo == null) return;

            ReportDiagnostic(context, rightHand, equalsExpression.Left.ToString(), variable.ToString());
        }
    }
}
