namespace GCop.String.Rules.Design
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NameOfAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.ThrowStatement;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "149",
                Category = Category.Design,
                Severity = DiagnosticSeverity.Warning,
                Message = "Instead of using string literal as parameter name use nameof(variableName)"
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            var @throw = context.Node as ThrowStatementSyntax;
            if (@throw == null) return;

            NodeToAnalyze = @throw;

            var objectCreation = @throw.Expression as ObjectCreationExpressionSyntax;
            if (objectCreation == null) return;

            var identifierName = (objectCreation.Type as IdentifierNameSyntax)?.Identifier.ValueText;

            if (identifierName?.IsNoneOf("ArgumentException", "ArgumentNullException") ?? true) return;

            var arguments = objectCreation.ArgumentList.Arguments.AsEnumerable();

            if (arguments.HasMany() || arguments.None()) return;

            if (arguments.First().Expression.IsKind(SyntaxKind.StringLiteralExpression) == false) return;

            var parameterValue = (arguments.First().Expression as LiteralExpressionSyntax)?.Token.ValueText;

            if (parameterValue?.Lacks(" ") == true)
            {
                var methodDeclaration = NodeToAnalyze.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();
                if (methodDeclaration == null)
                {
                    ReportDiagnostic(context, arguments.First().GetLocation());
                }
                else
                {
                    if (methodDeclaration.ParameterList.Parameters.Any(it => it.Identifier.ValueText == parameterValue))
                    {
                        ReportDiagnostic(context, arguments.First().GetLocation());
                    }
                }
            }
        }
    }
}
