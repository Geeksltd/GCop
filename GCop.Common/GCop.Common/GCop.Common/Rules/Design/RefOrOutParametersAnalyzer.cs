namespace GCop.Common.Rules.Design
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RefOrOutParametersAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.MethodDeclaration;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "119",
                Category = Category.Design,
                Severity = DiagnosticSeverity.Warning,
                Message = "Don’t use {0} parameters in method definition. To return several objects, define a class or struct for your method return type."
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            if (methodDeclaration.Modifiers.Any(it => it.IsKind(SyntaxKind.OverrideKeyword))) return;

            //we should skip the rule if method has a [DllImport] attribute
            if (methodDeclaration.AttributeLists.SelectMany(it => it.Attributes).Any(it => it.Name.ToString() == "DllImport")) return;

            methodDeclaration.ParameterList.Parameters.ToList().ForEach(arg =>
            {
                if (context.SemanticModel.GetDeclaredSymbol(arg) is IParameterSymbol parameter && parameter.RefKind != RefKind.None)
                {
                    var diagnostic = Diagnostic.Create(Description, arg.Identifier.GetLocation(), parameter.RefKind.ToString().ToLower());
                    context.ReportDiagnostic(diagnostic);
                }
            });
        }
    }
}
