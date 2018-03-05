namespace GCop.Common.Rules.Usage
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class ImmutableMethodsAnalyzer : GCopAnalyzer
	{
		string[] ImmutabelMethodTypes = new[] { "DateTime", "IOrderedEnumerable", "String", "IEnumerable", "Enumerable", "Linq", "MSharpExtensions", "Queryable"/*, "Regex" */};

		string[] ImmutabelMethodNames = new[]
		{          
            //linq
            "OrderByDescending",
			"OrderBy",
			"Count",
			"Aggregate",
			"Cast",
			"Distinct",
			"Except",
			"AsEnumerable",
			"ToArray",
			"Concat" ,
			"AsEnumerable" ,
			"AsQueryable"
			,
            //String
            "Substring",
			"StartsWith",
			"EndsWith",
			"HasValue",
			"Insert",
			"ToLower",
			"ToUpper",
			"Trim" ,
			"TrimAfter",
			"TrimBefore",
			"Concat" ,
			"TrimEnd" ,
			"TrimStart",
			"ToString",
			"Format" ,
			"FormatWith"
		};

		protected override void Configure()
		{
			RegisterSyntaxNodeAction(context => Analyze(context), SyntaxKind.InvocationExpression);
		}

		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "517",
				Category = Category.Usage,
				Severity = DiagnosticSeverity.Warning,
				Message = @"'{0}()' returns a value but doesn't change the object. It's meaningless to call it without using the returned result."
			};
		}

		protected void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var invocationExpressionSyntax = NodeToAnalyze as InvocationExpressionSyntax;
			if (invocationExpressionSyntax == null) return;
			var methodsName = invocationExpressionSyntax.Expression?.GetIdentifier();
			var methodsSyntax = invocationExpressionSyntax.Expression?.GetIdentifierSyntax();
			var insideMemberAccess = (invocationExpressionSyntax.Expression) as MemberAccessExpressionSyntax;
			if (insideMemberAccess != null)
			{
				var lastMethods = insideMemberAccess.ChildNodes().OfType<IdentifierNameSyntax>().LastOrDefault();
				if (lastMethods != null)
				{
					methodsName = lastMethods.GetIdentifier();
					methodsSyntax = lastMethods.GetIdentifierSyntax();
				}
			}
			if (Evaluate(methodsName, methodsSyntax, invocationExpressionSyntax)) return;

			Simplify(context, invocationExpressionSyntax, methodsSyntax, methodsName, insideMemberAccess);
		}

		private bool Evaluate(string methodsName, IdentifierNameSyntax methodsSyntax, InvocationExpressionSyntax invocationExpressionSyntax)
		{
			if (methodsName.IsEmpty()) return true;
			if (methodsSyntax == null) return true;
			var ancesstors = methodsSyntax.Ancestors();
			if (ancesstors.OfType<ReturnStatementSyntax>().Any()) return true;
			if (ancesstors.OfType<YieldStatementSyntax>().Any()) return true;
			if (ancesstors.OfType<ForEachStatementSyntax>().Any()) return true;
			if (ancesstors.OfType<ArgumentSyntax>().Any()) return true;
			if (ancesstors.OfType<IfStatementSyntax>().Any()) return true;
			if (ancesstors.OfType<ArrowExpressionClauseSyntax>().Any()) return true;
			if (ancesstors.OfType<VariableDeclaratorSyntax>().Any()) return true;
			if (ancesstors.OfType<ThrowStatementSyntax>().Any()) return true;
			var semiColon = invocationExpressionSyntax.Parent?.GetLastToken();
			if (semiColon.HasValue)
				if (semiColon.Value.Kind() != SyntaxKind.SemicolonToken) return true;
			if (invocationExpressionSyntax.GetLastToken().GetNextToken().Kind() != SyntaxKind.SemicolonToken)
				return true;
			return false;
		}

		private void Simplify(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocationExpressionSyntax, IdentifierNameSyntax methodsSyntax, string methodsName, MemberAccessExpressionSyntax insideMemberAccess)
		{
			var methodInfo = context.SemanticModel.GetSymbolInfo(invocationExpressionSyntax).Symbol as IMethodSymbol;
			if (methodInfo != null)
			{
				if (methodInfo.ReturnsVoid) return;

				if (methodInfo.ReturnType != null)
				{
					if (methodInfo.ReturnType.TypeKind == TypeKind.Struct && (methodInfo.ReturnType.Name.IsAnyOf("DateTime", "Double")))
					{
						ReportDiagnostic(context, methodsSyntax, methodsName);
						return;
					}
				}
			}
			if (insideMemberAccess != null)
			{
				var varibaleIdentifier = insideMemberAccess.GetIdentifierSyntax();
				var varibaleIdentifierInfo = context.SemanticModel.GetSymbolInfo(varibaleIdentifier).Symbol as ILocalSymbol;
				if (varibaleIdentifier != null && varibaleIdentifierInfo != null)
				{
					if (varibaleIdentifierInfo.Type.TypeKind == TypeKind.Struct)
					{
						ReportDiagnostic(context, methodsSyntax, messageArgs: methodsName);
						return;
					}
				}
			}
			if (methodInfo == null) return;
			if (methodInfo.ContainingSymbol == null) return;
			var baseClassName = methodInfo.ContainingSymbol.Name;
			if (!(ImmutabelMethodNames.Contains(methodsName))) return;
			if (!(ImmutabelMethodTypes.Contains(baseClassName))) return;
			{
				ReportDiagnostic(context, methodsSyntax, messageArgs: methodsName);
			}
		}
	}
}