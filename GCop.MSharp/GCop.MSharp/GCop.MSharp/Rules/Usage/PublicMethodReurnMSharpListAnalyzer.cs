namespace GCop.MSharp.Rules.Usage
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
    public class PublicMethodReurnMSharpListAnalyzer : GCopAnalyzer
    {
        protected override void Configure()
        {
            RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration);
        }

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "533",
                Category = Category.Usage,
                Severity = DiagnosticSeverity.Warning,
                Message = "Choose return type of IEnumerable<{0}> to imply that it's not modifiable."
            };
        }
        private void Analyze(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;
            NodeToAnalyze = methodDeclaration;

            //Checking the method scope , only public is accepted
            var methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclaration);
            if (methodSymbol.DeclaredAccessibility != Accessibility.Public) return;
            if (methodSymbol.DeclaredAccessibility == Accessibility.Private) return;
            if (methodSymbol.DeclaredAccessibility == Accessibility.Protected) return;

            //Skipping the rule if any of the method parameters is List<...>
            if (methodDeclaration.ParameterList != null)
            {
                if (methodDeclaration.ParameterList.Parameters.Any())
                {
                    foreach (var paramater in methodDeclaration.ParameterList.Parameters)
                    {
                        if (paramater.ChildNodes().OfKind(SyntaxKind.GenericName).Any()) return;
                    }
                }
            }

            // Checking the reurn of method , it should be List<Something>
            var returnType = methodDeclaration.ReturnType;
            if (returnType.Kind() != SyntaxKind.GenericName) return;

            var generic = returnType as GenericNameSyntax;
            if (generic.Identifier == null) return;
            if (generic.Identifier.ValueText != "List") return;

            var entityTypeSyntax = generic.ChildNodes().OfType<TypeArgumentListSyntax>().FirstOrDefault();
            if (entityTypeSyntax == null) return;

            if (entityTypeSyntax.GetIdentifierSyntax() == null) return;

            var entitySymbol = context.SemanticModel.GetSymbolInfo(entityTypeSyntax.GetIdentifierSyntax()).Symbol;
            if (entitySymbol == null) return;

            if (IsInheritedFromIEntity(entitySymbol) == false) return;
            ReportDiagnostic(context, generic, entityTypeSyntax.GetIdentifier());
        }

        private bool IsInheritedFromIEntity(ISymbol symbol)
        {
            if (symbol is INamedTypeSymbol)
                return (symbol as INamedTypeSymbol).AllInterfaces.Any(it => it.Name == "IEntity");
            return false;
        }

    }
}
