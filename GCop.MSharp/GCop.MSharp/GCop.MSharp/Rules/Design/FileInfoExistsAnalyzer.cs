namespace GCop.MSharp.Rules.Design
{
    using Core;
    using Core.Attributes;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [MSharpExclusive]
    [ZebbleExclusive]
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class FileInfoExistsAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.SimpleMemberAccessExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "174",
                Category = Category.Design,
                Message = "You should use the method Exists() instead of the property because the property caches the result, which can cause problems.",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var invocation = context.Node as MemberAccessExpressionSyntax;
            if (invocation == null) return;

            var property = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IPropertySymbol;
            if (property == null) return;

            if (property.Name == "Exists" && property.ContainingType.Name == "FileInfo")
            {
                ReportDiagnostic(context, invocation);
            }
        }
    }
}
