namespace GCop.Linq.Rules.Refactoring
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UseExceptORContainsInsteadAnyAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {

        protected override SyntaxKind Kind => SyntaxKind.InvocationExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "624",
                Category = Category.Refactoring,
                Severity = DiagnosticSeverity.Warning,
                //myList.Except(VALUE).Any()    // OR  //myList.Contains(VALUE) 
                Message = "Write it as  {0}.{1}(VALUE){2}"
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var invocExpres = NodeToAnalyze as InvocationExpressionSyntax;

            if (invocExpres == null)
                return;

            if (invocExpres.ArgumentList.IsNone()) return;
            if (invocExpres.ArgumentList.Arguments == null) return;

            var memberAccessExpression = invocExpres.ChildNodes().OfType<MemberAccessExpressionSyntax>().FirstOrDefault();
            if (memberAccessExpression == null) return;

            //Any<T>() checking or Any()
            var anyMethod = memberAccessExpression.ChildNodes().LastOrDefault();
            if (anyMethod == null) return;

            if (anyMethod.Kind() == SyntaxKind.GenericName) return;
            if (anyMethod.ToString().Lacks("Any")) return;

            // Parsing Lambda experssion
            var arguments = invocExpres.ArgumentList;
            var firstArg = arguments.ChildNodes().FirstOrDefault() as ArgumentSyntax;
            if (firstArg == null) return;

            var lambdaNodeExpres = firstArg.Expression as SimpleLambdaExpressionSyntax;
            if (lambdaNodeExpres == null) return;

            //var lambdaString = lambdaNodeExpres.ToString();

            // chcking for this : myList.Any(x => x != VALUE
            if (lambdaNodeExpres.ChildNodes().OfKind(SyntaxKind.NotEqualsExpression).Any())
            {
                var param = lambdaNodeExpres.Parameter;
                var notEqulaExper = lambdaNodeExpres.ChildNodes().OfKind(SyntaxKind.NotEqualsExpression).FirstOrDefault();
                if (notEqulaExper.IsNone()) return;
                if (notEqulaExper.ChildNodes().OfKind(SyntaxKind.SimpleMemberAccessExpression).Any()) return;

                var left = (notEqulaExper as BinaryExpressionSyntax).Left.GetIdentifierSyntax();
                var right = (notEqulaExper as BinaryExpressionSyntax).Right.GetIdentifierSyntax();
                if (left == null || right == null) return;

                var isInLeft = HasNodeTakePartInExpression((notEqulaExper as BinaryExpressionSyntax).Left, param);
                var isInRight = HasNodeTakePartInExpression((notEqulaExper as BinaryExpressionSyntax).Right, param);

                if (isInLeft || isInRight)
                    ReportDiagnostic(context, firstArg.Parent,
                               memberAccessExpression.ChildNodes().FirstOrDefault().GetIdentifier(),
                               "Except", ".Any()");
            }

            if (lambdaNodeExpres.ChildNodes().OfKind(SyntaxKind.EqualsExpression).None()) return;
            {
                var param = lambdaNodeExpres.Parameter;
                var notEqulaExper = lambdaNodeExpres.ChildNodes().OfKind(SyntaxKind.EqualsExpression).FirstOrDefault();
                if (notEqulaExper.IsNone()) return;
                if (notEqulaExper.ChildNodes().OfKind(SyntaxKind.SimpleMemberAccessExpression).Any()) return;

                var left = (notEqulaExper as BinaryExpressionSyntax).Left.GetIdentifierSyntax();
                var right = (notEqulaExper as BinaryExpressionSyntax).Right.GetIdentifierSyntax();
                if (left == null || right == null) return;

                var isInLeft = HasNodeTakePartInExpression((notEqulaExper as BinaryExpressionSyntax).Left, param);
                var isInRight = HasNodeTakePartInExpression((notEqulaExper as BinaryExpressionSyntax).Right, param);

                if (isInLeft || isInRight)

                    ReportDiagnostic(context, firstArg.Parent,
                        memberAccessExpression.ChildNodes().FirstOrDefault().GetIdentifier(),
                        "Contains", "");
            }
        }

        bool HasNodeTakePartInExpression(ExpressionSyntax expression, ParameterSyntax node)
        {
            if (node == null) return false;
            if (expression == null) return false;

            return expression.DescendantNodesAndSelf().Any(x => x.GetIdentifier() == node.Identifier.ValueText);
        }
    }
}