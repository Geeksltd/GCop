namespace GCop.Common.Rules.Design
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UseVirtualMethodToDefineSubTypeLogicAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.IfStatement;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "181",
                Category = Category.Design,
                Severity = DiagnosticSeverity.Warning,
                Message = "Define a virtual method and write this logic using polymorphism."
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var expression = NodeToAnalyze as IfStatementSyntax;

            if (expression == null)
                return;

            var isExpression = expression.ChildNodes().OfType<BinaryExpressionSyntax>().OfKind(SyntaxKind.IsExpression).FirstOrDefault();
            var thisExpression = isExpression?.ChildNodes().OfType<ThisExpressionSyntax>().FirstOrDefault();

            if (thisExpression == null)
                return;

            var classIdentifier = isExpression.ChildNodes().OfType<IdentifierNameSyntax>().LastOrDefault();

            if (!IsClassSubTypeOfThis(classIdentifier, thisExpression, context))
                return;

            ReportDiagnostic(context, expression);
        }

        private bool IsClassSubTypeOfThis(IdentifierNameSyntax classIdentifier, ThisExpressionSyntax thisExpression, SyntaxNodeAnalysisContext context)
        {
            var semanticModel = context.SemanticModel;
            var baseTypes = (semanticModel.GetSymbolInfo(classIdentifier).Symbol as INamedTypeSymbol).AllBaseTypes().ExceptLast();  // Ignores System.Object.
            var thisType = semanticModel.GetSymbolInfo(thisExpression).Symbol?.ContainingType;

            return baseTypes.Contains(thisType);
        }
    }
}
