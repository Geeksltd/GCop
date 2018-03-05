namespace GCop.Thread.Rules.Design
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UseInvokeMethodToFireEventAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.InvocationExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "172",
                Category = Category.Design,
                Message = "Remove the check for null. Instead use {0}?.Invoke()",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var invocation = (InvocationExpressionSyntax)context.Node;
            var identifier = invocation.Expression as IdentifierNameSyntax;
            if (identifier == null) return;
            var typeInfo = context.SemanticModel.GetTypeInfo(identifier, context.CancellationToken);

            if (typeInfo.ConvertedType?.BaseType == null) return;
            if (typeInfo.ConvertedType.BaseType.Name != typeof(MulticastDelegate).Name) return;

            var symbol = context.SemanticModel.GetSymbolInfo(identifier).Symbol;
            if (symbol is ILocalSymbol) return;

            var invokedMethodSymbol = (typeInfo.ConvertedType as INamedTypeSymbol)?.DelegateInvokeMethod;
            if (invokedMethodSymbol == null) return;
            if (!invokedMethodSymbol.ReturnsVoid && !invokedMethodSymbol.ReturnType.IsReferenceType) return;

            //if (IsEventHandlerLike(invocation, context.SemanticModel))
            //{
            //    ReportDiagnostic(context, invocation.GetLocation(), identifier.Identifier.Text);
            //}

            if (HasCheckForNull(invocation, context.SemanticModel, symbol))
            {
                ReportDiagnostic(context, invocation.GetLocation(), identifier.Identifier.Text);
            }
        }

        private static bool HasCheckForNull(InvocationExpressionSyntax invocation, SemanticModel semanticModel, ISymbol symbol)
        {
            if (invocation.FirstAncestorOfKind(SyntaxKind.MethodDeclaration) is MethodDeclarationSyntax method && method.Body != null)
            {
                var ifs = method.Body.Statements.OfKind(SyntaxKind.IfStatement);

                foreach (IfStatementSyntax @if in ifs)
                {
                    if (!@if.Condition?.IsKind(SyntaxKind.NotEqualsExpression) ?? true) continue;

                    var equals = (BinaryExpressionSyntax)@if.Condition;
                    if (equals.Left == null || equals.Right == null) continue;
                    if (@if.GetLocation().SourceSpan.Start > invocation.GetLocation().SourceSpan.Start) return false;
                    ISymbol identifierSymbol;
                    if (equals.Right.IsKind(SyntaxKind.NullLiteralExpression) && equals.Left.IsKind(SyntaxKind.IdentifierName))
                        identifierSymbol = semanticModel.GetSymbolInfo(equals.Left).Symbol;
                    else if (equals.Left.IsKind(SyntaxKind.NullLiteralExpression) && equals.Right.IsKind(SyntaxKind.IdentifierName))
                        identifierSymbol = semanticModel.GetSymbolInfo(equals.Right).Symbol;
                    else continue;
                    if (!symbol.Equals(identifierSymbol)) continue;
                    return true;
                    //if (@if.Statement == null) continue;
                    //if (@if.Statement.IsKind(SyntaxKind.Block))
                    //{
                    //    var ifBlock = (BlockSyntax)@if.Statement;
                    //    if (ifBlock.Statements.OfKind(SyntaxKind.ThrowStatement, SyntaxKind.ReturnStatement).Any()) return true;
                    //}
                    //else
                    //{
                    //    if (@if.Statement.IsAnyKind(SyntaxKind.ThrowStatement, SyntaxKind.ReturnStatement)) return true;
                    //}
                }
            }
            return false;
        }

        private static bool IsEventHandlerLike(InvocationExpressionSyntax invocation, SemanticModel semanticModel)
        {
            //If there is _ in method names 
            //if (invocation.Identifier.ValueText.Contains("_")) return true;

            if (invocation.ArgumentList.Arguments.None()) return false;

            var firstParameter = invocation.ArgumentList.Arguments[0];
            var senderType = semanticModel.GetSymbolInfo(firstParameter).Symbol as IParameterSymbol;
            if (senderType.Type.SpecialType != SpecialType.System_Object) return false;

            return invocation.ArgumentList.Arguments.TrueForAtLeastOnce(it =>
            {
                var eventArgsType = semanticModel.GetTypeInfo(it).Type as INamedTypeSymbol;
                if (eventArgsType == null) return false;
                return eventArgsType.AllBaseTypesAndSelf().Any(type => type.ToString() == "System.EventArgs");
            });
        }
    }
}
