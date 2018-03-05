namespace GCop.Common.Rules.Naming
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Collections.Generic;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class ResultAsVariableNameAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
	{
		protected override SyntaxKind Kind => SyntaxKind.MethodDeclaration;

		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "214",
				Category = Category.Naming,
				Severity = DiagnosticSeverity.Info,
				Message = "The variable defined to return the result of the method should be named 'result'"
			};
		}

		protected override void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var methodDeclaration = NodeToAnalyze as MethodDeclarationSyntax;
			if (methodDeclaration.Body == null) return;

			var returnStatements = methodDeclaration.Body.DescendantNodes().OfType<ReturnStatementSyntax>();
			var listOfVariables = new List<VariableDefinition>();

			bool shouldBeIgnored = false;

			if (methodDeclaration.Body.GetCountOfStatements() > 4) return;

			returnStatements.ForEach(@return =>
			{
				if (shouldBeIgnored) return;

				var returnNodes = @return.ChildNodes();
				if (returnNodes.IsSingle())
				{
					var returnIdentifier = returnNodes.OfType<IdentifierNameSyntax>().FirstOrDefault();
					if (returnIdentifier != null)
					{
						var variableInfo = context.SemanticModel.GetSymbolInfo(returnIdentifier).Symbol as ILocalSymbol;
						if (variableInfo == null)
						{
							shouldBeIgnored = true;
							return;
						}

						if (IsLoopVariable(@return, variableInfo.Name))
						{
							shouldBeIgnored = true;
							return;
						}

						listOfVariables.Add(new VariableDefinition
						{
							Location = returnIdentifier?.GetLocation(),
							Name = returnIdentifier?.Identifier.ValueText
						});
					}
					else
					{
						shouldBeIgnored = true;
						return;
					}
				}
			});

			if (shouldBeIgnored) return;

			var variables = listOfVariables.GroupBy(it => it.Name);

			if (variables.IsSingle() && variables.First().Key != "result")
			{
				if (methodDeclaration.ParameterList.Parameters.Any(it => it.Identifier.ValueText == variables.First().Key)) return;

				foreach (var item in variables.First())
				{
					ReportDiagnostic(context, item.Location);
				}
			}
		}

		private bool IsLoopVariable(ReturnStatementSyntax @return, string variableName)
		{
			var isloopVariable = @return.Ancestors().OfType<ForEachStatementSyntax>().TrueForAtLeastOnce(it =>
			{
				return it.Identifier.ValueText == variableName;
			});

			if (!isloopVariable)
			{
				isloopVariable = @return.Ancestors().OfType<ForStatementSyntax>().TrueForAtLeastOnce(it =>
				{
					return it.Declaration.Variables.Any(@var => @var.Identifier.ValueText == variableName);
				});
			}

			if (!isloopVariable)
			{
				isloopVariable = @return.Ancestors().OfType<WhileStatementSyntax>().TrueForAtLeastOnce(it =>
				 {
					 return it.ChildNodes().FirstOrDefault()?.DescendantNodes().OfType<IdentifierNameSyntax>().Any(@var => @var.Identifier.ValueText == variableName) ?? false;
				 });
			}

			return isloopVariable;
		}
	}
}
