namespace GCop.Thread.Rules.Usage
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class VoidAsyncMethodAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.MethodDeclaration;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "500",
                Category = Category.Usage,
                Severity = DiagnosticSeverity.Warning,
                Message = "Method should not be void"
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var methodDeclaration = NodeToAnalyze as MethodDeclarationSyntax;

            if (methodDeclaration.ChildTokens().None(it => it.IsKind(SyntaxKind.AsyncKeyword))) return;
            if (methodDeclaration.ReturnType?.ToString() != "void") return;

            if (methodDeclaration.Modifiers.Any(x => x.IsKind(SyntaxKind.OverrideKeyword))) return;

            if (methodDeclaration.GetName().Contains("_")) return;
            if (methodDeclaration.GetName().EndsWith("ed")) return;

            if (methodDeclaration.ParameterList.Parameters.TrueForAtLeastOnce(it =>
             {
                 var parameter = context.SemanticModel.GetDeclaredSymbol(it) as IParameterSymbol;
                 return parameter.IsInherited<EventArgs>() || parameter.Type.Name == "Object" || parameter.Type.Name.EndsWith("EventArgs");
             })) return;

            ReportDiagnostic(context, methodDeclaration.ReturnType);
        }
    }
}
