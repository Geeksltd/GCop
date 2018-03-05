namespace GCop.Collections.Rules.Design
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System;
    using System.Collections;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EmptyCollectionInsteadOfNullAnalyzer:GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.MethodDeclaration;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "103",
                Category = Category.Design,
                Message = "Instead of null, return an empty collection such as Enumerable.Empty<{0}>",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {

            NodeToAnalyze = context.Node;
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            if (IsPredefinedType(methodDeclaration.ReturnType)) return;

            var method = context.SemanticModel.GetDeclaredSymbol(methodDeclaration);

            //Rule should not be effected if method ReturnType is not assignable from IEnumerable. all other generic types are allowed
            if (method.ReturnType.AllInterfaces.None(it => it.Name == typeof(IEnumerable).Name)) return;

            if (method.ReturnType.Name == "SqlConnectionStringBuilder") return;

            var returnStatments = methodDeclaration.GetReturnStatements();

            returnStatments?.ToList().ForEach(@return =>
            {
                if (@return.Expression.IsKind(SyntaxKind.NullLiteralExpression))
                {
                    ReportDiagnostic(context, @return, method.ReturnType.Normalize());
                }
            });
        }


        private bool IsPredefinedType(TypeSyntax type) => type is PredefinedTypeSyntax || type is NullableTypeSyntax;

        /* private bool IsGenericType(TypeSyntax type)
         {
             return type is GenericNameSyntax;
         }*/
    }
}

