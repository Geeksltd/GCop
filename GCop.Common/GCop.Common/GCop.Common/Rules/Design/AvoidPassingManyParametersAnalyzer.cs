namespace GCop.Common.Rules.Design
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AvoidPassingManyParametersAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        const int ParameterCount = 6;

        protected override SyntaxKind Kind => SyntaxKind.MethodDeclaration;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "101",
                Category = Category.Design,
                Severity = DiagnosticSeverity.Warning,
                Message = "Too many parameters as argument. Define a container and encapsulate them",
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;
            NodeToAnalyze = methodDeclaration;
            if (methodDeclaration.AttributeLists.Any(it => it.Attributes.Any(attr => attr.Name.ToString().Contains("DllImport")))) return;
            //We are not supposed to look at extension methods parameters which they will have this keyword
            var parameters =
                methodDeclaration.ParameterList.Parameters
                .Where(it => it.ChildTokens()
                                .None(child => child.IsKind(SyntaxKind.ThisKeyword)) && it.ChildNodes().OfType<EqualsValueClauseSyntax>().None())
                                .ToList();

            if (parameters.Count() > ParameterCount)
            {
                ReportDiagnostic(context, methodDeclaration.Identifier);
            }
        }
    }
}
