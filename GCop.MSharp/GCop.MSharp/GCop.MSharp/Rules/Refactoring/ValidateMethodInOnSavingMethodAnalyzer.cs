namespace GCop.MSharp.Rules.Refactoring
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ValidateMethodInOnSavingMethodAnalyzer : GCopAnalyzer
    {
        protected override void Configure() => RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration);

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "630",
                Category = Category.Refactoring,
                Severity = DiagnosticSeverity.Warning,
                Message = "Override the Validate() method and write your validation logic there."
            };
        }

        protected void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            if ((NodeToAnalyze as MethodDeclarationSyntax).GetName() != "OnSaving") return;

            //checking the inheritance from Entity
            var classNode = NodeToAnalyze.GetSingleAncestor<ClassDeclarationSyntax>() as ClassDeclarationSyntax;
            if (classNode == null) return;

            var symbolClass = context.SemanticModel.GetDeclaredSymbol(classNode);
            if (symbolClass == null) return;

            //var baseClass = symbolClass.BaseType;
            //var inherit = symbolClass.IsInherited<MSharp.Framework.IEntity>();
            //if (inherit == false) return;

            var methodbody = (NodeToAnalyze as MethodDeclarationSyntax).Body;
            if (methodbody == null) return;

            // Finding  calls another method whose name starts with Validate...()
            var childs = methodbody.DescendantNodes().OfKind(SyntaxKind.InvocationExpression);
            if (childs.None()) return;

            var methodsHasValidatenTheirName = childs.Where(x => x.GetIdentifier() != null).Where(it => it.GetIdentifier().StartsWith("Validate"));
            if (methodsHasValidatenTheirName.None())
            {
                // for this situation: this.Validate();
                var members = childs.SelectMany(x => x.ChildNodes().OfKind(SyntaxKind.SimpleMemberAccessExpression));
                if (members.None()) return;

                methodsHasValidatenTheirName = members.Where(x => x.GetIdentifier() != null).Where(it => it.GetIdentifier().StartsWith("Validate"));
            }

            var identifier = methodsHasValidatenTheirName.FirstOrDefault()?.GetIdentifierSyntax();
            if (identifier == null) return;
            ReportDiagnostic(context, identifier);
        }
    }
}
