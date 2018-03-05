namespace GCop.Conditional.Rules.Style
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Collections.Generic;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class ConditionalStructureDuplicateImplementationAnalyzer : GCopAnalyzer
	{
		readonly int NumberOfAllowedCharacter = 40;
		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "415",
				Category = Category.Style,
				Severity = DiagnosticSeverity.Warning,
				Message = "The same code is repeated in multiple IF branches. Instead update the IF condition to cover both scenarios."
			};
		}

		protected override void Configure()
		{
			RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
			RegisterSyntaxNodeAction(AnalyzeSwitchStatement, SyntaxKind.SwitchSection);
		}

		private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var ifStatement = (IfStatementSyntax)context.Node;

			if (ifStatement.Statement?.ToString().Length <= NumberOfAllowedCharacter) return;

			var precedingStatements = GetPrecedingStatementsInConditionChain(ifStatement).ToList();

			CheckStatement(context, ifStatement.Statement, precedingStatements);

			if (ifStatement.Else == null) return;

			precedingStatements.Add(ifStatement.Statement);
			CheckStatement(context, ifStatement.Else.Statement, precedingStatements);
		}

		private void AnalyzeSwitchStatement(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var switchSection = (SwitchSectionSyntax)context.Node;
			if (switchSection.Statements.ToString().Length <= NumberOfAllowedCharacter) return;

			var precedingSection = GetPrecedingSections(switchSection).FirstOrDefault(preceding => AreEquivalent(switchSection.Statements, preceding.Statements));

			if (precedingSection != null)
			{
				ReportSection(context, switchSection, precedingSection);
			}
		}

		private void CheckStatement(SyntaxNodeAnalysisContext context, StatementSyntax statementToCheck, IEnumerable<StatementSyntax> precedingStatements)
		{
			var precedingStatement = precedingStatements.FirstOrDefault(preceding => statementToCheck.IsEquivalent(preceding));

			if (precedingStatement == null) return;

			var elseClause = statementToCheck.GetSingleAncestor<ElseClauseSyntax>();
			if (elseClause != null && elseClause.Parent.IsEquivalent(precedingStatement.Parent)) return;
			ReportStatement(context, statementToCheck, precedingStatement);
		}

		private void ReportSection(SyntaxNodeAnalysisContext context, SwitchSectionSyntax switchSection, SwitchSectionSyntax precedingSection)
		{
			switchSection.Statements.Where(it => it.Kind() != SyntaxKind.BreakStatement).ForEach(it => ReportSyntaxNode(context, it, precedingSection, "case"));
		}

		private void ReportStatement(SyntaxNodeAnalysisContext context, StatementSyntax statement, StatementSyntax precedingStatement)
		{
			statement.DescendantNodes().OfType<ExpressionStatementSyntax>().ForEach(it => ReportSyntaxNode(context, it, precedingStatement, "branch"));
		}

		private void ReportSyntaxNode(SyntaxNodeAnalysisContext context, SyntaxNode node, SyntaxNode precedingNode, string errorMessageDiscriminator)
		{
			context.ReportDiagnostic(Diagnostic.Create(
						   Description,
						   node.GetLocation(),
						   precedingNode.GetLineNumberToReport(),
						   errorMessageDiscriminator));
		}

		public IEnumerable<StatementSyntax> GetPrecedingStatementsInConditionChain(IfStatementSyntax ifStatement) => GetPrecedingIfsInConditionChain(ifStatement).Select(i => i.Statement);

		public IEnumerable<ExpressionSyntax> GetPrecedingConditionsInConditionChain(IfStatementSyntax ifStatement) => GetPrecedingIfsInConditionChain(ifStatement).Select(i => i.Condition);

		public IList<IfStatementSyntax> GetPrecedingIfsInConditionChain(IfStatementSyntax ifStatement)
		{
			var ifList = new List<IfStatementSyntax>();
			var currentIf = ifStatement;

			while (currentIf.Parent is ElseClauseSyntax &&
				currentIf.Parent.Parent is IfStatementSyntax)
			{
				var precedingIf = (IfStatementSyntax)currentIf.Parent.Parent;
				ifList.Add(precedingIf);
				currentIf = precedingIf;
			}

			ifList.Reverse();
			return ifList;
		}

		public IEnumerable<SwitchSectionSyntax> GetPrecedingSections(SwitchSectionSyntax caseStatement)
		{
			if (caseStatement == null)
			{
				return new SwitchSectionSyntax[0];
			}

			var switchStatement = (SwitchStatementSyntax)caseStatement.Parent;
			var currentSectionIndex = switchStatement.Sections.IndexOf(caseStatement);
			return switchStatement.Sections.Take(currentSectionIndex);
		}

		public bool AreEquivalent(SyntaxList<SyntaxNode> nodeList1, SyntaxList<SyntaxNode> nodeList2)
		{
			if (nodeList1.Count != nodeList2.Count)
			{
				return false;
			}

			for (var i = 0; i < nodeList1.Count; i++)
			{
				if (!nodeList1[i].IsEquivalent(nodeList2[i]))
				{
					return false;
				}
			}
			return true;
		}

		private class IfStatementDefinition : NodeDefinition
		{
			public IfStatementSyntax IfStatement { get; set; }
		}
	}
}
