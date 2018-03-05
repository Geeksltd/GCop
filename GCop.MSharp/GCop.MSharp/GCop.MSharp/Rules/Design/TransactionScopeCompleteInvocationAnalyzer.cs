namespace GCop.MSharp.Rules.Design
{
    using Core;
    using Core.Attributes;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System;
    using System.Linq;

    [MSharpExclusive]
    [ZebbleExclusive]
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class TransactionScopeCompleteInvocationAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        private readonly string MethodName = "CreateTransactionScope";
        protected override SyntaxKind Kind => SyntaxKind.InvocationExpression;

        private string ScopeVariable;
        private BlockSyntax OwnerBlock;
        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "123",
                Category = Category.Design,
                Message = "Transaction is created but the method .Complete() is never called.",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var invocation = (InvocationExpressionSyntax)context.Node;

            OwnerBlock = GetBlock(invocation);

            var memberAccess = invocation.Expression as MemberAccessExpressionSyntax;
            if (memberAccess == null) return;

            var symbol = context.SemanticModel.GetSymbolInfo(memberAccess).Symbol;
            if (symbol == null) return;

            var method = symbol as IMethodSymbol;

            if (method == null) return;

            if (method.Name != MethodName) return;
            //if( method.Name == MethodName ){}

            ScopeVariable = GetVariableOfInvocation(invocation);

            if (ScopeVariable.IsEmpty()) return;

            if (!IsCompletedCalled())
            {
                ReportDiagnostic(context, invocation);
            }
        }

        private bool IsCompletedCalled()
        {
            var completeInvocations = OwnerBlock.DescendantNodes().OfType<InvocationExpressionSyntax>().Where(it => (it.Expression as MemberAccessExpressionSyntax)?.Name.Identifier.ValueText == "Complete").ToList();

            return completeInvocations.TrueForAtLeastOnce(it =>
           {
               var variable = GetVariableOfInvocation(it);

               return ScopeVariable == variable;
           });
        }

        private BlockSyntax GetBlock(InvocationExpressionSyntax invocation)
        {
            try
            {
                var containerBlock = invocation.Parent;
                while (!(containerBlock is BlockSyntax))
                {
                    containerBlock = containerBlock.Parent;
                }

                return containerBlock as BlockSyntax;
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }

        private string GetVariableOfInvocation(InvocationExpressionSyntax invocation)
        {
            if (invocation.Parent is ExpressionStatementSyntax)
                return ((invocation.Expression as MemberAccessExpressionSyntax)?.ChildNodes().FirstOrDefault() as IdentifierNameSyntax)?.Identifier.ValueText;
            else
                return (invocation.Parent?.Parent as VariableDeclaratorSyntax)?.Identifier.ValueText;
        }
    }
}
