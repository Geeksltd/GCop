namespace GCop.Common.Rules.Design
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class UseVarInsteadOfTypeAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
	{
		private readonly string[] ExcludedMethods = new string[] { "GetBaseDataSource", "GetBaseSource", "GetSource" };
		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "132",
				Category = Category.Design,
				Message = "Since the type is inferred, use 'var' instead",
                Severity = DiagnosticSeverity.Warning
			};
		}

		protected override SyntaxKind Kind => SyntaxKind.LocalDeclarationStatement;

		protected override void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var decNode = context.Node as LocalDeclarationStatementSyntax;
			if (decNode == null) return;
			if (decNode.IsConst) return; // allow const
			if (!IsInValidMethod(decNode)) return;

			var declaration = decNode.Declaration;
			if (declaration.Type.IsVar) return; // if already var, stop analysing

			if (declaration.DescendantTokens().Any(it => it.IsKind(SyntaxKind.IntKeyword))) return;

			// https://msdn.microsoft.com/en-us/library/bb384061.aspx (int i = (i = 20); is valid -> var is invalid, allow such things)
			var assignmentExpressions = declaration.DescendantNodes().OfType<AssignmentExpressionSyntax>();
			if (assignmentExpressions.Any())
			{
				var variableText = declaration.DescendantNodes().OfType<VariableDeclaratorSyntax>().FirstOrDefault().Identifier.ValueText;
				var isAssignmentWithinDeclaration = assignmentExpressions.SelectMany(e => e.DescendantTokens()).Any(t => t.ValueText == variableText);

				if (isAssignmentWithinDeclaration) return;
			}

			var equalsClause = declaration.DescendantNodes().OfType<EqualsValueClauseSyntax>().FirstOrDefault(); // get the right hand side expression
			if (equalsClause == null) return;

			var lhsTypeInfo = context.SemanticModel.GetTypeInfo(declaration.Type, context.CancellationToken);
			var rhsTypeInfo = context.SemanticModel.GetTypeInfo(equalsClause.Value, context.CancellationToken);
			if (lhsTypeInfo.Type == null || rhsTypeInfo.Type == null) return;

			if (lhsTypeInfo.Type.Name.IsAnyOf("Boolean", "Decimal", "Int32", "String", "Int64", "Char")) return;
			if (lhsTypeInfo.Type.Name.IsAnyOf("Action", "Func")) return;

			if (lhsTypeInfo.Type.ToString() == rhsTypeInfo.Type.ToString())
			{
				// if the type inferred from the LHS and RHS are exactly the same, throw error
				var diagnostic = Diagnostic.Create(Description, declaration.Type.GetLocation());
				context.ReportDiagnostic(diagnostic);
			}
		}

		private bool IsInValidMethod(SyntaxNode node)
		{
			while (node?.Parent != null)
			{
				if (node.Parent is MethodDeclarationSyntax)
				{
					var methodName = (node.Parent as MethodDeclarationSyntax)?.Identifier.ValueText;
					if (ExcludedMethods.Contains(methodName))
						return false;
					if (methodName.ToLower().StartsWith("Get".ToLower()) && methodName.ToLower().EndsWith("Source".ToLower()))
						return false;
				}

				node = node.Parent;
			}

			return true;
		}
	}
}
