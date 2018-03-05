namespace GCop.MSharp.Rules.Performance
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
    public class InverseRelationshipAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        private readonly string DefaultParameterType = "T";
        private InvocationExpressionSyntax Invocation;
        protected override SyntaxKind Kind => SyntaxKind.InvocationExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "305",
                Category = Category.Performance,
                Message = "Instead create an inverse relationship from {0} to this type and call it as a property",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            Invocation = (InvocationExpressionSyntax)context.Node;

            var method = GetMethodInfo(Invocation, context.SemanticModel);
            if (method == null || method.Name != "GetList") return;

            Invocation.ArgumentList.Arguments.Where(argument => argument.Expression is SimpleLambdaExpressionSyntax).Select(it => it.Expression).ForEach(lambda =>
            {
                var equal = lambda.ChildNodes().OfType<BinaryExpressionSyntax>().FirstOrDefault();

                MemberAccessExpressionSyntax otherHand = null;

                if (equal?.Right is ThisExpressionSyntax) otherHand = equal?.Left as MemberAccessExpressionSyntax;
                else if (equal?.Left is ThisExpressionSyntax) otherHand = equal?.Right as MemberAccessExpressionSyntax;

                if (otherHand != null && !IsTypeArgumentInterface(method))
                {
                    ReportDiagnostic(context, equal, GetGenericTypeName(method));
                }
            });
        }

        private IMethodSymbol GetMethodInfo(InvocationExpressionSyntax invocation, SemanticModel semanticModel)
        {
            return semanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
        }

        private string GetGenericTypeName(IMethodSymbol method) => method.TypeArguments.FirstOrDefault()?.Name ?? DefaultParameterType;

        private bool IsTypeArgumentInterface(IMethodSymbol method)
        {
            var typeArgument = method.TypeArguments.FirstOrDefault();
            return typeArgument.TypeKind == TypeKind.Interface;
        }
    }
}
