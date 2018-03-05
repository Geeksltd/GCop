namespace GCop.String.FixProvider.Design
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Rules.Design;
    using System;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NameOfAnalyzer)), Shared]
    public class NameOfCodeFixProvider : GCopCodeFixProvider
    {
        private string Title;
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop149");

        protected override void RegisterCodeFix()
        {
            try
            {
                var token = Root.FindToken(DiagnosticSpan.Start);
                if (!token.IsKind(SyntaxKind.StringLiteralToken)) return;
                Title = $"Replace with nameof({token.ValueText})";
                Context.RegisterCodeFix(CodeAction.Create(Title, action => ReplaceStringLiteralWithNameOf(Context.Document, token, action), Title), Diagnostic);
            }
            catch (NullReferenceException)
            {
                //No matter to handle NullReferenceException
            }
        }

        private async Task<Document> ReplaceStringLiteralWithNameOf(Document document, SyntaxToken token, CancellationToken cancellationToken)
        {
            var stringLiteralExpression = token.Parent as LiteralExpressionSyntax;


            var argument = stringLiteralExpression.Ancestors().OfType<ArgumentSyntax>().FirstOrDefault();
            var argumentList = argument.Parent as ArgumentListSyntax;
            var newArgument = BuildNewArgumentSyntax(token.ValueText);

            var newArgumentList = argumentList.ReplaceNode(argument, newArgument);

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(argumentList, newArgumentList);
            return document.WithText(newRoot.GetText());
        }

        private ArgumentSyntax BuildNewArgumentSyntax(string variable)
        {
            var expression = SyntaxFactory.IdentifierName("nameof");
            var argumentList = SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(new[] { SyntaxFactory.Argument(SyntaxFactory.IdentifierName(variable)) }));
            var invocation = SyntaxFactory.InvocationExpression(expression, argumentList);
            return SyntaxFactory.Argument(invocation);
        }
    }
}
