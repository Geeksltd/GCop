namespace GCop.Conditional.Rules.Design
{
	using Core;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using Microsoft.CodeAnalysis.Diagnostics;
	using System.Linq;

	[DiagnosticAnalyzer( LanguageNames.CSharp )]
    public class AvoidAssignmentWithinConditionAnalyzer:GCopAnalyzer
    {
        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "166",
                Category = Category.Design,
                Message = "Avoid assignment within conditional statements",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Configure()
        {
            RegisterSyntaxNodeAction( context => Analyze( context ), SyntaxKind.EqualsExpression
                                                                   , SyntaxKind.GreaterThanExpression
                                                                   , SyntaxKind.LessThanExpression
                                                                   , SyntaxKind.GreaterThanOrEqualExpression
                                                                   , SyntaxKind.LessThanOrEqualExpression );
        }

        protected void Analyze(SyntaxNodeAnalysisContext context)
        {
            var expression = context.Node as BinaryExpressionSyntax;

            NodeToAnalyze = expression;

            if ( expression.Left.ChildNodes().OfType<AssignmentExpressionSyntax>().Any() )
                ReportDiagnostic( context, expression.Left );

            if ( expression.Right.ChildNodes().OfType<AssignmentExpressionSyntax>().Any() )
                ReportDiagnostic( context, expression.Right );
        }
    }
}
