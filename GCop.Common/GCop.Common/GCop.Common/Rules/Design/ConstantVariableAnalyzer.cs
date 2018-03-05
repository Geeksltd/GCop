namespace GCop.Common.Rules.Design
{
	using Core;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using Microsoft.CodeAnalysis.Diagnostics;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class ConstantVariableAnalyzer : GCopAnalyzer
	{
		string[] Exceptions = new[] { "-2", "-1", "0", "1", "2", "3", "4", "5", "12", "1024", "true", "false", "10", "100", "0.1", "0.5", "0.01", "0.001", "0.0001", "0.00001", "1000", "360" };

		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "179",
				Category = Category.Design,
				Severity = DiagnosticSeverity.Warning,
				Message = @"Do not hardcode numbers, strings or other values.
-Declare constant in the top of the file and use it in your code if the value is unlikely to ever change. 
-If the value may change in the future (or on different servers) and the nature of the value is technical, then it must be stored in Web.Config.
-If the nature of the value is business related, or easy to understand by a normal business admin user, then it must be stored in the application in an entity named Setting."
			};
		}

		protected override void Configure() => RegisterSyntaxNodeAction(Analyze, SyntaxKind.NumericLiteralExpression);

		protected void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;

			// variable definiation with constant and static identifter should be skipped
			if (CheckNode()) return;

			if (NodeToAnalyze.Ancestors().OfType<ArgumentSyntax>().Any())
			{
				var invocations = NodeToAnalyze.Ancestors().OfKind(SyntaxKind.InvocationExpression).ToList();
				if (!invocations.None())
					if (CheckInvocation(context, invocations)) return;
			}

			//-- showing only one message for new list {1, 2, 3, 50 , 40};
			if (NodeToAnalyze.Parent?.Kind() == SyntaxKind.ArrayInitializerExpression ||
				NodeToAnalyze.Parent?.Kind() == SyntaxKind.CollectionInitializerExpression)
			{
				var parent = NodeToAnalyze.Parent as InitializerExpressionSyntax;
				var numbers = parent?.Expressions.ToString();

				var beforeNumbers = numbers.Substring(0, numbers.IndexOf(NodeToAnalyze.ToString()));

				var sepratedNums = beforeNumbers.Trim().Split(',');
				for (int i = 0; i < Exceptions.Length; i++)
				{
					var idx = sepratedNums.Select(x => x.Trim()).IndexOf(Exceptions[i]);
					if (idx != -1)
						sepratedNums[idx] = "";
				}

				beforeNumbers = sepratedNums.Where(x => x.Trim().HasValue()).ToString(",");
				if (beforeNumbers.IsEmpty()) return;
				if (beforeNumbers.Contains(",")) return;

				if (NodeToAnalyze.Parent?.Parent?.Kind() == SyntaxKind.ImplicitArrayCreationExpression)
				{
					ReportDiagnostic(context, NodeToAnalyze.Parent.Parent);
				}
				else if (NodeToAnalyze.Parent.Kind() == SyntaxKind.CollectionInitializerExpression)
				{
					ReportDiagnostic(context, NodeToAnalyze.Parent.Parent ?? NodeToAnalyze.Parent);
				}
				else
					ReportDiagnostic(context, NodeToAnalyze.Parent);
			}
			else
				ReportDiagnostic(context, NodeToAnalyze);
		}

		private bool CheckNode()
		{
			if (NodeToAnalyze == null) return true;
			if (NodeToAnalyze.Ancestors().OfType<FieldDeclarationSyntax>().Any()) return true;
			if (NodeToAnalyze.Ancestors().OfType<ArrayCreationExpressionSyntax>().Any()) return true;
			if (NodeToAnalyze.Ancestors().OfType<EnumDeclarationSyntax>().Any()) return true;

			if (NodeToAnalyze is LiteralExpressionSyntax literalExpersion)
			{
				if (literalExpersion.Token.Kind() == SyntaxKind.StringLiteralToken) return true;
				// adding (-4) mines number to check
				var literalValue = literalExpersion.Token.ValueText;
				if (literalExpersion.Parent != null)
				{
					literalValue = literalExpersion.Parent.IsKind(SyntaxKind.UnaryMinusExpression) ? $"-{literalValue}" : literalValue;
				}
				if (literalValue.IsAnyOf(Exceptions)) return true;
			}

			if (NodeToAnalyze.GetSingleAncestor<ClassDeclarationSyntax>()?.Identifier.ValueText == "TaskManager") return true;

			if (NodeToAnalyze.IsKind(SyntaxKind.NumericLiteralExpression))
			{
				var anscestorIdentifier = NodeToAnalyze.GetSingleAncestor<MethodDeclarationSyntax>()?.Identifier;
				if (anscestorIdentifier?.ValueText?.Contains((NodeToAnalyze as LiteralExpressionSyntax).Token.ValueText) == true)
					return true;

				var localVar = NodeToAnalyze.GetSingleAncestor<LocalDeclarationStatementSyntax>();

				if (localVar?.ChildTokens()?.Any(it => it.Kind() == SyntaxKind.ConstKeyword) == true)
				{
					return true;
				}
			}
			return false;
		}

		private bool CheckInvocation(SyntaxNodeAnalysisContext context, List<SyntaxNode> invocations)
		{
			foreach (var lastInvocation in invocations)
			{
				if (lastInvocation != null)
				{
					if (lastInvocation.ToString().Contains(".Summarize(")) return true;
					if (lastInvocation?.Parent != null)
					{
						var parent = lastInvocation.Parent.ToString().ToLower();
						// Thread.Sleep should skip                            
						if (parent.Contains("Thread".ToLower())) return true;

						//  list.ElementAt(5) should be skipped
						if (lastInvocation.Parent.ToString().ToLower().Contains("ElementAt".ToLower())) return true;

						// Msharp functions should be skipped
						var symbol = context.SemanticModel.GetSymbolInfo(lastInvocation).Symbol;

						if (symbol != null)
						{
							if (symbol.ContainingNamespace.ToString().Contains("MSharp")) return true;
							if (symbol.ContainingType.ToString().Contains("MSharp")) return true;

							// Randon.Next function should be skipped
							if (symbol.ContainingType.ToString().Contains("Random")) return true;
							if (parent.Contains("Next".ToLower())) return true;

							// Thread.Task
							if (symbol.ContainingType.ToString().Contains("Threading") &&
								parent.Contains("Task".ToLower())) return true;
						}
					}
				}
			}
			return false;
		}
	}
}
