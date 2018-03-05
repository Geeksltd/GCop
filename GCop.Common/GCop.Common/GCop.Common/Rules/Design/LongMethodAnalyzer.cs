namespace GCop.Common.Rules.Design
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class LongMethodAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        private readonly string FetchPrefix = "Get";
        private readonly short MaximumNumberOfStatements = 40;
        private readonly string[] ExcludedBaseClasses = new string[] { "SqlDataProvider" };
        private readonly string[] ExcludedMethods = new string[] { "ValidateProperties", "Cascade_Deleting" };

        private readonly string StringBuilderType = "System.Text.StringBuilder";
        private readonly string GenericListType = "System.Collections.Generic.List";

        protected override SyntaxKind Kind => SyntaxKind.MethodDeclaration;

        public MethodDeclarationSyntax Method;
        public ClassDeclarationSyntax Class;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "116",
                Category = Category.Design,
                Severity = DiagnosticSeverity.Warning,
                Message = @"Break this down into smaller methods. If such methods would become meaningless as standalone methods in the context of the class, you can refactor this method into a Stateful Service class"
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            Method = context.Node as MethodDeclarationSyntax;

            //this condition solves the conflict with UseCalculateForMethodNameAnalyzer
            if (Method?.GetName()?.StartsWith(FetchPrefix) ?? true) return;
            if (Method.Body == null) return;

            Class = Method.Parent as ClassDeclarationSyntax;
            if (Class == null) return;

            if (Class.Is("SqlDataProvider")) return;

            var baseClass = Class.BaseList?.Types.OfType<SimpleBaseTypeSyntax>().FirstOrDefault();

            if (baseClass != null)
            {
                var baseClassName = (baseClass.Type as IdentifierNameSyntax)?.Identifier.ValueText;

                if (baseClassName.IsEmpty()) return;

                if (ExcludedBaseClasses.Contains(baseClassName))
                    return;
                var baseClassSymbol = context.SemanticModel.GetSymbolInfo(baseClass.Type).Symbol as INamedTypeSymbol;

                if (!Array.TrueForAll(ExcludedBaseClasses, type => !IsAssignableFrom(baseClassSymbol, type))) return;
            }

            var method = context.SemanticModel.GetDeclaredSymbol(Method) as IMethodSymbol;
            if (method == null) return;

            if (ExcludedMethods.Contains(method.Name)) return;

            if (Method.Body.GetCountOfStatements() - CalculateCountOfIgnoredStatements(context.SemanticModel, Method.Body) > MaximumNumberOfStatements)
            {
                ReportDiagnostic(context, Method.Identifier, method.Name);
            }
        }

        private bool IsAssignableFrom(INamedTypeSymbol @class, string type)
        {
            try
            {
                while (@class != null)
                {
                    var typeName = (@class.BaseType?.Name).Or(@class.Name);
                    if (typeName == type) return true;

                    @class = @class.BaseType;
                }
                return false;
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }

        private int CalculateCountOfIgnoredStatements(SemanticModel semanticModel, BlockSyntax body)
        {
            int numberOfIgnoredStatements = 0;
            foreach (var statement in body.DescendantNodes().OfType<MemberAccessExpressionSyntax>())
            {
                var memberCaller = statement?.GetIdentifierSyntax();
                if (memberCaller == null) continue;
                var methodName = statement.ChildNodes().OfType<IdentifierNameSyntax>().LastOrDefault()?.Identifier.ValueText;
                var memberCallerInfo = semanticModel.GetTypeInfo(memberCaller).Type;
                if (memberCallerInfo == null) continue;
                if (memberCallerInfo.ToString().StartsWith(GenericListType) && methodName == "Add") numberOfIgnoredStatements++;
                if (memberCallerInfo.ToString().StartsWith(StringBuilderType)) numberOfIgnoredStatements++;
            }
            return numberOfIgnoredStatements;
        }
    }
}
