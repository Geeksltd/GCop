namespace GCop.Common.Rules.Style
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class SwitchToAutoPropAnalyzer : GCopAnalyzer
	{
		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "403",
				Category = Category.Style,
				Severity = DiagnosticSeverity.Warning,
				Message = "Change {0} to an auto property"
			};
		}

		protected override void Configure()
		{
			RegisterSyntaxNodeActionForVersionLower(context => AnalyzeProperty(context, true), LanguageVersion.CSharp5, SyntaxKind.PropertyDeclaration);
			RegisterSyntaxNodeActionForVersionLower(context => AnalyzeProperty(context, false), LanguageVersion.CSharp5, SyntaxKind.PropertyDeclaration);
		}

		private void AnalyzeProperty(SyntaxNodeAnalysisContext context, bool canHaveFieldInitializer)
		{
			NodeToAnalyze = context.Node;
			var property = (PropertyDeclarationSyntax)context.Node;
			if (property.AccessorList?.Accessors.Count != 2) return;
			if (property.AccessorList.Accessors.Any(a => a.Body == null)) return;
			if (property.AccessorList.Accessors.Any(a => a.Body.Statements.Count != 1)) return;
			var getter = property.AccessorList.Accessors.First(a => a.Keyword.ValueText == "get");
			var getterReturn = getter.Body.Statements.First() as ReturnStatementSyntax;
			if (getterReturn == null) return;
			var setter = property.AccessorList.Accessors.First(a => a.Keyword.ValueText == "set");
			var setterExpressionStatement = setter.Body.Statements.First() as ExpressionStatementSyntax;
			var setterAssignmentExpression = setterExpressionStatement?.Expression as AssignmentExpressionSyntax;
			if (setterAssignmentExpression == null) return;

			SimpleNameSyntax returnIdentifier = null;

			if (getterReturn.Expression is MemberAccessExpressionSyntax && ((MemberAccessExpressionSyntax)getterReturn.Expression).Expression is ThisExpressionSyntax)
			{
				returnIdentifier = ((MemberAccessExpressionSyntax)getterReturn.Expression).Name;
			}
			else
				returnIdentifier = getterReturn.Expression as IdentifierNameSyntax;


			if (returnIdentifier == null) return;
			var semanticModel = context.SemanticModel;
			var returnIdentifierSymbol = semanticModel.GetSymbolInfo(returnIdentifier).Symbol;
			if (returnIdentifierSymbol == null) return;
			Simplify(context, setterAssignmentExpression, semanticModel, returnIdentifierSymbol, property, canHaveFieldInitializer);
		}

		private void Simplify(SyntaxNodeAnalysisContext context, AssignmentExpressionSyntax setterAssignmentExpression, SemanticModel semanticModel, ISymbol returnIdentifierSymbol, PropertyDeclarationSyntax property, bool canHaveFieldInitializer)
		{
			SimpleNameSyntax assignmentLeftIdentifier = null;
			if (setterAssignmentExpression.Left is MemberAccessExpressionSyntax && ((MemberAccessExpressionSyntax)setterAssignmentExpression.Left).Expression is ThisExpressionSyntax)
			{
				assignmentLeftIdentifier = ((MemberAccessExpressionSyntax)setterAssignmentExpression.Left).Name;
			}
			else
				assignmentLeftIdentifier = setterAssignmentExpression.Left as IdentifierNameSyntax;


			if (assignmentLeftIdentifier == null) return;
			var assignmentLeftIdentifierSymbol = semanticModel.GetSymbolInfo(assignmentLeftIdentifier).Symbol;
			if (!assignmentLeftIdentifierSymbol.Equals(returnIdentifierSymbol)) return;
			var assignmentRightIdentifier = setterAssignmentExpression.Right as IdentifierNameSyntax;
			if (assignmentRightIdentifier == null) return;
			if (assignmentRightIdentifier.Identifier.Text != "value") return;
			if (assignmentLeftIdentifierSymbol.Kind != SymbolKind.Field) return;
			var backingFieldClassSymbol = assignmentLeftIdentifierSymbol.ContainingType;
			var propertySymbol = semanticModel.GetDeclaredSymbol(property);
			var propertyClassSymbol = propertySymbol.ContainingType;
			if (!propertyClassSymbol.Equals(backingFieldClassSymbol)) return;

			if (!canHaveFieldInitializer)
			{
				var variableDeclarator = assignmentLeftIdentifierSymbol.DeclaringSyntaxReferences.First().GetSyntax() as VariableDeclaratorSyntax;
				if (variableDeclarator.Initializer != null) return;
			}

			ReportDiagnostic(context, property.GetLocation(), property.Identifier.Text);
		}
	}
}
