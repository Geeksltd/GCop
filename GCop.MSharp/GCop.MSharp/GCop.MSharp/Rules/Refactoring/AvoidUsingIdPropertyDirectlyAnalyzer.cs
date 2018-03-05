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
    public class AvoidUsingIdPropertyDirectlyAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.SimpleLambdaExpression;

        protected override RuleDescription GetDescription()
        {
            //((DteExtensions.DTE.
            return new RuleDescription
            {
                ID = "602",
                Category = Category.Refactoring,
                Severity = DiagnosticSeverity.Warning,
                Message = "while the type of {0} already has a property named {1}, Use it instead of ID"
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var lambda = context.Node as SimpleLambdaExpressionSyntax;
            if (lambda.Body == null || lambda.Parameter == null) return;

            var memberAccessExpressions = lambda.Body.DescendantNodes().OfType<MemberAccessExpressionSyntax>()?.Where(it => it.ChildNodes().OfType<IdentifierNameSyntax>().Any(i => i.Identifier.ValueText == "ID"));

            memberAccessExpressions.ForEach(expression =>
            {
                var callOfProperties = GetCallsOfProperties(expression.ToString());
                if (callOfProperties.Length < 2) return;

                ITypeSymbol idContainerType = null;
                IdentifierNameSyntax firstIdentifier = null;

                if (callOfProperties.Length == 2)
                {
                    var property = expression.ChildNodes().OfType<IdentifierNameSyntax>().FirstOrDefault();
                    if (property == null) return;

                    var propertyInfo = context.SemanticModel.GetSymbolInfo(property).Symbol as IPropertySymbol;
                    if (propertyInfo == null) return;

                    idContainerType = propertyInfo.ContainingType;
                }
                else
                {
                    firstIdentifier = GetFirstIdentifier(expression);
                    if (firstIdentifier == null) return;

                    idContainerType = context.SemanticModel.GetTypeInfo(firstIdentifier).Type as ITypeSymbol;
                }
                var lambdaParameter = context.SemanticModel.GetDeclaredSymbol(lambda.Parameter) as IParameterSymbol;
                if (lambdaParameter == null) return;


                var memberAccessContainsId = expression.DescendantNodesAndSelf()
                .OfType<MemberAccessExpressionSyntax>().FirstOrDefault(it => it.ChildNodes().OfType<IdentifierNameSyntax>().Any(idn => idn.Identifier.ValueText == "ID"));

                if (memberAccessContainsId == null) return;

                var firstExpression = memberAccessContainsId;
                var idContainer = firstExpression.ChildNodes().OfType<IdentifierNameSyntax>().FirstOrDefault();

                if (callOfProperties.Length > 2)
                {
                    firstExpression = memberAccessContainsId.ChildNodes().OfType<MemberAccessExpressionSyntax>().FirstOrDefault();
                    if (firstExpression == null) return;
                    idContainer = firstExpression.ChildNodes()?.OfType<IdentifierNameSyntax>().LastOrDefault();
                }
                if (idContainer == null) return;

                var idPropertyName = idContainer.Identifier.ValueText + "Id";

                if (idContainerType.GetMembers().Any(it => it.Name == idPropertyName))
                {
                    ReportDiagnostic(context, expression, lambdaParameter.Type.Name, idPropertyName);
                }
            });
        }

        private IdentifierNameSyntax GetFirstIdentifier(MemberAccessExpressionSyntax expression)
        {
            var propertyCallers = expression.ToString().Split('.');
            var index = propertyCallers.Length - 3;
            return expression.DescendantNodes().OfType<IdentifierNameSyntax>().FirstOrDefault(it => it.Identifier.ValueText == propertyCallers[index]);
        }


        private string[] GetCallsOfProperties(string expression) => expression.Split('.');
    }
}
