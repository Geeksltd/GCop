namespace GCop.MSharp.Rules.Performance
{
    using Core;
    using Core.Attributes;
    using GCop.MSharp.Rules.Refactoring;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [MSharpExclusive]
    [ZebbleExclusive]
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AvoidCallingCountAfterGetListAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        private readonly string EmptyParenthesized = "()";
        protected override SyntaxKind Kind => SyntaxKind.InvocationExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "315",
                Category = Category.Performance,
                Message = "It should be written instead as: Database.Count<{0}>{1}",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var invocation = (InvocationExpressionSyntax)context.Node;

            if (invocation.ArgumentList.Arguments.FirstOrDefault()?.Expression is SimpleLambdaExpressionSyntax countLambdaExpression)
            {
                if (countLambdaExpression.ChildNodes().Any(it => it.Kind() == SyntaxKind.LogicalOrExpression)) return;
            }

            var memberAccess = invocation.Expression as MemberAccessExpressionSyntax;
            if (memberAccess == null) return;

            var methodInfo = context.SemanticModel.GetSymbolInfo(memberAccess).Symbol as IMethodSymbol;
            if (methodInfo == null || methodInfo.Name != "Count" || methodInfo.ContainingNamespace.ToString() != "System.Linq") return;

            var innerInvocation = memberAccess.ChildNodes().OfType<InvocationExpressionSyntax>().FirstOrDefault();
            var getListInvocation = innerInvocation?.Expression as MemberAccessExpressionSyntax;
            if (getListInvocation == null) return;

            var getListInfo = context.SemanticModel.GetSymbolInfo(getListInvocation).Symbol as IMethodSymbol;
            if (getListInfo == null || getListInfo.Name != "GetList" || getListInfo.ContainingType.Name != "Database" || getListInfo.ContainingAssembly.Name != "MSharp.Framework") return;

            var genericType = getListInfo.TypeArguments.First().Name;
            var lambda = innerInvocation.ArgumentList.Arguments.FirstOrDefault()?.Expression as SimpleLambdaExpressionSyntax;
            if (lambda == null)
            {
                lambda = invocation.ArgumentList.Arguments.FirstOrDefault()?.Expression as SimpleLambdaExpressionSyntax;
                if (!CriteriaShouldBeConvertedToSqlAnalyzer.CanRunInDbMode(lambda, context.SemanticModel).IsValid) return;
                ReportDiagnostic(context, invocation, genericType, $"({lambda})");
                return;
            }
            bool getListHasLambdaParameter = lambda != null;

            var countLambda = GetCountArguments(invocation.ArgumentList, lambda?.ToString());
            if (getListHasLambdaParameter)
            {
                var inv = innerInvocation ?? invocation;
                var lambdaParameter = (inv.ArgumentList.Arguments.FirstOrDefault()?.Expression as SimpleLambdaExpressionSyntax)?.Parameter?.ToString();
                if (lambdaParameter.IsEmpty()) return;
                countLambda = countLambda.ReplaceWholeWord(lambda.Parameter.ToString(), lambdaParameter);
            }

            ReportDiagnostic(context, invocation, genericType, countLambda);
        }


        private string GetCountArguments(ArgumentListSyntax arguments, string getListLambda)
        {
            string countLambda = "(...)";
            var countArguments = arguments.ToString();
            if (countArguments == EmptyParenthesized)
                countLambda = $"({getListLambda})";
            else
            {
                if (getListLambda.HasValue())
                    countLambda = countArguments.Remove(countArguments.LastIndexOf(')'), 1) + " && " + getListLambda.Substring(getListLambda.LastIndexOf("=>") + 2) + ")";
                else
                    countLambda = countArguments;
            }

            return countLambda;
        }
    }
}
