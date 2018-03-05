namespace GCop.Common.Rules.Design
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ComplexCodeAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        private readonly string[] MethodNames = new string[] { "OnSaving", "OnDeleting" };

        protected override SyntaxKind Kind => SyntaxKind.MethodDeclaration;
        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "134",
                Category = Category.Design,
                Severity = DiagnosticSeverity.Warning,
                Message = "This method should not contain complex code, Instead call other focused methods to perform the complex logic"
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node;
            NodeToAnalyze = node;

            BlockSyntax block = null;
            Location errorLocation = null;

            var method = (MethodDeclarationSyntax)node;

            if (!method.Is(MethodNames)) return;

            block = method.Body;
            if (block == null) return;

            errorLocation = method.Identifier.GetLocation();

            if (block.IsTooLong(Numbers.Ten))
            {
                ReportDiagnostic(context, errorLocation);
            }
        }
    }
}
