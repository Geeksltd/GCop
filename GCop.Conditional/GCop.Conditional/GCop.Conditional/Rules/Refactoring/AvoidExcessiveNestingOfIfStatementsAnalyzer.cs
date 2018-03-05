namespace GCop.Conditional.Rules.Refactoring
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Collections.Generic;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AvoidExcessiveNestingOfIfStatementsAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.IfStatement;

        const int MAX_LEVELS = 4;
        const int MAX_STATEMENTS = 20;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "617",
                Category = Category.Refactoring,
                Severity = DiagnosticSeverity.Warning,
                Message = "Avoid deep nesting of IF statements. Break the method down into smaller methods, or return early if possible."
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var ifExpression = NodeToAnalyze as IfStatementSyntax;

            if (ifExpression == null || ifExpression.Parent is ElseClauseSyntax)
                return;

            //Skip if the IF statement doesn't have any brackets. 
            if (ifExpression.ChildNodes().OfType<BlockSyntax>().None()) return;

            var callingMethod = ifExpression.GetParent<MethodDeclarationSyntax>() ?? ifExpression.GetParent<ConstructorDeclarationSyntax>();
            var numberOfStatements = callingMethod?.DescendantNodes().OfType<StatementSyntax>().Count(i => i.ChildTokens().LastOrDefault().IsKind(SyntaxKind.SemicolonToken));  // Statements ending with semicolon.

            if (numberOfStatements < MAX_STATEMENTS)
                return;

            GetNthLevelNodes(ifExpression).Do(i => ReportDiagnostic(context, i));
        }

        private IEnumerable<SyntaxNode> GetNthLevelNodes(SyntaxNode node)
        {
            var next = GetNextNodes(node);
            var counter = 2;

            while (counter < MAX_LEVELS)
            {
                if (next.None())
                    return next;

                next = next.SelectMany(i => GetNextNodes(i));
                counter++;
            }

            return next;
        }

        private IEnumerable<SyntaxNode> GetNextNodes(SyntaxNode node)
        {
            var nestedIfs = (node.ChildNodes().OfType<BlockSyntax>().FirstOrDefault()?.ChildNodes().OfType<IfStatementSyntax>().Select(i => i as SyntaxNode)).OrEmpty();

            var nestedElses = (node.ChildNodes().OfType<ElseClauseSyntax>()
                .Where(e => e?.ChildNodes().OfType<BlockSyntax>().Any() ?? false)
                .SelectMany(i => i?.ChildNodes().OfType<BlockSyntax>().SelectMany(x => x?.ChildNodes().OfType<IfStatementSyntax>())
                .Select(u => u as SyntaxNode))).OrEmpty();

            var nestedElseIfs = (node.ChildNodes().OfType<ElseClauseSyntax>()
                .SelectMany(i => i?.ChildNodes().OfType<IfStatementSyntax>())
                .SelectMany(x => x.ChildNodes().OfType<BlockSyntax>().SelectMany(u => u?.ChildNodes().OfType<IfStatementSyntax>())
                .Select(t => t as SyntaxNode))).OrEmpty();

            return nestedIfs.Concat(nestedElses).Concat(nestedElseIfs);
        }
    }
}
