namespace GCop.Common.Rules.Performance
{
    using Core;
    using Core.Syntax;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class UseDirectIdPropertyAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
	{
		private string Type = "MSharp.Framework.Database";
		protected override SyntaxKind Kind => SyntaxKind.InvocationExpression;

		private InvocationExpressionSyntax Invocation;
		private SemanticModel SemanticModel;
		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "304",
				Category = Category.Performance,
				Message = "Property named {0}Id should be used instead",
                Severity = DiagnosticSeverity.Warning
			};
		}

		protected override void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			Invocation = context.Node as InvocationExpressionSyntax;
			if (Invocation == null) return;

			SemanticModel = context.SemanticModel;

			var method = SemanticModel.GetSymbolInfo(Invocation).Symbol as IMethodSymbol;
			if (method == null) return;

			if (method.ReceiverType.ToString() == Type) return;

			Invocation.ArgumentList.Arguments.Where(it => it.Expression is SimpleLambdaExpressionSyntax).ForEach(it =>
			{
				var lambda = it.Expression as SimpleLambdaExpressionSyntax;
				if (lambda == null || lambda.Body == null) return;

				if (lambda.Body.IsKind(SyntaxKind.EqualsExpression))
				{
					var validation = Validation(lambda.Parameter.ToString(), lambda.Body as BinaryExpressionSyntax);

					if (!validation.IsValid)
						ReportDiagnostic(context, lambda.Body, validation.Errors.First().Message);
				}
				else
				{
					lambda.Body.DescendantNodes().OfType<BinaryExpressionSyntax>().ForEach(equalsClause =>
					{
						var validation = Validation(lambda.Parameter.ToString(), equalsClause);

						if (!validation.IsValid)
						{
							ReportDiagnostic(context, validation.Errors.First().ErrorLocation, validation.Errors.First().Message);
						}
					});
				}
			});
		}

		private ValidationResult Validation(string originLambdaParameter, BinaryExpressionSyntax equalsClause)
		{
			ExpressionSyntax notNullHand = null;

			if (equalsClause.Right.IsKind(SyntaxKind.NullLiteralExpression)) notNullHand = equalsClause.Left;
			else if (equalsClause.Left.IsKind(SyntaxKind.NullLiteralExpression)) notNullHand = equalsClause.Right;

			if (notNullHand is MemberAccessExpressionSyntax invocation)
			{
				var lambdaIdentifer = invocation.DescendantNodes().OfType<IdentifierNameSyntax>().FirstOrDefault(it => it.Identifier.ValueText == originLambdaParameter);
				if (lambdaIdentifer == null) return ValidationResult.Ok;

				var lambdaParameter = SemanticModel.GetSymbolInfo(lambdaIdentifer).Symbol as IParameterSymbol;
				if (lambdaParameter == null) return ValidationResult.Ok;

				var members = lambdaParameter.Type.GetMembers();
				var possiblePropertyId = new[] { invocation.Name + "Id", invocation + "ID" };
				if (members.Any(it => possiblePropertyId.Contains(it.Name)))
				{
					return new ValidationResult(new ValidationError { Message = invocation.Name.ToString(), ErrorLocation = invocation.GetLocation() });
				}
			}

			return ValidationResult.Ok;
		}
	}
}
