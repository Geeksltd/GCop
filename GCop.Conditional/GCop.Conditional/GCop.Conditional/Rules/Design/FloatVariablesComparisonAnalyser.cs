namespace GCop.Conditional.Rules.Design
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class FloatVariablesComparisonAnalyser : GCopAnalyzer
    {
        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "180",
                Category = Category.Design,
                Severity = DiagnosticSeverity.Warning,
                Message = "Double and float comparison isn't exact in .NET. Use myDouble.AlmostEquals(anotherDouble) instead."
            };
        }
        protected void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;

            var equalExpresion = NodeToAnalyze.DescendantNodesAndSelf().Where(x => x.IsKind(SyntaxKind.EqualsExpression, SyntaxKind.NotEqualsExpression));

            foreach (var equlaExper in equalExpresion)
            {
                if (equlaExper == null) continue;
                if (equlaExper.ChildNodes().None()) continue;

                var left = (equlaExper as BinaryExpressionSyntax)?.Left;
                var right = (equlaExper as BinaryExpressionSyntax)?.Right;
                if (left == null || right == null) return;

                var isLeftDouble = context.SemanticModel.GetSymbolInfo(left).Symbol?.Is<double>();
                var isRightDouble = context.SemanticModel.GetSymbolInfo(right).Symbol?.Is<double>();
                if ((isLeftDouble ?? false) && (isRightDouble ?? false))
                {
                    ReportDiagnostic(context, equlaExper);
                }

                var isLeftFloat = context.SemanticModel.GetSymbolInfo(left).Symbol?.Is<float>();
                var isRightFloat = context.SemanticModel.GetSymbolInfo(right).Symbol?.Is<float>();
                if ((isLeftFloat ?? false) && (isRightFloat ?? false))
                {
                    ReportDiagnostic(context, equlaExper);
                }
            }
        }
        protected override void Configure()
        {
            RegisterSyntaxNodeAction(Analyze, SyntaxKind.IfStatement
                                            , SyntaxKind.ConditionalExpression
                                            , SyntaxKind.ExpressionStatement);
        }
    }
}

