namespace GCop.Linq.Rules.Refactoring
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Collections.Generic;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EnumerableQuerySimplificationAnalyzer : GCopAnalyzer
    {
        private static readonly string[] MethodNamesWithPredicate =
        {
            "Any", "LongCount", "Count",
            "First", "FirstOrDefault", "Last", "LastOrDefault",
            "Single", "SingleOrDefault"
        };
        private static readonly string[] MethodNamesForTypeCheckingWithSelect =
        {
            "Any", "LongCount", "Count",
            "First", "FirstOrDefault", "Last", "LastOrDefault",
            "Single", "SingleOrDefault", "SkipWhile", "TakeWhile"
        };
        private static readonly string[] MethodNamesToCollection =
        {
            "ToList", "ToArray"
        };

        private static readonly SyntaxKind[] AsIsSyntaxKinds = { SyntaxKind.AsExpression, SyntaxKind.IsExpression };
        private const string WhereMethodName = "Where";
        private const string SelectMethodName = "Select";

        internal const string MessageUseInstead = "Use {0} here instead.";
        internal const string MessageDropAndChange = "Drop \"{0}\" and move the condition into the \"{1}\".";
        internal const string MessageDropFromMiddle = "Calling {0} is unnecessary here.";
        private static readonly ExpressionSyntax NullExpression = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "621",
                Category = Category.Refactoring,
                Severity = DiagnosticSeverity.Warning,
                Message = "{0}"
            };
        }

        protected override void Configure()
        {
            //RegisterSyntaxNodeAction(CheckCountCall, SyntaxKind.InvocationExpression);
            // RegisterSyntaxNodeAction(CheckToCollectionCalls, SyntaxKind.InvocationExpression);
            RegisterSyntaxNodeAction(CheckExtensionMethodsOnIEnumerable, SyntaxKind.InvocationExpression);
        }

        private void CheckToCollectionCalls(SyntaxNodeAnalysisContext context)
        {
            var outerInvocation = (InvocationExpressionSyntax)context.Node;
            var outerMethodSymbol = context.SemanticModel.GetSymbolInfo(outerInvocation).Symbol as IMethodSymbol;
            if (outerMethodSymbol == null ||
                !MethodCanBeCalledOnIEnumerable(outerMethodSymbol, context.SemanticModel))
            {
                return;
            }

            var innerInvocation = GetInnerInvocation(outerInvocation, outerMethodSymbol);
            if (innerInvocation == null)
            {
                return;
            }

            var innerMethodSymbol = context.SemanticModel.GetSymbolInfo(innerInvocation).Symbol as IMethodSymbol;
            if (innerMethodSymbol == null ||
                !MethodCanBeCalledOnIEnumerable(innerMethodSymbol, context.SemanticModel))
            {
                return;
            }

            if (MethodNamesToCollection.Contains(innerMethodSymbol.Name))
            {
                ReportDiagnostic(context, GetReportLocation(innerInvocation), string.Format(MessageDropFromMiddle, innerMethodSymbol.Name));
            }
        }

        private void CheckExtensionMethodsOnIEnumerable(SyntaxNodeAnalysisContext context)
        {
            var outerInvocation = (InvocationExpressionSyntax)context.Node;
            var outerMethodSymbol = context.SemanticModel.GetSymbolInfo(outerInvocation).Symbol as IMethodSymbol;
            if (outerMethodSymbol == null ||
                !MethodIsOnIEnumerable(outerMethodSymbol, context.SemanticModel))
            {
                return;
            }

            if (outerMethodSymbol.Name == "Select" && outerMethodSymbol.ContainingNamespace.ToString() == "System.Linq")
            {
                var lambda = outerInvocation.ArgumentList.Arguments.FirstOrDefault()?.Expression as SimpleLambdaExpressionSyntax;
                //@15945 : Rule should be skipped when the type is char
                if (lambda?.Body is CastExpressionSyntax castExpression && castExpression.Type.ToString().IsNoneOf("Char", "char"))
                {
                    ReportDiagnostic(context, GetReportLocation(outerInvocation), $"Use Cast<{castExpression.Type}>() here instead.");
                    return;
                }
            }

            var innerInvocation = GetInnerInvocation(outerInvocation, outerMethodSymbol);
            if (innerInvocation == null)
            {
                return;
            }

            var innerMethodSymbol = context.SemanticModel.GetSymbolInfo(innerInvocation).Symbol as IMethodSymbol;
            if (innerMethodSymbol == null ||
                !MethodIsOnIEnumerable(innerMethodSymbol, context.SemanticModel))
            {
                return;
            }

            if (CheckForSimplifiable(outerMethodSymbol, outerInvocation, innerMethodSymbol, innerInvocation, context))
            {
                return;
            }

            if (CheckForCastSimplification(outerMethodSymbol, innerMethodSymbol, innerInvocation,
                outerInvocation, context))
            {
                return;
            }
        }

        private bool MethodCanBeCalledOnIEnumerable(IMethodSymbol methodSymbol, SemanticModel semanticModel)
        {
            if (methodSymbol == null)
            {
                return false;
            }

            var enumerableType = semanticModel.Compilation
                .GetSpecialType(SpecialType.System_Collections_Generic_IEnumerable_T);

            var receiverType = methodSymbol.ReceiverType as INamedTypeSymbol;

            if (methodSymbol.MethodKind == MethodKind.Ordinary &&
                methodSymbol.IsExtensionMethod)
            {
                receiverType = methodSymbol.Parameters.First().Type as INamedTypeSymbol;
            }

            return receiverType != null && receiverType.ConstructedFrom.DerivesOrImplementsAny(enumerableType);
        }

        private InvocationExpressionSyntax GetInnerInvocation(InvocationExpressionSyntax outerInvocation, IMethodSymbol outerMethodSymbol)
        {
            InvocationExpressionSyntax innerInvocation = null;
            if (outerMethodSymbol.MethodKind == MethodKind.ReducedExtension)
            {
                if (outerInvocation.Expression is MemberAccessExpressionSyntax memberAccess)
                {
                    innerInvocation = memberAccess.Expression as InvocationExpressionSyntax;
                }
            }
            else
            {
                var argument = outerInvocation.ArgumentList.Arguments.FirstOrDefault();
                if (argument != null)
                {
                    innerInvocation = argument.Expression as InvocationExpressionSyntax;
                }
                else
                {
                    if (outerInvocation.Expression is MemberAccessExpressionSyntax memberAccess)
                    {
                        innerInvocation = memberAccess.Expression as InvocationExpressionSyntax;
                    }
                }
            }
            return innerInvocation;
        }

        private List<ArgumentSyntax> GetReducedArguments(IMethodSymbol methodSymbol, InvocationExpressionSyntax invocation)
        {
            if (methodSymbol.MethodKind == MethodKind.ReducedExtension)
                return invocation.ArgumentList.Arguments.ToList();

            return invocation.ArgumentList.Arguments.Skip(1).ToList();
        }

        private bool CheckForCastSimplification(IMethodSymbol outerMethodSymbol, IMethodSymbol innerMethodSymbol,
            InvocationExpressionSyntax innerInvocation, InvocationExpressionSyntax outerInvocation, SyntaxNodeAnalysisContext context)
        {
            var tryGetCast = TryGetCastInLambda(SyntaxKind.AsExpression, innerMethodSymbol, innerInvocation);
            if (MethodNamesForTypeCheckingWithSelect.Contains(outerMethodSymbol.Name) &&
    innerMethodSymbol.Name == SelectMethodName &&
    IsFirstExpressionInLambdaIsNullChecking(outerMethodSymbol, outerInvocation) && tryGetCast.ReturnValue)
            {
                ReportDiagnostic(context, GetReportLocation(innerInvocation), string.Format(MessageUseInstead, $"\"OfType<{tryGetCast.TypeName}>()\""));
                return true;
            }

            var isExpression = IsExpressionInLambdaIsCast(outerMethodSymbol, outerInvocation);
            var tryGetCastIn = TryGetCastInLambda(SyntaxKind.IsExpression, innerMethodSymbol, innerInvocation);
            if (outerMethodSymbol.Name == SelectMethodName && innerMethodSymbol.Name == WhereMethodName && isExpression.ReturnValue && tryGetCastIn.ReturnValue && isExpression.TypeName == tryGetCastIn.TypeName)
            {
                ReportDiagnostic(context, GetReportLocation(innerInvocation), string.Format(MessageUseInstead, $"\"OfType<{tryGetCastIn.TypeName}>()\""));
                return true;
            }

            return false;
        }

        private Location GetReportLocation(InvocationExpressionSyntax invocation)
        {
            var memberAccess = invocation.Expression as MemberAccessExpressionSyntax;
            return memberAccess == null
                ? invocation.Expression.GetLocation()
                : memberAccess.Name.GetLocation();
        }

        private LambdaCast IsExpressionInLambdaIsCast(IMethodSymbol methodSymbol, InvocationExpressionSyntax invocation)
        {
            var lambdaCast = new LambdaCast
            {
                ReturnValue = false,
                TypeName = null
            };

            var firstTry = TryGetCastInLambda(SyntaxKind.AsExpression, methodSymbol, invocation);
            var secondTry = TryGetCastInLambda(methodSymbol, invocation);
            lambdaCast.ReturnValue = firstTry.ReturnValue || secondTry.ReturnValue;
            lambdaCast.TypeName = firstTry.TypeName;
            lambdaCast.TypeName = secondTry.TypeName;
            return lambdaCast;
        }

        private bool IsFirstExpressionInLambdaIsNullChecking(IMethodSymbol methodSymbol, InvocationExpressionSyntax invocation)
        {
            var arguments = GetReducedArguments(methodSymbol, invocation);
            if (arguments.Count != 1)
            {
                return false;
            }
            var expression = arguments.First().Expression;

            var binaryExpression = GetExpressionFromParens(GetExpressionFromLambda(expression)) as BinaryExpressionSyntax;
            var lambdaParameter = GetLambdaParameter(expression);

            while (binaryExpression != null)
            {
                if (!binaryExpression.IsKind(SyntaxKind.LogicalAndExpression))
                {
                    return binaryExpression.IsKind(SyntaxKind.NotEqualsExpression) &&
                           IsNullChecking(binaryExpression, lambdaParameter);
                }
                binaryExpression = GetExpressionFromParens(binaryExpression.Left) as BinaryExpressionSyntax;
            }
            return false;
        }

        private bool IsNullChecking(BinaryExpressionSyntax binaryExpression, string lambdaParameter)
        {
            if (NullExpression.IsEquivalent(GetExpressionFromParens(binaryExpression.Left)) &&
                GetExpressionFromParens(binaryExpression.Right).ToString() == lambdaParameter)
            {
                return true;
            }
            if (NullExpression.IsEquivalent(GetExpressionFromParens(binaryExpression.Right)) &&
                GetExpressionFromParens(binaryExpression.Left).ToString() == lambdaParameter)
            {
                return true;
            }
            return false;
        }

        private ExpressionSyntax GetExpressionFromParens(ExpressionSyntax expression)
        {
            var parens = expression as ParenthesizedExpressionSyntax;
            var current = expression;
            while (parens != null)
            {
                current = parens.Expression;
                parens = current as ParenthesizedExpressionSyntax;
            }

            return current;
        }

        private ExpressionSyntax GetExpressionFromLambda(ExpressionSyntax expression)
        {
            ExpressionSyntax lambdaBody;
            var lambda = expression as SimpleLambdaExpressionSyntax;
            if (lambda == null)
            {
                var parenthesizedLambda = expression as ParenthesizedLambdaExpressionSyntax;
                if (parenthesizedLambda == null)
                {
                    return null;
                }
                lambdaBody = parenthesizedLambda.Body as ExpressionSyntax;
            }
            else
            {
                lambdaBody = lambda.Body as ExpressionSyntax;
            }

            return lambdaBody;
        }
        private string GetLambdaParameter(ExpressionSyntax expression)
        {
            if (expression is SimpleLambdaExpressionSyntax lambda)
            {
                return lambda.Parameter.Identifier.ValueText;
            }

            var parenthesizedLambda = expression as ParenthesizedLambdaExpressionSyntax;
            if (parenthesizedLambda == null ||
                !parenthesizedLambda.ParameterList.Parameters.Any())
            {
                return null;
            }
            return parenthesizedLambda.ParameterList.Parameters.First().Identifier.ValueText;
        }

        private LambdaCast TryGetCastInLambda(SyntaxKind asOrIs, IMethodSymbol methodSymbol, InvocationExpressionSyntax invocation)
        {
            var lambdaCast = new LambdaCast
            {
                TypeName = null,
                ReturnValue = true
            };
            if (AsIsSyntaxKinds.Lacks(asOrIs))
            {
                lambdaCast.ReturnValue = false;
            }

            var arguments = GetReducedArguments(methodSymbol, invocation);
            if (arguments.Count != 1)
            {
                lambdaCast.ReturnValue = false;
            }

            var expression = arguments.First().Expression;
            var lambdaBody = GetExpressionFromParens(GetExpressionFromLambda(expression)) as BinaryExpressionSyntax;
            var lambdaParameter = GetLambdaParameter(expression);
            if (lambdaBody == null || lambdaParameter.IsEmpty() ||
                !lambdaBody.IsKind(asOrIs))
            {
                lambdaCast.ReturnValue = false;
            }

            var castedExpression = GetExpressionFromParens(lambdaBody.Left);
            if (lambdaParameter != castedExpression.ToString())
            {
                lambdaCast.ReturnValue = false;
            }

            lambdaCast.TypeName = lambdaBody.Right.ToString();
            return lambdaCast;
        }
        private LambdaCast TryGetCastInLambda(IMethodSymbol methodSymbol, InvocationExpressionSyntax invocation)
        {
            var lambdaCast = new LambdaCast
            {
                TypeName = null,
                ReturnValue = true
            };

            var arguments = GetReducedArguments(methodSymbol, invocation);
            if (arguments.Count != 1)
            {
                lambdaCast.ReturnValue = false;
            }

            var expression = arguments.First().Expression;
            var castExpression = GetExpressionFromParens(GetExpressionFromLambda(expression)) as CastExpressionSyntax;
            var lambdaParameter = GetLambdaParameter(expression);
            if (castExpression == null || lambdaParameter.IsEmpty())
            {
                lambdaCast.ReturnValue = false;
            }

            var castedExpression = GetExpressionFromParens(castExpression.Expression);
            if (lambdaParameter != castedExpression.ToString())
            {
                lambdaCast.ReturnValue = false;
            }

            lambdaCast.TypeName = castExpression.Type.ToString();
            return lambdaCast;
        }

        private bool CheckForSimplifiable(IMethodSymbol outerMethodSymbol, InvocationExpressionSyntax outerInvocation,
            IMethodSymbol innerMethodSymbol, InvocationExpressionSyntax innerInvocation, SyntaxNodeAnalysisContext context)
        {
            if (MethodIsNotUsingPredicate(outerMethodSymbol, outerInvocation) &&
                innerMethodSymbol.Name == WhereMethodName &&
                innerMethodSymbol.Parameters.Any(symbol =>
                {
                    var namedType = symbol.Type as INamedTypeSymbol;
                    if (namedType == null)
                    {
                        return false;
                    }
                    return namedType.TypeArguments.Count() == 2;
                }))
            {
                ReportDiagnostic(context, GetReportLocation(innerInvocation), string.Format(MessageDropAndChange, WhereMethodName, outerMethodSymbol.Name));
                return true;
            }

            return false;
        }

        private bool MethodIsNotUsingPredicate(IMethodSymbol methodSymbol, InvocationExpressionSyntax invocation)
        {
            var arguments = GetReducedArguments(methodSymbol, invocation);

            return arguments.None() && MethodNamesWithPredicate.Contains(methodSymbol.Name);
        }

        bool MethodIsOnIEnumerable(IMethodSymbol methodSymbol, SemanticModel semanticModel)
        {
            if (methodSymbol == null)
            {
                return false;
            }

            var enumerableType = semanticModel.Compilation.GetSpecialType(SpecialType.System_Collections_Generic_IEnumerable_T);
            var receiverType = methodSymbol.ReceiverType as INamedTypeSymbol;

            if (methodSymbol.MethodKind == MethodKind.Ordinary &&
                methodSymbol.IsExtensionMethod)
            {
                receiverType = methodSymbol.Parameters.First().Type as INamedTypeSymbol;
            }

            return receiverType != null && receiverType.ConstructedFrom.Equals(enumerableType);
        }
    }

    class LambdaCast
    {
        public bool ReturnValue { get; set; }
        public string TypeName { get; set; }
    }
}
