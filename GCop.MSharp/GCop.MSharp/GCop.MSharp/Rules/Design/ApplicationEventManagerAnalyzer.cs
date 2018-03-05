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
    public class ApplicationEventManagerAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        private string ClassName = "ApplicationEventManager";
        private string MethodName = "RecordException";
        protected override SyntaxKind Kind => SyntaxKind.SimpleMemberAccessExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "100",
                Category = Category.Design,
                Message = "Replace with Log.Error(...)",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            var invocation = context.Node as MemberAccessExpressionSyntax;
            if (invocation == null) return;

            NodeToAnalyze = invocation;

            var method = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
            if (method == null) return;

            if (method.Name != MethodName || method.ContainingType.Name != ClassName || method.ContainingNamespace.ToString().Lacks("MSharp")) return;
            //if (method.Name == MethodName && method.ContainingType.Name == ClassName && method.ContainingNamespace.ToString().Contains("MSharp"))

            var recordExceptionIdentifier = invocation.DescendantNodes().OfType<IdentifierNameSyntax>().FirstOrDefault(it => it.Identifier.ValueText == MethodName);

            var elementLocation = recordExceptionIdentifier?.GetLocation() ?? invocation.DescendantNodes().OfType<IdentifierNameSyntax>().FirstOrDefault()?.GetLocation() ?? invocation.GetLocation();

            context.ReportDiagnostic(Diagnostic.Create(Description, elementLocation));
        }
    }
}
