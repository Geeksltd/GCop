namespace GCop.String.Rules.Usage
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StringFormatArgumentsAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.InvocationExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "504",
                Category = Category.Usage,
                Severity = DiagnosticSeverity.Warning,
                Message = "{0}"
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            bool isFormatWith = false;
            var invocationExpression = (InvocationExpressionSyntax)context.Node;
            var memberExpresion = invocationExpression.Expression as MemberAccessExpressionSyntax;
            var methodName = memberExpresion?.Name?.ToString();
            if (methodName.IsEmpty() || methodName.IsNoneOf("Format", "FormatWith")) return;

            isFormatWith = methodName == "FormatWith";

            var memberSymbol = context.SemanticModel.GetSymbolInfo(memberExpresion).Symbol;
            if (memberSymbol == null) return;
            var memberSignature = memberSymbol.ToString();
            if (!memberSignature.StartsWith("string.Format(string, ") && !memberSignature.StartsWith("string.FormatWith(object, ")) return;
            var argumentList = invocationExpression.ArgumentList as ArgumentListSyntax;
            if (argumentList == null) return;
            var arguments = argumentList.Arguments;
            if (!isFormatWith && (!arguments[0]?.Expression?.IsKind(SyntaxKind.StringLiteralExpression) ?? false)) return;
            if (memberSignature == "string.Format(string, params object[])" && arguments.Count == 2 && context.SemanticModel.GetTypeInfo(arguments[1].Expression).Type.TypeKind == TypeKind.Array) return;

            LiteralExpressionSyntax formatLiteral = null;

            if (isFormatWith)
                formatLiteral = (invocationExpression.Expression as MemberAccessExpressionSyntax)?.ChildNodes().FirstOrDefault() as LiteralExpressionSyntax;
            else
                formatLiteral = (LiteralExpressionSyntax)arguments[0].Expression;

            //We should only look at string literals
            if (formatLiteral == null) return;

            var analyzingInterpolation = (InterpolatedStringExpressionSyntax)SyntaxFactory.ParseExpression($"${formatLiteral.Token.Text}");
            var allInterpolations = analyzingInterpolation.Contents.Where(c => c.IsKind(SyntaxKind.Interpolation)).Cast<InterpolationSyntax>();
            var distinctInterpolations = allInterpolations.Select(c => c.Expression.ToString()).Distinct();

            int numberOfArguments = isFormatWith ? arguments.Count : arguments.Count - 1;

            if (distinctInterpolations.Count() < numberOfArguments)
            {
                var newDescription = GetDescription().ChangeMessage($"The number of arguments in {methodName} is incorrect.");
                ReportDiagnostic(context, newDescription, invocationExpression.GetLocation());
                return;
            }

            foreach (var interpolation in distinctInterpolations)
            {
                var validIndexReference = false;
                int argIndexReference = 0;

                argIndexReference = interpolation.TryParseAs<int>().Value;
                validIndexReference = argIndexReference >= 0 && argIndexReference < numberOfArguments;

                if (validIndexReference) return;

                var newDescription = GetDescription().ChangeMessage($"Invalid argument reference in {methodName}");
                ReportDiagnostic(context, newDescription, invocationExpression.GetLocation());
                return;
            }
        }
    }
}
