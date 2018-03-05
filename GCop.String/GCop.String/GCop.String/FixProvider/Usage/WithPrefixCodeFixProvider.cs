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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(WithPrefixCodeFixProvider)), Shared]
    public class WithPrefixCodeFixProvider : GCopCodeFixProvider
    {
        private string Title => "Use .WithPrefix method";

        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop531");

        protected override void RegisterCodeFix()
        {
            var token = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ConditionalExpressionSyntax>().FirstOrDefault();
            if (token == null) return;
            Context.RegisterCodeFix(CodeAction.Create(Title, action => UseWithPrefix(Context.Document, token, action), Title), Diagnostic);
        }

        private async Task<Document> UseWithPrefix(Document document, ConditionalExpressionSyntax conditional, CancellationToken cancellationToken)
        {
            InvocationExpressionSyntax newExpression = null;
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
                var something = addExpression.ChildNodes().FirstOrDefault()?.ToString();
                newExpression = SyntaxFactory.InvocationExpression(SyntaxFactory.ParseExpression(variableName + ".WithPrefix"), SyntaxFactory.ParseArgumentList("(" + something + ")"));
            }
            catch (Exception ex)
            {
                //No logging needed
            }

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(conditional, newExpression);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}