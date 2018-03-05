namespace GCop.Conditional.Rules.Refactoring
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NullableBoolExpressionAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.IfStatement;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "627",
                Category = Category.Refactoring,
                Severity = DiagnosticSeverity.Warning,
                Message = "{0}"
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var ifStatement = context.Node as IfStatementSyntax;
            var condition = ifStatement.Condition.ToString();

            if (condition.KeepReplacing(" ", "").Contains("??false"))
            {
                var coalescNode = ifStatement.ChildNodes().OfKind(SyntaxKind.CoalesceExpression).FirstOrDefault();
                if (coalescNode == null) return;

                var falseNode = coalescNode.ChildNodes().OfKind(SyntaxKind.FalseLiteralExpression).FirstOrDefault();
                if (falseNode == null) return;


                ReportDiagnostic(context, coalescNode.As<BinaryExpressionSyntax>().OperatorToken, "Instead of « ?? false » use the more readable expression of « == true »");
            }

            if (condition.KeepReplacing(" ", "").Lacks("==true")) return;
            // if (condition.ReplaceAll(" ", "").Contains("==true")){}

            var equalExpre = ifStatement.ChildNodes().OfKind(SyntaxKind.EqualsExpression).FirstOrDefault();
            if (equalExpre == null) return;

            var notOperandat = equalExpre.ChildNodes().FirstOrDefault();
            if (notOperandat == null) return;

            if (notOperandat.Kind() == SyntaxKind.LogicalNotExpression)
                ReportDiagnostic(context, notOperandat.GetIdentifierSyntax(), "Instead of !(nullable expression == true) use the more readable alternative of: (nullable expression == false).");
        }
    }
}
