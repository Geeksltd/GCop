namespace GCop.MSharp.Rules.Refactoring
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
    public class UseFindByWhereApplicableAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.InvocationExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "618",
                Category = Category.Refactoring,
                Severity = DiagnosticSeverity.Warning,
                Message = "Use {0} instead."
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;

            var invocation = context.Node as InvocationExpressionSyntax;

            var memberAccessExpression = invocation.Expression as MemberAccessExpressionSyntax;
            if (memberAccessExpression == null || invocation.ArgumentList.Arguments.None() || invocation.ArgumentList.Arguments.HasMany()) return;

            var methodInfo = context.SemanticModel.GetSymbolInfo(memberAccessExpression).Symbol as IMethodSymbol;
            if (methodInfo == null || methodInfo.Name != "Find" || methodInfo.ContainingType.Name != "Database" || methodInfo.ContainingAssembly.Name != "MSharp.Framework") return;

            var lambdaExpression = invocation.ArgumentList.Arguments.FirstOrDefault()?.Expression as SimpleLambdaExpressionSyntax;
            var equalsExpression = lambdaExpression?.ChildNodes().FirstOrDefault(it => it.IsKind(SyntaxKind.EqualsExpression)) as BinaryExpressionSyntax;
            if (equalsExpression == null || equalsExpression.Left == null || equalsExpression.Right == null) return;

            equalsExpression.Left.DescendantNodes().OfType<IdentifierNameSyntax>().ForEach(it =>
            {
                if (context.SemanticModel.GetSymbolInfo(it).Symbol is IPropertySymbol property)
                {
                    var memberName = $"FindBy{property.Name}";
                    var containingMethod = context.Node.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();
                    if (containingMethod.GetName() != memberName)
                    {
                        if (property.ContainingType.GetMembers().Any(member => member.Name == memberName && member.Kind == SymbolKind.Method && member.IsStatic))
                        {
                            memberName = $"{property.ContainingType.Name}.FindBy{property.Name}({equalsExpression.Right})";
                            ReportDiagnostic(context, invocation, memberName);
                        }
                    }
                }
            });
        }
    }
}
