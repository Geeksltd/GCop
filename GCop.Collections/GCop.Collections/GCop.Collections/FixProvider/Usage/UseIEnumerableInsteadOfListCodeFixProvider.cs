namespace GCop.Collections.FixProvider.Usage
{
    using GCop.Collections.Core;
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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseIEnumerableInsteadOfListCodeFixProvider)), Shared]
    public class UseIEnumerableInsteadOfListCodeFixProvider : GCopCodeFixProvider
    {
        private string Title => "Use IEnumerable type instead";

        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop529");

        protected override void RegisterCodeFix()
        {
            var token = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().FirstOrDefault();
            if (token == null) return;
            Context.RegisterCodeFix(CodeAction.Create(Title, action => UseIEnumerable(Context.Document, token, action), Title), Diagnostic);
        }

        private async Task<Document> UseIEnumerable(Document document, MethodDeclarationSyntax method, CancellationToken cancellationToken)
        {
            MethodDeclarationSyntax newMethod = null;
            try
            {
                var parameters = new SeparatedSyntaxList<ParameterSyntax>();
                foreach (var parameter in method.ParameterList.Parameters)
                {
                    parameters = parameters.Add(SyntaxFactory.Parameter(parameter.AttributeLists,
                        parameter.Modifiers, SyntaxFactory.ParseTypeName(parameter.Type.ToString().Replace("List", "IEnumerable") + " "), parameter.Identifier, parameter.Default));
                }

                var parameterList = method.ParameterList.Update(method.ParameterList.OpenParenToken, parameters, method.ParameterList.CloseParenToken);

                newMethod = SyntaxFactory.MethodDeclaration(method.AttributeLists, method.Modifiers, method.ReturnType, method.ExplicitInterfaceSpecifier, method.Identifier, method.TypeParameterList, parameterList, method.ConstraintClauses, method.Body, method.ExpressionBody, method.SemicolonToken);
            }
            catch (Exception ex)
            {
                //No logging needed
            }

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(method, newMethod);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}