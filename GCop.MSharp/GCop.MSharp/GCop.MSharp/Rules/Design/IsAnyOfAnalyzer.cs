namespace GCop.MSharp.Rules.Design
{
    using Core;
    using Core.Attributes;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [MSharpExclusive]
    [ZebbleExclusive]
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class IsAnyOfAnalyzer : GCopAnalyzer
    {
        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "162",
                Category = Category.Design,
                Message = "Replace with {0}.IsAnyOf({1})",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Configure()
        {
            RegisterSyntaxNodeAction(context => Analyze(context), SyntaxKind.ReturnStatement, SyntaxKind.IfStatement, SyntaxKind.VariableDeclaration, SyntaxKind.SimpleAssignmentExpression);
        }

        protected void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;

            if (NodeToAnalyze.DescendantNodes().OfKind(SyntaxKind.LogicalOrExpression).None()) return;

            var logicalOrExpression = NodeToAnalyze.DescendantNodes().OfKind(SyntaxKind.LogicalOrExpression).ToList();

            var conditions = logicalOrExpression.SelectMany(it => it.ChildNodes()).OfType<BinaryExpressionSyntax>().Where(it => it.IsKind(SyntaxKind.EqualsExpression)).Select(exp => new Condition
            {
                Variable = exp?.Left.ToString(),
                Location = exp?.Left.GetLocation(),
                LeftSide = exp.Left,
                RightSide = exp.Right
            });

            if (!conditions.HasMany()) return;

            var numberOfExpression = 0;
            var items = "";

            conditions.GroupBy(it => it.Variable).Where(it => it.HasMany() && it.Key.HasValue()).ToList().ForEach(expressions =>
            {
                var allConditions = expressions.SkipWhile(it => it.LeftSide == null);

                foreach (var condition in allConditions)
                {
                    if (numberOfExpression == 2)
                    {
                        ReportDiagnostic(context, NodeToAnalyze, expressions.Key, items + "etc..");
                        numberOfExpression = 0;
                        break;
                    }
                    else
                    {
                        var symbol = context.SemanticModel.GetSymbolInfo(condition.LeftSide).Symbol;
                        if (symbol.Is<string>() /*|| symbol.IsInherited<IEntity>()*/)
                        {
                            numberOfExpression++;
                            items += $"{condition.RightSide}, ";
                        }
                    }

                    if (numberOfExpression == 2)
                    {
                        allConditions.ForEach(it =>
                        {
                            ReportDiagnostic(context, it.Location, expressions.Key, items + "etc..");
                            numberOfExpression = 0;
                        });
                        break;
                    }
                }
            });
        }

        private class Condition
        {
            public string Variable { get; set; }
            public IdentifierNameSyntax Identifier { get; set; }
            public ExpressionSyntax LeftSide { get; set; }
            public ExpressionSyntax RightSide { get; set; }
            public Location Location { get; set; }
        }
    }
}
