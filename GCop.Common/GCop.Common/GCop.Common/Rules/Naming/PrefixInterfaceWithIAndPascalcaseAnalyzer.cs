namespace GCop.Common.Rules.Naming
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PrefixInterfaceWithIAndPascalcaseAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.InterfaceDeclaration;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "205",
                Category = Category.Naming,
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var interfaceDeclaration = context.Node as InterfaceDeclarationSyntax;
            var identifier = interfaceDeclaration.Identifier;
            var interfaceName = identifier.ValueText;

            // Do not process interface names less than 2 chars
            if (interfaceName.Length < 2)
                return;

            if (!interfaceName.StartsWith("I"))
            {
                if (interfaceName.StartsWith("i"))
                {
                    ReportDiagnostic(context, identifier, "Interfaces must start with a capital 'I'.");
                    return;
                }
                else
                {
                    ReportDiagnostic(context, identifier, "Interfaces must start with the letter 'I'.");
                    return;
                }
            }

            interfaceName = interfaceName.Substring(1);
            if (!interfaceName.IsPascalCase())
            {
                ReportDiagnostic(context, identifier, "Interfaces names must be Pascal Case.");
                return;
            }
        }

        private void ReportDiagnostic(SyntaxNodeAnalysisContext context, SyntaxToken token, string message)
        {
            var diagnostic = Diagnostic.Create(Description, token.GetLocation(), message);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
