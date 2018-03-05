namespace GCop.Common.Rules.Design
{
	using Core;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using Microsoft.CodeAnalysis.Diagnostics;
	using System;
	using System.Linq;

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AvoidInstantiateBeforeAssignmentAnalyzer:GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.LocalDeclarationStatement;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "168",
                Category = Category.Design,
                Message = "Don't instantiate a variable with the new keyword if you are going to assign it to a different object immediately.",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            var declaration = context.Node as LocalDeclarationStatementSyntax;
            if (declaration == null) return;

            NodeToAnalyze = declaration;
            var equalsClause = declaration.DescendantNodes().OfType<EqualsValueClauseSyntax>().FirstOrDefault();
            if (equalsClause == null) return;

            var objectCreation = equalsClause.Value as ObjectCreationExpressionSyntax;
            if (objectCreation == null) return;

            var variableName = equalsClause.EqualsToken.GetPreviousToken().ToString();
            if (variableName.IsEmpty()) return;

            if (declaration.Parent == null) return;

            var allChilds = declaration.Parent.ChildNodes().ToList();

            var declarationIndex = allChilds.IndexOf(declaration);
            if (declarationIndex < 0) return;

            if (allChilds.Count() <= declarationIndex + 1) return;

            var nextExpression = allChilds[declarationIndex + 1];

            if (nextExpression.IsNotKind(SyntaxKind.ExpressionStatement)) return;
            var assignment = nextExpression.ChildNodes().OfType<AssignmentExpressionSyntax>().FirstOrDefault();
            if (assignment == null) return;
            if (assignment.Left.IsKind(SyntaxKind.IdentifierName) && assignment.Left.GetIdentifier() == variableName)
            {
                ReportDiagnostic(context, declaration);
            }

            if (assignment.Left.IsKind(SyntaxKind.ElementAccessExpression)) return;

            else if (assignment.Left.ChildNodes().OfType<IdentifierNameSyntax>().IsSingle() && assignment.Left.GetIdentifier() == variableName)
            {
                ReportDiagnostic(context, declaration);
            }
        }
    }
}
