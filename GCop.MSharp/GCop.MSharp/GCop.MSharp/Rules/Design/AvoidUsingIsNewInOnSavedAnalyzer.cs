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
    public class AvoidUsingIsNewInOnSavedAnalyzer : GCopAnalyzer
    {
        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "159",
                Category = Category.Design,
                Message = "In OnSaved method the property IsNew must not be used, instead use: e.Mode == SaveMode.Insert",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Configure() => RegisterCodeBlockAction(context => Analyze(context));

        protected void Analyze(CodeBlockAnalysisContext context)
        {
            var method = context.CodeBlock as MethodDeclarationSyntax;
            if (method == null) return;
            NodeToAnalyze = method;

            if (method.Identifier.ValueText != "OnSaved" || method.Modifiers.None(it => it.IsKind(SyntaxKind.OverrideKeyword))) return;

            method.Body.DescendantTokens().Where(it => it.IsKind(SyntaxKind.IdentifierToken) && it.ToString() == "IsNew").ForEach(it =>
            {
                context.ReportDiagnostic(Diagnostic.Create(Description, it.GetLocation()));
            });
        }
    }
}
