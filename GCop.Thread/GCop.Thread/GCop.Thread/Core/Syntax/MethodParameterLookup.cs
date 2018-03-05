namespace GCop.Thread.Core.Syntax
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using System.Linq;

    internal class MethodParameterLookup
    {
        private readonly InvocationExpressionSyntax Invocation;
        readonly IMethodSymbol MethodSymbol;

        public MethodParameterLookup(InvocationExpressionSyntax invocation, SemanticModel semanticModel)
        {
            Invocation = invocation;
            MethodSymbol = semanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
        }

        public static IParameterSymbol GetParameterSymbol(ArgumentSyntax argument, ArgumentListSyntax argumentList, IMethodSymbol method)
        {
            if (!argumentList.Arguments.Contains(argument) ||
                method == null)
            {
                return null;
            }

            if (argument.NameColon != null)
            {
                return method.Parameters
                    .FirstOrDefault(symbol => symbol.Name == argument.NameColon.Name.Identifier.ValueText);
            }

            var argumentIndex = argumentList.Arguments.IndexOf(argument);
            var parameterIndex = argumentIndex;

            if (parameterIndex >= method.Parameters.Length)
            {
                var pp = method.Parameters.Last();
                return pp.IsParams ? pp : null;
            }
            return method.Parameters[parameterIndex];
        }

        public IParameterSymbol GetParameterSymbol(ArgumentSyntax argument)
        {
            return GetParameterSymbol(argument, Invocation.ArgumentList, MethodSymbol);
        }
    }
}
