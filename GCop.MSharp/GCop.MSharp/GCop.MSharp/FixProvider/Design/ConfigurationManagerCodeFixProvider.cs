namespace GCop.MSharp.FixProvider.Design
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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ConfigurationManagerCodeFixProvider)), Shared]
    public class ConfigurationManagerCodeFixProvider : GCopCodeFixProvider
    {
        private string Title => "Change it to use Config's method";
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop164");

        protected override void RegisterCodeFix()
        {
            try
            {
                var token = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MemberAccessExpressionSyntax>().FirstOrDefault();
                if (token == null) return;

                Context.RegisterCodeFix(CodeAction.Create(Title, action => UseConfig(Context.Document, token, action), Title), Diagnostic);
            }
            catch (NullReferenceException)
            {
                //No matter to handle NullReferenceException
            }
        }

        private async Task<Document> UseConfig(Document document, MemberAccessExpressionSyntax memberAccess, CancellationToken cancellationToken)
        {
            InvocationExpressionSyntax newMemberAccess = null;
            ElementAccessExpressionSyntax elementAccess = null;
            try
            {
                elementAccess = memberAccess.Parent as ElementAccessExpressionSyntax;
                var arguments = elementAccess.ArgumentList.ToString();
                var identifier = memberAccess.ChildNodes().OfType<IdentifierNameSyntax>().ToList()[1].ToString();

                newMemberAccess = SyntaxFactory.InvocationExpression(
                    SyntaxFactory.ParseExpression("Config." + (identifier == "ConnectionStrings" ? SyntaxFactory.IdentifierName("GetConnectionString") : SyntaxFactory.IdentifierName("Get"))),
                    SyntaxFactory.ParseArgumentList(arguments.Replace('[', '(').Replace(']', ')'))
                    );
            }
            catch
            {
                //No loggin needed
            }

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(elementAccess, newMemberAccess);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}