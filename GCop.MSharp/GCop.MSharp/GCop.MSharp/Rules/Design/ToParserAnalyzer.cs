namespace GCop.MSharp.Rules.Design
{
    using Core;
    using Core.Attributes;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [MSharpExclusive]
    [ZebbleExclusive]
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ToParserAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.InvocationExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "121",
                Category = Category.Design,
                Message = "Use {0}.To<{1}>() instead of {1}.Parse({0})",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var invocationExpressionSyntax = context.Node as InvocationExpressionSyntax;
            var memberAccessExpressionSyntaxList = invocationExpressionSyntax?.DescendantNodes().OfType<MemberAccessExpressionSyntax>();
            if (memberAccessExpressionSyntaxList == null || memberAccessExpressionSyntaxList.HasMany()) return;

            var memberAccessExpressionSyntax = memberAccessExpressionSyntaxList.FirstOrDefault();
            if (memberAccessExpressionSyntax == null) return;

            var nodes = memberAccessExpressionSyntax.DescendantNodes().ToList();

            var parseToken = memberAccessExpressionSyntax.DescendantTokens()
                                                         .Where(t => t.IsKind(SyntaxKind.IdentifierToken))
                                                         .FirstOrDefault(t => t.ValueText == "Parse");

            if (parseToken == null || parseToken.Parent == null) return;

            // We only want to do this for System Types (other libs and frameworks may have Parse methods non related to M# Framework To<T>()
            var typeInfo = context.SemanticModel.GetTypeInfo(memberAccessExpressionSyntax.Expression);
            if (typeInfo.Type.ContainingNamespace.Name != "System" || typeInfo.Type.ToString() == "System.Version") return;

            var argumentListSyntax = invocationExpressionSyntax.ChildNodes().OfType<ArgumentListSyntax>().SingleOrDefault();
            if (argumentListSyntax == null) return;

            SyntaxNode identifierNameSyntax = null;

            if (parseToken.Parent is IdentifierNameSyntax)
                identifierNameSyntax = (IdentifierNameSyntax)parseToken.Parent;
            else if (parseToken.Parent is GenericNameSyntax)
                identifierNameSyntax = (GenericNameSyntax)parseToken.Parent;

            var methodName = identifierNameSyntax.ToString();
            var predefinedTypeSyntax = nodes.OfType<PredefinedTypeSyntax>().SingleOrDefault();
            var type = predefinedTypeSyntax?.Keyword.ValueText ?? nodes.First().ToString();
            if (argumentListSyntax.Arguments.Count > 2) return; // Parse overload with more complicated Parsing
            if (argumentListSyntax.Arguments.None()) return; // Parse overload with more complicated Parsing
            var parseArgument = argumentListSyntax.Arguments[0];

            context.ReportDiagnostic(Diagnostic.Create(Description, invocationExpressionSyntax.GetLocation(), parseArgument, type));
        }
    }
}