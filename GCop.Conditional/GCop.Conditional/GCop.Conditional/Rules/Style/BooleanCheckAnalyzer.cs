namespace GCop.Conditional.Rules.Style
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Collections.Generic;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class BooleanCheckAnalyzer : GCopAnalyzer
	{
		private static readonly Dictionary<SyntaxKind, string> OppositeTokens = new Dictionary<SyntaxKind, string>
																				{
																							{SyntaxKind.GreaterThanToken, "<="},
																							{SyntaxKind.GreaterThanEqualsToken, "<"},
																							{SyntaxKind.LessThanToken, ">="},
																							{SyntaxKind.LessThanEqualsToken, ">"},
																							{SyntaxKind.EqualsEqualsToken, "!="},
																							{SyntaxKind.ExclamationEqualsToken, "=="}
																				};

		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "413",
				Category = Category.Style,
				Severity = DiagnosticSeverity.Warning,
				Message = "It should be written as {0}"
			};
		}

		protected override void Configure()
		{
			RegisterSyntaxNodeAction(Analyze, SyntaxKind.GreaterThanExpression,
											  SyntaxKind.GreaterThanOrEqualExpression,
											  SyntaxKind.LessThanExpression,
											  SyntaxKind.LessThanOrEqualExpression,
											  SyntaxKind.EqualsExpression,
											  SyntaxKind.NotEqualsExpression);
		}

		protected void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;

			var binaryExpression = (BinaryExpressionSyntax)NodeToAnalyze;
			var enclosingSymbol = context.SemanticModel.GetEnclosingSymbol(binaryExpression.SpanStart) as IMethodSymbol;

			if (enclosingSymbol?.MethodKind == MethodKind.UserDefinedOperator) return;

			var parenthesizedParent = binaryExpression.Parent;
			while (parenthesizedParent is ParenthesizedExpressionSyntax)
			{
				parenthesizedParent = parenthesizedParent.Parent;
			}

			var logicalNot = parenthesizedParent as PrefixUnaryExpressionSyntax;
			if (logicalNot == null) return;

			if (logicalNot.OperatorToken.IsKind(SyntaxKind.ExclamationToken) == false) return;

			//in this situation  => if (! ( X > 0)) { ....}  ------------------------------
			// when X is nullable<int>:     //If X is nullable, it should be skipped.            
			var identifier = binaryExpression.GetIdentifierSyntax();
			if (identifier != null)
			{
				var symbol = context.SemanticModel.GetSymbolInfo(identifier).Symbol;
				if (symbol != null)
					if (symbol.IsNullable()) return;
			}

			var oppositeToken = OppositeTokens[binaryExpression.OperatorToken.Kind()];
			var messageArg = binaryExpression.ToString().Replace(binaryExpression.OperatorToken.ToString(), oppositeToken);

			ReportDiagnostic(context, logicalNot, messageArg);
		}
	}
}
