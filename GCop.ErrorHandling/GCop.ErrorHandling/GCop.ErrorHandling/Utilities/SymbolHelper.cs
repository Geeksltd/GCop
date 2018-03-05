namespace GCop.ErrorHandling.Utilities
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public static class SymbolHelper
    {
        public static async Task<bool> IsFieldSymbolUseInPropertyAsync(IFieldSymbol symbol, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (symbol == null) return false;

            var propertyGetters = symbol.ContainingType.GetMembers().Where(m => (m as IMethodSymbol)?.MethodKind == MethodKind.PropertyGet).Cast<IMethodSymbol>();
            foreach (var getter in propertyGetters)
            {
                var syntaxReference = getter.DeclaringSyntaxReferences.FirstOrDefault();
                var syntax = await syntaxReference?.GetSyntaxAsync(cancellationToken);

                // if symbol is the return statement of the getter, return true
                if (IsReturnOf(symbol, syntax)) return true;

                // if not
                // get the return identifier
                var returnIdentifier = syntax.GetReturnStatement()
                    ?.DescendantNodes().OfType<IdentifierNameSyntax>().FirstOrDefault()
                    ?.Identifier.ValueText;

                // get all assigned tokens in the getter
                // and check if the tokens are assigned to the returns identifier
                var tokenAssignedToReturnStatement = FindTokensAssignedFromSymbol(symbol, syntax).Any(t => t.ValueText == returnIdentifier);
                if (tokenAssignedToReturnStatement) return true;
            }

            return false;
        }

        public static IEnumerable<SyntaxToken> FindTokensAssignedFromSymbol(ISymbol symbol, SyntaxNode node)
        {
            if (node == null) yield break;

            var assignments = node.DescendantNodes().OfType<AssignmentExpressionSyntax>();

            // check right hand side for the symbol name and return them
            foreach (var assignment in assignments)
            {
                if (assignment.Right == null) continue;

                foreach (var innerNode in assignment.Right.DescendantNodes())
                {
                    if (innerNode is IdentifierNameSyntax)
                    {
                        if (((IdentifierNameSyntax)innerNode).Identifier.ValueText == symbol.Name)
                        {
                            var identifierName = assignment.Left.DescendantNodes().OfType<IdentifierNameSyntax>().FirstOrDefault();
                            if (identifierName != null) yield return identifierName.Identifier;
                        }
                    }
                }
            }

            var variableDeclarators = node.DescendantNodes().OfType<VariableDeclaratorSyntax>();

            // check right hand side for the symbol name and return them
            foreach (var decs in variableDeclarators)
            {
                if (decs.Initializer == null) continue;

                foreach (var innerNode in decs.Initializer.DescendantNodes())
                {
                    if (innerNode is IdentifierNameSyntax)
                    {
                        if (((IdentifierNameSyntax)innerNode).Identifier.ValueText == symbol.Name)
                        {
                            yield return decs.Identifier;
                        }
                    }
                }
            }
        }

        public static bool IsReturnOf(ISymbol symbol, SyntaxNode node)
        {
            if (node == null || symbol == null) return false;


            var identifierName = node.GetReturnStatement()
                ?.DescendantNodes()
                .OfType<IdentifierNameSyntax>().FirstOrDefault()
                ?.Identifier.ValueText;

            if (identifierName.IsEmpty() && node.Parent is ArrowExpressionClauseSyntax)
            {
                identifierName = node.DescendantNodes()
                    .OfType<IdentifierNameSyntax>()
                    .FirstOrDefault()
                    ?.Identifier.ValueText;
            }

            if (identifierName.IsEmpty() && node is ArrowExpressionClauseSyntax)
            {
                identifierName = node.DescendantNodes()
                    .OfType<IdentifierNameSyntax>()
                    .FirstOrDefault()
                    ?.Identifier.ValueText;
            }

            if (symbol.Name == identifierName) return true;
            return false;
        }

        public static bool IsAssignableFrom(this ITypeSymbol symbol, Type type)
        {
            while (symbol != null)
            {
                var typeName = (symbol.ContainingType?.Name).Or(symbol.Name);
                if (typeName == type.Name) return true;

                symbol = symbol.BaseType;
            }
            return false;
        }

        public static bool IsAssignableFrom(this ITypeSymbol symbol, string type)
        {
            while (symbol != null)
            {
                var typeName = (symbol.ContainingType?.Name).Or(symbol.Name);
                if (typeName == type) return true;

                symbol = symbol.BaseType;
            }
            return false;
        }

        public static string Normalize(this ITypeSymbol returnType)
        {
            if (returnType == null) return "T";
            var sepratedParts = returnType.ToDisplayParts();
            if (sepratedParts.Length < 2) return "T";
            return sepratedParts[sepratedParts.Length - 2].ToString();
        }
    }
}
