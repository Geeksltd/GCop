namespace GCop.Linq.Rules.Style
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RedundantCastAnalyzer : GCopAnalyzer
    {
        private static readonly string[] CastIEnumerableMethods =
        {
            "Cast", "OfType"
        };

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "418",
                Category = Category.Style,
                Severity = DiagnosticSeverity.Warning,
                Message = "Remove the unnecessary casting."
            };
        }

        protected override void Configure()
        {
            RegisterSyntaxNodeAction(AnalyzeCastExpression, SyntaxKind.CastExpression);
            RegisterSyntaxNodeAction(AnalyzeInvocationExpression, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeCastExpression(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var castExpression = (CastExpressionSyntax)NodeToAnalyze;

            var expressionType = context.SemanticModel.GetTypeInfo(castExpression.Expression).Type;
            if (expressionType == null)
            {
                return;
            }

            var castType = context.SemanticModel.GetTypeInfo(castExpression.Type).Type;
            if (castType == null)
            {
                return;
            }

            if (expressionType.Equals(castType))
            {
                ReportDiagnostic(context, castExpression.Type.GetLocation(), castType.ToDisplayString());
            }
        }

        private void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;
            var methodSymbol = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
            if (methodSymbol == null ||
                !MethodIsOnIEnumerable(methodSymbol, context.SemanticModel) ||
                CastIEnumerableMethods.Lacks(methodSymbol.Name))
            {
                return;
            }

            var elementType = GetElementType(invocation, methodSymbol, context.SemanticModel);
            if (elementType == null)
            {
                return;
            }

            var returnType = methodSymbol.ReturnType as INamedTypeSymbol;
            if (returnType == null || returnType.TypeArguments.None())
            {
                return;
            }

            var castType = returnType.TypeArguments.First();
            if (elementType.Equals(castType))
            {
                var methodCalledAsStatic = methodSymbol.MethodKind == MethodKind.Ordinary;
                ReportDiagnostic(context, GetReportLocation(invocation, methodCalledAsStatic), returnType.ToDisplayString());
            }
        }

        private static Location GetReportLocation(InvocationExpressionSyntax invocation, bool methodCalledAsStatic)
        {
            var memberAccess = invocation.Expression as MemberAccessExpressionSyntax;
            if (memberAccess == null)
            {
                return invocation.Expression.GetLocation();
            }

            if (methodCalledAsStatic)
                return memberAccess.GetLocation();

            return Location.Create(invocation.SyntaxTree, new TextSpan(memberAccess.OperatorToken.SpanStart, invocation.Span.End - memberAccess.OperatorToken.SpanStart));
        }

        private static ITypeSymbol GetElementType(InvocationExpressionSyntax invocation, IMethodSymbol methodSymbol, SemanticModel semanticModel)
        {
            ExpressionSyntax collection;
            if (methodSymbol.MethodKind == MethodKind.Ordinary)
            {
                if (!invocation.ArgumentList.Arguments.Any())
                {
                    return null;
                }
                collection = invocation.ArgumentList.Arguments.First().Expression;
            }
            else
            {
                var memberAccess = invocation.Expression as MemberAccessExpressionSyntax;
                if (memberAccess == null)
                {
                    return null;
                }
                collection = memberAccess.Expression;
            }

            if (semanticModel.GetTypeInfo(collection).Type is INamedTypeSymbol collectionType && collectionType.TypeArguments.IsSingle())
            {
                return collectionType.TypeArguments.First();
            }

            var arrayType = semanticModel.GetTypeInfo(collection).Type as IArrayTypeSymbol;
            return arrayType?.ElementType;
        }

        private static bool MethodIsOnIEnumerable(IMethodSymbol methodSymbol, SemanticModel semanticModel)
        {
            if (methodSymbol == null)
            {
                return false;
            }

            var enumerableType = semanticModel.Compilation.GetSpecialType(SpecialType.System_Collections_IEnumerable);
            var receiverType = methodSymbol.ReceiverType as INamedTypeSymbol;

            if (methodSymbol.MethodKind == MethodKind.Ordinary)
            {
                if (methodSymbol.Parameters.Count() != 1)
                {
                    return false;
                }
                receiverType = methodSymbol.Parameters.First().Type as INamedTypeSymbol;
            }

            return receiverType != null && receiverType.ConstructedFrom.Equals(enumerableType);
        }
    }
}
