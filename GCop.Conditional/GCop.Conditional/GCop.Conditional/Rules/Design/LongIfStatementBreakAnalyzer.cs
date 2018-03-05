namespace GCop.Conditional.Rules.Design
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class LongIfStatementBreakAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        private readonly int MaximumAllowedCharacters = 100;
        protected override SyntaxKind Kind => SyntaxKind.IfStatement;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "115",
                Category = Category.Design,
                Message = "This IF statement is too long for one line. Break the body of the IF into another line",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var @if = (IfStatementSyntax)context.Node;

            if (@if.CloseParenToken.GetAllTrivia().Any(it => it.IsKind(SyntaxKind.EndOfLineTrivia))) return;

            var sibilings = @if.ChildNodesAndTokens().ToList();
            var closeParenIndex = sibilings.IndexOf(@if.CloseParenToken);
            if (closeParenIndex <= 1) return;

            //4 character by default => if()
            var countOfCharacter = 0;

            //var previousStatement = sibilings[closeParenIndex - 1];

            //if (previousStatement != null)
            //{
            //    countOfCharacter += previousStatement.ToString().Length;
            //}

            var nextStatement = sibilings[closeParenIndex + 1];

            if (nextStatement != null)
            {
                countOfCharacter += nextStatement.ToString().Length;
            }

            if (countOfCharacter > MaximumAllowedCharacters) ReportDiagnostic(context, @if);
        }

        private void ReportDiagnostic(SyntaxNodeAnalysisContext context, SyntaxNode node)
        {
            var diagnostic = Diagnostic.Create(Description, node.GetFirstToken().GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }
}
