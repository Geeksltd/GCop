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
    public class OnSavedAnalyzer : GCopAnalyzer
    {
        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "143",
                Category = Category.Design,
                Message = "First line of OnSaved() method must be a call to base.OnSaved() otherwise CachedReferences will have a problem.",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Configure() => RegisterCodeBlockAction(context => Analyze(context));

        private void Analyze(CodeBlockAnalysisContext context)
        {
            NodeToAnalyze = context.CodeBlock;
            var method = context.CodeBlock as MethodDeclarationSyntax;

            if (method == null || method.Identifier.ValueText != "OnSaved" || method.Modifiers.None(it => it.IsKind(SyntaxKind.OverrideKeyword))) return;

            if (method.ParameterList.Parameters.None(it => it.Type.ToString() == "SaveEventArgs")) return;

            var firstStatement = method.Body?.ChildNodes().FirstOrDefault();

            if (!(firstStatement is ExpressionStatementSyntax))
            {
                context.ReportDiagnostic(Diagnostic.Create(Description, method.Body?.OpenBraceToken.GetLocation() ?? method.Identifier.GetLocation()));
                return;
            }

            var expression = firstStatement as ExpressionStatementSyntax;
            if (!expression.ToString().StartsWith("base.OnSaved"))
            {
                context.ReportDiagnostic(Diagnostic.Create(Description, method.Body?.OpenBraceToken.GetLocation() ?? method.Identifier.GetLocation()));
            }
        }
    }
}
