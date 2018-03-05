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
    public class UseIdsExtensionAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.SimpleMemberAccessExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "127",
                Message = "Use {0}.IDs() instead of {1}",
                Category = Category.Design,
                Severity = DiagnosticSeverity.Warning,
                IsEnabledByDefault = true
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var memberAccessExpressionSyntax = context.Node as MemberAccessExpressionSyntax;
            if (memberAccessExpressionSyntax == null) return;

            var isSelectExpression = memberAccessExpressionSyntax.Name.ToString() == "Select";
            if (!isSelectExpression) return;

            var invocationExpression = memberAccessExpressionSyntax.Parent as InvocationExpressionSyntax;
            if (invocationExpression == null) return;
            if (invocationExpression.ArgumentList.Arguments.Count != 1) return;

            var lambdaExpression = invocationExpression.ArgumentList.Arguments[0].Expression as LambdaExpressionSyntax;
            if (lambdaExpression == null) return;

            var memberAccessExpression = lambdaExpression.Body as MemberAccessExpressionSyntax;
            if (memberAccessExpression == null) return;
            if (memberAccessExpression.Name.ToString() != "ID") return;

            var symbolInfo = context.SemanticModel.GetSymbolInfo(memberAccessExpressionSyntax.Expression, context.CancellationToken);

            var sourceLocalSymbol = symbolInfo.Symbol as ILocalSymbol;
            if (sourceLocalSymbol == null) return;


            var isEnumerable = false;
            var isEntity = false;

            // surely should just find if it implements IEnumerable and the T is IEntity.
            // then from that we dont need to check ArrayTypeSymbol or INamedTypeSymbol etc.
            if (sourceLocalSymbol.Type is IArrayTypeSymbol arrayType)
            {
                isEnumerable = arrayType.AllInterfaces.Any(i => i.Name == "IEnumerable");
                isEntity = arrayType.ElementType.AllInterfaces.Any(i => i.Name == "IEntity" && i.IsGenericType); // Entity<T>
            }

            if (sourceLocalSymbol.Type is INamedTypeSymbol namedTypeSymbol)
            {
                var typeArgument = namedTypeSymbol.TypeArguments.FirstOrDefault();

                if (typeArgument != null)
                {
                    isEntity = typeArgument.BaseType.AllInterfaces.Any(i => i.Name == "IEntity" && i.IsGenericType);
                    isEnumerable = namedTypeSymbol.AllInterfaces.Any(i => i.Name == "IEnumerable");
                }
            }

            if (isEnumerable == false || isEntity == false) return;


            var diagnostic = Diagnostic.Create(
                Description,
                memberAccessExpressionSyntax.GetLocation(),
                sourceLocalSymbol.Name,
                memberAccessExpressionSyntax.ToString());

            context.ReportDiagnostic(diagnostic);
        }
    }
}