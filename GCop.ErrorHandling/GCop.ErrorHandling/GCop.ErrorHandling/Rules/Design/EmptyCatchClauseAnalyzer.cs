namespace GCop.ErrorHandling.Rules.Design
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EmptyCatchClauseAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.CatchClause;

        private CatchClauseSyntax Catch;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "138",
                Category = Category.Design,
                Severity = DiagnosticSeverity.Warning,
                Message = "When you catch an exception you should throw exception or at least log error"
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            Catch = context.Node as CatchClauseSyntax;
            NodeToAnalyze = Catch;

            if (Catch.Declaration == null && CatchClauseHasNoThrowStatement() && !HasLogStatement() && !IsExceptionInstanceUsedInAnyStatements() && !HasDebugOrConsoleStatments())
            {
                ReportDiagnostic(context, Catch.Block.OpenBraceToken);
                return;
            }

            Catch.DescendantNodes().OfType<CatchDeclarationSyntax>().ForEach(@catch =>
           {
               if (IsGeneralCatch(@catch) && CatchClauseHasNoThrowStatement() && !HasLogStatement() && !IsExceptionInstanceUsedInAnyStatements() && !HasDebugOrConsoleStatments())
               {
                   ReportDiagnostic(context, Catch.Block.OpenBraceToken);
               }
           });
        }

        private bool CatchClauseHasNoThrowStatement() => Catch.DescendantNodes().OfType<ThrowStatementSyntax>().None();

        private bool IsGeneralCatch(CatchDeclarationSyntax @catch)
        {
            return @catch.ChildNodes().OfType<IdentifierNameSyntax>().Any(it => it.Identifier.ValueText == "Exception");
        }

        private bool HasLogStatement()
        {
            if (Catch.Block.CloseBraceToken.GetAllTrivia().Any(it => it.ToString().Contains("log", caseSensitive: false))) return true;

            if (Catch.Block.DescendantTrivia().Any(t => t.ToString().Contains("log", false))) return true;


            var tokens = Catch.Block.DescendantNodes().SelectMany(it => it.ChildNodesAndTokens()).ToList();

            if (tokens.None()) return false;

            return tokens.TrueForAtLeastOnce(nodeOrToken =>
            {
                if (nodeOrToken.GetLeadingTrivia().Any(it => it.ToString().Contains("log", caseSensitive: false))) return true;
                if (nodeOrToken.GetTrailingTrivia().Any(it => it.ToString().Contains("log", caseSensitive: false))) return true;
                if (nodeOrToken.ToString().Contains("log", caseSensitive: false)) return true;
                if (nodeOrToken.GetLeadingTrivia().Any(it => it.ToString().Contains("log", caseSensitive: false))) return true;

                return false;
            });
        }

        private bool HasDebugOrConsoleStatments()
        {
            var keywords = new string[] { "Debug.WriteLineIf", "Debug.WriteIf", "Debug.Write", "Debug.WriteLine", "Console.Write", "Console.WriteLine" };
            return Catch.Block.DescendantNodes().OfType<MemberAccessExpressionSyntax>().Any(it => it.ToString().ContainsAny(keywords, caseSensitive: true));
        }

        private bool IsExceptionInstanceUsedInAnyStatements()
        {
            var token = Catch.Declaration?.ChildTokens()?.FirstOrDefault(it => it.IsKind(SyntaxKind.IdentifierToken));
            if (token == null) return false;

            return Catch.Block.DescendantTokens().Any(it => it.IsKind(SyntaxKind.IdentifierToken) && it.ToString() == token.ToString());
        }
    }
}
