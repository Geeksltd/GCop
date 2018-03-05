namespace GCop.Common.Rules.Style
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Collections.Generic;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class ObjectInitializerAnalyzer : GCopAnalyzer
	{
		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "401",
				Category = Category.Style,
				Severity = DiagnosticSeverity.Warning,
				Message = "Instead of setting the properties in separate lines, use constructor initializers."
			};
		}

		protected override void Configure()
		{
			RegisterSyntaxNodeAction(context => AnalyzeAssignment(context), SyntaxKind.ExpressionStatement);
			RegisterSyntaxNodeAction(context => AnalyzeLocalDeclaration(context), SyntaxKind.LocalDeclarationStatement);
		}

		private void AnalyzeAssignment(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var semanticModel = context.SemanticModel;

			var expressionStatement = context.Node as ExpressionStatementSyntax;
			if (expressionStatement?.Expression?.IsNotKind(SyntaxKind.SimpleAssignmentExpression) ?? true) return;

			var assignmentExpression = (AssignmentExpressionSyntax)expressionStatement.Expression;
			if (assignmentExpression.Right.IsNotKind(SyntaxKind.ObjectCreationExpression)) return;

			if (((ObjectCreationExpressionSyntax)assignmentExpression.Right).Initializer?.IsKind(SyntaxKind.CollectionInitializerExpression) == true) return;

			var variableSymbol = semanticModel.GetSymbolInfo(assignmentExpression.Left).Symbol;
			var assignmentExpressionStatements = FindAssignmentExpressions(semanticModel, expressionStatement, variableSymbol);

			if (assignmentExpressionStatements.Any() == false) return;
			if (HasAssignmentUsingDeclaredVariable(semanticModel, variableSymbol, assignmentExpressionStatements)) return;

			assignmentExpressionStatements.ForEach(element => context.ReportDiagnostic(Diagnostic.Create(Description, element.GetLocation())));
		}

		private void AnalyzeLocalDeclaration(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var semanticModel = context.SemanticModel;
			var localDeclarationStatement = context.Node as LocalDeclarationStatementSyntax;
			if (localDeclarationStatement == null) return;

			if (localDeclarationStatement.Declaration?.Variables.Count != 1) return;

			var variable = localDeclarationStatement.Declaration.Variables.Single();
			var equalsValueClauseSyntax = variable.Initializer as EqualsValueClauseSyntax;
			if (equalsValueClauseSyntax?.Value.IsNotKind(SyntaxKind.ObjectCreationExpression) ?? true) return;
			if (((ObjectCreationExpressionSyntax)equalsValueClauseSyntax.Value).Initializer?.IsKind(SyntaxKind.CollectionInitializerExpression) == true) return;

			var variableSymbol = semanticModel.GetDeclaredSymbol(variable);
			var assignmentExpressionStatements = FindAssignmentExpressions(semanticModel, localDeclarationStatement, variableSymbol);

			if (assignmentExpressionStatements.Any() == false) return;
			if (HasAssignmentUsingDeclaredVariable(semanticModel, variableSymbol, assignmentExpressionStatements)) return;

			assignmentExpressionStatements.ForEach(element => context.ReportDiagnostic(Diagnostic.Create(Description, element.GetLocation())));
		}

		public static bool HasAssignmentUsingDeclaredVariable(SemanticModel semanticModel, ISymbol variableSymbol, IEnumerable<ExpressionStatementSyntax> assignmentExpressionStatements)
		{
			foreach (var assignmentExpressionStatement in assignmentExpressionStatements)
			{
				var assignmentExpression = (AssignmentExpressionSyntax)assignmentExpressionStatement.Expression;
				var ids = assignmentExpression.Right.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>().ToList();
				//if the type of symbol is delegate should ignore the rule
				var symbolType = semanticModel.GetTypeInfo(assignmentExpression.Left).Type;
				if (symbolType?.BaseType?.ToString() == "System.MulticastDelegate") return true;
				if (ids.Any(id => semanticModel.GetSymbolInfo(id).Symbol?.Equals(variableSymbol) == true)) return true;
			}

			return false;
		}

		public static List<ExpressionStatementSyntax> FindAssignmentExpressions(SemanticModel semanticModel, StatementSyntax statement, ISymbol variableSymbol)
		{
			var blockParent = statement.FirstAncestorOrSelf<BlockSyntax>();
			var isBefore = true;
			var assignmentExpressions = new List<ExpressionStatementSyntax>();
			foreach (var blockStatement in blockParent.Statements)
			{
				if (isBefore)
				{
					if (blockStatement.Equals(statement)) isBefore = false;
				}
				else
				{
					var expressionStatement = blockStatement as ExpressionStatementSyntax;
					if (expressionStatement == null) break;
					var assignmentExpression = expressionStatement.Expression as AssignmentExpressionSyntax;
					if (assignmentExpression == null || !assignmentExpression.IsKind(SyntaxKind.SimpleAssignmentExpression)) break;
					var memberAccess = assignmentExpression.Left as MemberAccessExpressionSyntax;
					if (memberAccess == null || !memberAccess.IsKind(SyntaxKind.SimpleMemberAccessExpression)) break;
					var memberIdentifier = memberAccess.Expression as IdentifierNameSyntax;
					if (memberIdentifier == null) break;
					var propertyIdentifier = memberAccess.Name as IdentifierNameSyntax;
					if (propertyIdentifier == null) break;
					if (!semanticModel.GetSymbolInfo(memberIdentifier).Symbol.Equals(variableSymbol)) break;
					assignmentExpressions.Add(expressionStatement);
				}
			}
			return assignmentExpressions;
		}
	}
}
