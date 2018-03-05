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
    public class AvoidNullCheckInDatabaseExpressionAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        string MSharp = "MSharp.Framework";
        protected override SyntaxKind Kind => SyntaxKind.InvocationExpression;

        InvocationExpressionSyntax Invocation;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "145",
                Category = Category.Design,
                Message = "This phrase is translated to SQL which handles NULL implicitly. Do not worry about null values and write your query against the property (path) simply without using .Get(x => x...) or ?."
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            Invocation = context.Node as InvocationExpressionSyntax;
            NodeToAnalyze = Invocation;
            if (Invocation == null || Invocation.Expression.GetIdentifier() != "Database") return;

            Invocation.ArgumentList.Arguments.Select(it => it.Expression).Where(it => it is SimpleLambdaExpressionSyntax && CanRunInDbMode(it as SimpleLambdaExpressionSyntax, context.SemanticModel)).ForEach(it =>
             {
                 var lambda = it as SimpleLambdaExpressionSyntax;

                 lambda.Body.DescendantNodes().OfType<InvocationExpressionSyntax>().ForEach(invocation =>
                 {
                     var nameSyntax = GetIdentifier(invocation.Expression);
                     if (nameSyntax != null)
                     {
                         if (context.SemanticModel.GetSymbolInfo(nameSyntax).Symbol is IMethodSymbol method && method.Name == "Get" && method.ContainingAssembly.Name == MSharp)
                         {
                             //Provided lambda expression in Get method

                             if (invocation.ArgumentList.Arguments.FirstOrDefault()?.Expression is SimpleLambdaExpressionSyntax getLambda)
                             {
                                 var lambdaParameter = getLambda.Parameter.ToString();
                                 //All of these properties should not have [Calculated] attribute
                                 if (getLambda.DescendantNodes().OfType<MemberAccessExpressionSyntax>().SelectMany(node => node.ChildNodes().OfType<IdentifierNameSyntax>()).ToList().TrueForAll(identifier =>
                                 {
                                     //Skipping the comparison of lambda parameter. for example: it => it.Something
                                     if (identifier.ToString() == lambdaParameter) return true;

                                     //If property is null, means property is any kind of method invocation, so we should skip that
                                     if (context.SemanticModel.GetSymbolInfo(identifier).Symbol is IPropertySymbol property && property.GetAttributes().None(attr => attr.AttributeClass.Name == "CalculatedAttribute")) return true;
                                     return false;
                                 }))
                                     ReportDiagnostic(context, nameSyntax);
                             }
                         }
                     }
                 });

                 lambda.Body.DescendantNodes().OfType<ConditionalAccessExpressionSyntax>().ForEach(invocation =>
                 {
                     invocation.ChildTokens().Where(token => token.IsKind(SyntaxKind.QuestionToken)).ForEach(token =>
                     {
                         ReportDiagnostic(context, token);
                     });
                 });
             });
        }

        private IdentifierNameSyntax GetIdentifier(SyntaxNode node)
        {
            var identifiers = node.ChildNodes().OfType<IdentifierNameSyntax>();
            if (identifiers.HasMany()) return identifiers.Last();
            else return identifiers.FirstOrDefault();
        }

        public bool CanRunInDbMode(SimpleLambdaExpressionSyntax lambda, SemanticModel semanticModel)
        {
            if (lambda == null) return false;

            var canRunInDbMode = true;

            var nodes = lambda.DescendantNodes();
            if (nodes.OfType<ParenthesizedExpressionSyntax>().Any()) canRunInDbMode = false;
            if (nodes.OfType<BinaryExpressionSyntax>().Any(it => it.IsKind(SyntaxKind.LogicalNotExpression)))
                canRunInDbMode = false;

            if (nodes.OfType<InvocationExpressionSyntax>().TrueForAtLeastOnce(it =>
            {
                var method = semanticModel.GetSymbolInfo(it).Symbol;
                if (method == null || (method.Name == "Get" && method.ContainingAssembly.Name == MSharp)) return false;
                if (method.ContainingAssembly.Name == MSharp) return true;

                return true;
            })) canRunInDbMode = false;

            return canRunInDbMode;
        }
    }
}
