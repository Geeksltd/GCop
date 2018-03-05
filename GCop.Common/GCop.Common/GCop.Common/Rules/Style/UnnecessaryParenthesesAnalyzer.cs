namespace GCop.Common.Rules.Style
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class UnnecessaryParenthesesAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
	{
		protected override SyntaxKind Kind => SyntaxKind.ParenthesizedExpression;

		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "432",
				Category = Category.Style,
				Severity = DiagnosticSeverity.Warning,
				Message = "Unnecessary paranthesis should be removed."
			};
		}

		protected override void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var self = NodeToAnalyze as ParenthesizedExpressionSyntax;

			//If the file is TaskManager skip this rule 
			if (self.GetSingleAncestor<ClassDeclarationSyntax>()?.GetName() == "TaskManager") return;

			var ret = (self.Parent as ReturnStatementSyntax)?.ChildNodes().OfType<ParenthesizedExpressionSyntax>();
			if (ret != null)
				if (ret.Any())
				{
					ReportDiagnostic(context, ret.FirstOrDefault());
					return;
				}

			var varDeclare = (self.Parent as EqualsValueClauseSyntax)?.ChildNodes()?.OfType<ParenthesizedExpressionSyntax>();
			if (varDeclare != null)
				if (varDeclare.Any())
				{
					if (varDeclare.FirstOrDefault().DescendantNodes().OfKind(SyntaxKind.EqualsExpression).Any()) return;
					ReportDiagnostic(context, varDeclare.FirstOrDefault());
					return;
				}

			//  var xx = (id as object);
			var assingn = (self.Parent as AssignmentExpressionSyntax)?.ChildNodes().OfType<ParenthesizedExpressionSyntax>();
			if (assingn != null)
				if (assingn.Any())
				{
					ReportDiagnostic(context, assingn.FirstOrDefault());
					return;
				}

			// res = Convert.ToString(( Math.Sin(0) + 1));
			if (self.Parent is ArgumentSyntax argument)
			{
				if (self.GetFirstToken().GetPreviousToken() == argument.Parent.GetFirstToken())
					if (self.GetLastToken().GetNextToken() == argument.Parent.GetLastToken())
					{
						ReportDiagnostic(context, argument);
						return;
					}
			}

			//((((((((((((("=  =")))))))))))));
			if (self.Parent is ParenthesizedExpressionSyntax parentiz)
			{
				//((any thing ))
				if (self.GetLocation().SourceSpan.Start == parentiz.GetLocation().SourceSpan.Start + 1)
					if (self.GetLocation().SourceSpan.End == parentiz.GetLocation().SourceSpan.End - 1)
					{
						ReportDiagnostic(context, parentiz);
						return;
					}

				//(( any thing ))
				if (self.GetFirstToken().GetPreviousToken() == parentiz.GetFirstToken())
					if (self.GetLastToken().GetNextToken() == parentiz.GetLastToken())
					{
						ReportDiagnostic(context, parentiz);
						return;
					}
			}

			// (a) && (b)  
			var childNodes = self.DescendantTokens();
			if (childNodes.Any())
				if (childNodes.Count() == 3)
				{
					//cheking cast like ((bool)NewValue)
					if ((self.Parent as CastExpressionSyntax) != null) return;
					if (self.DescendantNodes().OfType<CastExpressionSyntax>().Any()) return;
					if (self.DescendantNodes().OfKind(SyntaxKind.EqualsExpression).Any()) return;

					ReportDiagnostic(context, self);
					return;
				}


			if (self.Parent is IfStatementSyntax ifStatement)
			{
				var logicals = ifStatement.Condition.DescendantNodes().OfType<BinaryExpressionSyntax>()
				.Where(it => it.IsKind(
					SyntaxKind.LogicalAndExpression,
					SyntaxKind.LogicalOrExpression));

				if (logicals.All(x => x.Kind() == SyntaxKind.LogicalAndExpression))
				{
					ReportDiagnostic(context, self);
					return;
				}

				if (logicals.All(x => x.Kind() == SyntaxKind.LogicalOrExpression))
				{
					ReportDiagnostic(context, self);
					return;
				}
			}
			else //do we have special patternt which is not in above items? if yes put in here else part
			{
			}
		}
	}
}
