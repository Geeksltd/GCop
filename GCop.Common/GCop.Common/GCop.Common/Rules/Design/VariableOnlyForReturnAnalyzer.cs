namespace GCop.Common.Rules.Design
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class VariableOnlyForReturnAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
	{
		protected override SyntaxKind Kind => SyntaxKind.ReturnStatement;

		protected override void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;

			var returnIdentifierSyntax = (NodeToAnalyze as ReturnStatementSyntax).GetIdentifierSyntax();
			if (returnIdentifierSyntax == null) return;

			var method = returnIdentifierSyntax.GetSingleAncestor<MethodDeclarationSyntax>();
			if (method == null) return;

			//Looking at how many codes are using that variable 
			var usedItems = method.Body.DescendantNodesAndTokens().Where(x => x.IsKind(SyntaxKind.IdentifierToken)).Where(x => x.ToString() == returnIdentifierSyntax.Identifier.ValueText);
			if (usedItems.Count() != 2) return;

			//If a method has more than one return statement should be skipped
			if (method.Body.DescendantNodesAndTokens().Count(x => x.IsKind(SyntaxKind.ReturnKeyword)) != 1) return;

			// looking for variable declaration before return statement
			var definitionOfVariable = method.Body.DescendantNodes().Where(x => x.IsKind(SyntaxKind.VariableDeclaration));
			if (definitionOfVariable.None()) return;

			bool found = false;
			foreach (var item in definitionOfVariable)
			{
				var defeinitionOfVaribale = item as VariableDeclarationSyntax;
				var varDeclarator = defeinitionOfVaribale.Variables.FirstOrDefault() as VariableDeclaratorSyntax;
				if (varDeclarator == null) continue;
				if (varDeclarator.Identifier == null) continue;
				if (varDeclarator.Identifier.ValueText == returnIdentifierSyntax.Identifier.ValueText)
				{
					found = true;
					if (defeinitionOfVaribale.Parent != null)
						if (method.Body.ChildIndex(NodeToAnalyze) != method.Body.ChildIndex(defeinitionOfVaribale.Parent) + 1) return;
				}
			}

			// var [result] = 10; return [result]; 
			if (found == false) return;

			usedItems = usedItems.Where(it => it.Parent?.Parent?.Kind() != SyntaxKind.ReturnStatement);

			//Skip variable which hase result of Database 
			foreach (var item in usedItems)
			{
				StatementSyntax expression;

				if (item.IsNode)
				{
					expression = item.AsNode().GetSingleAncestor<StatementSyntax>();
				}
				else
					expression = item.AsToken().Parent?.AncestorsAndSelf().OfType<StatementSyntax>().FirstOrDefault();

				if (expression == null) continue;

				// checking [expression Database.Sth] in ( var item=expression; ) in here                    
				var variableDeclarationSyntax = expression.ChildNodes()?.OfType<VariableDeclarationSyntax>().FirstOrDefault();
				if (variableDeclarationSyntax == null) continue;

				var variableDeclaratorSyntax = variableDeclarationSyntax.ChildNodes().OfType<VariableDeclaratorSyntax>().FirstOrDefault();
				if (variableDeclaratorSyntax == null) continue;

				var identifierToken = variableDeclaratorSyntax.Identifier; //  <- here we find [item] in ( var item=expression; )

				if (identifierToken == null) continue;


				if (identifierToken.ValueText != returnIdentifierSyntax.Identifier.ValueText) return;


				var equalsValueClauseSyntax = variableDeclaratorSyntax.ChildNodes().OfType<EqualsValueClauseSyntax>().FirstOrDefault();
				if (equalsValueClauseSyntax == null) continue;

				// checking [expression] in ( var item=expression; ) in here 
				var invocationExpresionSyntax = equalsValueClauseSyntax.ChildNodes()?.OfType<InvocationExpressionSyntax>().FirstOrDefault();
				if (invocationExpresionSyntax != null)
				{
					var simpleMemberAccess = invocationExpresionSyntax?.Expression as MemberAccessExpressionSyntax;
					if (simpleMemberAccess == null) continue;

					var identiferDatabase = simpleMemberAccess.ChildNodes().OfType<IdentifierNameSyntax>().FirstOrDefault();
					if (identiferDatabase == null) continue;

					if (identiferDatabase.Identifier.Text.ToLower() == "Database".ToLower())
					{
						var databaseInfo = context.SemanticModel.GetTypeInfo(identiferDatabase).Type;
						if (databaseInfo?.ContainingNamespace.ToString() == "MSharp.Framework") continue;
						else
						{
							ReportDiagnostic(context, identifierToken);
							return;
						}
					}
					else
					{
						ReportDiagnostic(context, identifierToken);
						return;
					}
				}

				if (identifierToken.ValueText == item.ToString())
				{
					// var result = 10; return result;
					ReportDiagnostic(context, identifierToken);
					return;
				}
			}
		}

		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "177",
				Category = Category.Design,
				Message = "Variable declaration is unnecessary due to it being used only for return statement",
                Severity = DiagnosticSeverity.Warning
			};
		}
	}
}