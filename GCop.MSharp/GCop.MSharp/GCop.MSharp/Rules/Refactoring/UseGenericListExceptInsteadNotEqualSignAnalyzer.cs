namespace GCop.MSharp.Rules.Refactoring
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
    public class UseGenericListExceptInsteadNotEqualSignAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.InvocationExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "625",
                Category = Category.Refactoring,
                Severity = DiagnosticSeverity.Warning,
                Message = "Using != operator on the entity is not converted into SQL. Use x.ID != [...].ID instead."
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var invocExpres = NodeToAnalyze as InvocationExpressionSyntax;

            if (invocExpres == null)
                return;

            if (invocExpres.ArgumentList.IsNone()) return;
            if (invocExpres.ArgumentList.Arguments == null) return;

            var memberAccessExpression = invocExpres.ChildNodes().OfType<MemberAccessExpressionSyntax>().FirstOrDefault();
            if (memberAccessExpression == null) return;

            //checking Databae + GetList
            var variableIdentifier = memberAccessExpression.ChildNodes()?.FirstOrDefault()?.GetIdentifierSyntax();
            if (variableIdentifier == null) return;
            if (variableIdentifier.Identifier == null) return;
            if (variableIdentifier.Identifier.ValueText != "Database") return;

            var variableInfo = context.SemanticModel.GetSymbolInfo(variableIdentifier).Symbol;
            if (variableInfo == null) return;
            if (variableInfo.ContainingNamespace == null) return;
            if (variableInfo.ContainingNamespace.ToString().Lacks("MSharp")) return;

            //GetList<T> checking
            var getList = memberAccessExpression.ChildNodes().LastOrDefault();
            if (getList == null) return;
            if (getList.Kind() != SyntaxKind.GenericName) return;
            if (getList.ToString().Lacks("GetList")) return;

            // Parsing Lambda experssion 
            var arguments = invocExpres.ArgumentList;
            var firstArg = arguments.ChildNodes().FirstOrDefault() as ArgumentSyntax;
            if (firstArg == null) return;
            var lambdaNodeExpres = firstArg.Expression as SimpleLambdaExpressionSyntax;
            if (lambdaNodeExpres == null) return;

            //var lambdaString = lambdaNode.ToString();            
            if (lambdaNodeExpres.ChildNodes().OfKind(SyntaxKind.NotEqualsExpression).None()) return;

            // Check the entity only
            if (lambdaNodeExpres.DescendantNodes().OfKind(SyntaxKind.SimpleMemberAccessExpression).Any()) return;
            ReportDiagnostic(context, lambdaNodeExpres);
        }
    }
}