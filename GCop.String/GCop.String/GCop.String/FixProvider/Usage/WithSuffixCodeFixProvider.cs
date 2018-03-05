namespace GCop.String.FixProvider.Usage
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using System;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(WithSuffixCodeFixProvider)), Shared]
    public class WithSuffixCodeFixProvider : GCopCodeFixProvider
    {
        private string Title => "Use .WithSuffix method";

        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop530");

        protected override void RegisterCodeFix()
        {
            var token = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ConditionalExpressionSyntax>().FirstOrDefault();
            if (token == null) return;
            Context.RegisterCodeFix(CodeAction.Create(Title, action => UseWithSuffix(Context.Document, token, action), Title), Diagnostic);
        }

        private async Task<Document> UseWithSuffix(Document document, ConditionalExpressionSyntax conditional, CancellationToken cancellationToken)
        {
            InvocationExpressionSyntax newInvocation = null;
            try
            {
                var invocation = conditional.Condition as InvocationExpressionSyntax;
                var identifierName = invocation.Expression?.As<MemberAccessExpressionSyntax>().ChildNodes().OfType<IdentifierNameSyntax>().LastOrDefault();
                var variableName = invocation.Expression.As<MemberAccessExpressionSyntax>().GetIdentifier();

                ExpressionSyntax addExpression = null;
                if (identifierName.Identifier.ValueText == "IsEmpty")
                {
                    addExpression = conditional.WhenFalse;
                }
                else if (identifierName.Identifier.ValueText == "HasValue")
                {
                    addExpression = conditional.WhenTrue;
                }
                var something = addExpression.ChildNodes().LastOrDefault()?.ToString();
                newInvocation = SyntaxFactory.InvocationExpression(SyntaxFactory.ParseExpression(variableName + ".WithSuffix"), SyntaxFactory.ParseArgumentList("(" + something + ")"));
            }
            catch (Exception ex)
            {
                //No logging needed
            }

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(conditional, newInvocation);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}