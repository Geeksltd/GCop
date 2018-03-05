namespace GCop.String.Rules.Usage
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;


    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UseStringCollectionIntersectsAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.InvocationExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "520",
                Category = Category.Usage,
                Severity = DiagnosticSeverity.Warning,
                Message = "Write it as {0}.Intersects({1})"
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var invocExpres = NodeToAnalyze as InvocationExpressionSyntax;

            if (invocExpres == null)
                return;

            if (invocExpres.ArgumentList == null) return;
            if (invocExpres.ArgumentList.IsNone()) return;

            var memberAccessExpression = invocExpres.ChildNodes().OfType<MemberAccessExpressionSyntax>().FirstOrDefault();
            if (memberAccessExpression == null) return;

            //checking the type of varibale (string collection)
            var variableIdentifier = memberAccessExpression.ChildNodes()?.FirstOrDefault()?.GetIdentifierSyntax();
            if (variableIdentifier == null) return;

            var variableInfo = context.SemanticModel.GetSymbolInfo(variableIdentifier).Symbol;
            if (variableInfo == null) return;

            //if (variableInfo?.Type?.ContainingSymbol?.ToString().Lacks("Collection") ?? true) return;            
            if (!variableInfo.IsInherited("ICollection", "System.Collections.ICollection")) return;

            if (variableInfo.GetSymbolType() == null) return;
            if (variableInfo.GetSymbolType().ToString().IsEmpty()) return;

            if (variableInfo.GetSymbolType().ToString().ToLower().Lacks("string")) return;

            var isAnyMethod = memberAccessExpression.ChildNodes()?.LastOrDefault()?.GetIdentifier();
            if (isAnyMethod.IsEmpty())
            {
                // Cheking for Generic types : 
                var isGeneric = memberAccessExpression.ChildNodes().LastOrDefault() as GenericNameSyntax;
                if (isGeneric == null) return;
                isAnyMethod = isGeneric.Identifier.ValueText;
            }

            if (isAnyMethod.IsEmpty()) return;
            if (isAnyMethod != "Any")
                if (isAnyMethod.ToLower() != "Any<string>".ToLower()) return;

            var arguments = invocExpres.ArgumentList;
            if (arguments == null) return;
            if (arguments.IsNone()) return;

            var firstArg = arguments.ChildNodes()?.FirstOrDefault() as ArgumentSyntax;
            if (firstArg == null) return;

            var lambdaNode = firstArg.Expression as SimpleLambdaExpressionSyntax;
            if (lambdaNode == null) return;

            var lambdaInvocationExperssion = lambdaNode.ChildNodes()?.OfType<InvocationExpressionSyntax>()?.FirstOrDefault();
            if (lambdaInvocationExperssion == null) return;

            var internalMemberAccess = lambdaInvocationExperssion.ChildNodes()?.OfType<MemberAccessExpressionSyntax>()?.FirstOrDefault();
            if (internalMemberAccess == null) return;

            var isContainsMethod = internalMemberAccess.ChildNodes()?.LastOrDefault()?.GetIdentifier();
            if (isContainsMethod.IsEmpty()) return;
            if (isContainsMethod != "Contains") return;

            var internalArgument = lambdaInvocationExperssion.ArgumentList?.ChildNodes()?.FirstOrDefault();
            if (internalArgument == null) return;

            if (lambdaNode.Parameter.Identifier.ValueText == internalArgument.GetIdentifier())
                ReportDiagnostic(context, invocExpres, variableIdentifier.Identifier.ValueText, internalMemberAccess.GetIdentifier());
        }
    }
}
