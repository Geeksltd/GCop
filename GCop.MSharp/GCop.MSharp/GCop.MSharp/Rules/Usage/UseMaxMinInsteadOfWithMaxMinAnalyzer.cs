namespace GCop.MSharp.Rules.Usage
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
    public class UseMaxMinInsteadOfWithMaxMinAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.InvocationExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "519",
                Category = Category.Usage,
                Severity = DiagnosticSeverity.Warning,
                Message = "Use {0}{1} instead."
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var invocExpres = NodeToAnalyze as InvocationExpressionSyntax;

            if (invocExpres == null)
                return;

            var memberAccessExpression = invocExpres.ChildNodes().OfType<MemberAccessExpressionSyntax>().FirstOrDefault();
            if (memberAccessExpression == null) return;
            var methodIdentifier = memberAccessExpression?.ChildNodes().LastOrDefault(it => (it is IdentifierNameSyntax) || (it is GenericNameSyntax));

            if (methodIdentifier == null)
                return;

            var isMinMaxMethod = CheckingWithMaxMinMethodName(methodIdentifier, context);
            if (isMinMaxMethod.IsEmpty()) return;

            if (invocExpres.Parent == null) return;

            ExpressionSyntax parentMemberAccess;

            parentMemberAccess = invocExpres.Parent as MemberAccessExpressionSyntax;
            if (parentMemberAccess == null)
            {
                parentMemberAccess = invocExpres.Parent as ConditionalAccessExpressionSyntax;
            }
            if (parentMemberAccess == null) return;

            var lastIdentifier = parentMemberAccess.GetIdentifierSyntax() as IdentifierNameSyntax;
            if (lastIdentifier == null)
            {
                // Cheking MemberBindingExpressionSyntax for  (?)--> var withMax = Students.WithMax(x => x.Age)?.Name; 
                lastIdentifier = parentMemberAccess.ChildNodes()?.OfType<MemberBindingExpressionSyntax>().LastOrDefault()?.GetIdentifierSyntax();
            }
            if (lastIdentifier == null) return;
            // Checking the [Age] property , inside of expression with after () : Students.WithMax(x => x.Age)?.Age;
            if (invocExpres.ArgumentList == null) return;
            if (invocExpres.ArgumentList.IsNone()) return;

            if (invocExpres.ArgumentList.Arguments == null) return;
            if (invocExpres.ArgumentList.Arguments.None()) return;

            var item = invocExpres.ArgumentList?.ChildNodes()?.FirstOrDefault();
            if (item == null) return;

            var afterLamda = (item as ArgumentSyntax)?.Expression as SimpleLambdaExpressionSyntax;
            if (afterLamda == null) return;

            var property = afterLamda.ChildNodes()?.LastOrDefault() as MemberAccessExpressionSyntax;
            if (property == null) return;

            var propertyIdentifier = property.ChildNodes()?.LastOrDefault();
            if (propertyIdentifier == null) return;
            if ((propertyIdentifier as IdentifierNameSyntax)?.Identifier == null) return;
            if (lastIdentifier?.Identifier == null) return;

            if (lastIdentifier.Identifier.ValueText != (propertyIdentifier as IdentifierNameSyntax).Identifier.ValueText) return;

            var isOrNull = CheckingNullMaxMinMethodName(invocExpres);
            if (isOrNull.IsEmpty()) return;

            var minOrMax = isMinMaxMethod.Substring(4, 3);
            var argumentLambdaExper = invocExpres.ArgumentList.ToString();
            ReportDiagnostic(context, methodIdentifier, minOrMax + isOrNull.Trim(), argumentLambdaExper);
        }

        private string CheckingWithMaxMinMethodName(SyntaxNode identifier, SyntaxNodeAnalysisContext context)
        {
            var methodInfo = context.SemanticModel.GetSymbolInfo(identifier).Symbol as IMethodSymbol;

            if (methodInfo == null
                || methodInfo.Name.IsNoneOf("WithMax", "WithMin")
                || !methodInfo.IsExtensionMethod
                || !methodInfo.ContainingAssembly.ToString().StartsWith("MSharp.Framework")) return null;

            return identifier.GetIdentifier();
        }

        private string CheckingNullMaxMinMethodName(InvocationExpressionSyntax invocation)
        {
            if (invocation == null) return null;

            var condition = invocation.Parent as ConditionalAccessExpressionSyntax;
            if (condition == null) return " ";
            else return "OrNull ";
        }
    }
}