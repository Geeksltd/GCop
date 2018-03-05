namespace GCop.MSharp.Rules.Performance
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
    public class DatabaseSaveThisInOnSavingOnSavedAnalyzer : GCopAnalyzer
    {
        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "319",
                Category = Category.Performance,
                Severity = DiagnosticSeverity.Warning,
                Message = "[Database.Save(this);] without condition will create a loop in {0} method."
            };
        }

        protected override void Configure() => RegisterCodeBlockAction(context => Analyze(context));

        protected void Analyze(CodeBlockAnalysisContext context)
        {
            var method = context.CodeBlock as MethodDeclarationSyntax;
            if (method == null) return;

            NodeToAnalyze = method;
            if (method.Identifier.ValueText.IsNoneOf("OnSaved", "OnSaving") || method.Modifiers.None(it => it.IsKind(SyntaxKind.OverrideKeyword))) return;

            method.Body.ChildNodes().OfType<ExpressionStatementSyntax>().ForEach(it =>
            {
                if (it.ToString().ToLower().Replace(" ", "") == "database.save(this);")
                    context.ReportDiagnostic(Diagnostic.Create(Description, it.GetLocation(), method.Identifier.ValueText));
            });
        }
    }
}