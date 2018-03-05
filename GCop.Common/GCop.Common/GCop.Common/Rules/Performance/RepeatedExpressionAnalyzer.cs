namespace GCop.Common.Rules.Performance
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Collections.Generic;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class RepeatedExpressionAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
	{
		const int MinSizeLimitation = 40;
		const int MiddleSizeLimitation = 50;
		const int MaxSizeLimitation = 70;
		const int SkipLineNumberofRepeatedItems = 10;

		protected override SyntaxKind Kind => SyntaxKind.MethodDeclaration;

		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "317",
				Category = Category.Performance,
				Severity = DiagnosticSeverity.Warning,
				Message = @"This code is repeated {0} times in this method. If its value remains the same during the method execution, store it in a variable. Otherwise define a method (or Func<T> variable) instead of repeating the expression. [{1}]"

			};
		}

		protected override void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;

			var methodBody = (NodeToAnalyze as MethodDeclarationSyntax).Body;
			if (methodBody == null) return;

			var allNodes = methodBody.DescendantNodes().OfType<CSharpSyntaxNode>()
				.Except(x => x.Kind() == SyntaxKind.StringLiteralExpression)
				.Except(x => x.Kind() == SyntaxKind.ObjectCreationExpression)
				.Except(x => x.AncestorsAndSelf().OfKind(SyntaxKind.SimpleLambdaExpression).Any())
				.Except(x => x.AncestorsAndSelf().OfKind(SyntaxKind.ParenthesizedLambdaExpression).Any())
				.Except(x => x.Kind() == SyntaxKind.ParenthesizedLambdaExpression)
				.Except(x => x.Kind() == SyntaxKind.ArgumentList)
				.Except(x => x.Kind() == SyntaxKind.Argument)
				.Except(x => x.Kind() == SyntaxKind.ExpressionStatement)
				.Except(x => x.Kind() == SyntaxKind.Block)
				.Except(x => x.Kind() == SyntaxKind.QueryBody)
				.Except(x => x.Kind() == SyntaxKind.SelectClause)
				.Except(x => x.Kind() == SyntaxKind.LocalDeclarationStatement)
				.Except(x => x.Kind() == SyntaxKind.YieldReturnStatement)
				.Except(x => x.Kind() == SyntaxKind.VariableDeclaration)
				.Except(x => x.Kind() == SyntaxKind.SimpleMemberAccessExpression)
				.Except(x => x.Kind() == SyntaxKind.VariableDeclarator)
				.Except(x => x.Kind() == SyntaxKind.CatchClause)
				.Except(x => (x.Kind() == SyntaxKind.InvocationExpression) && IsVoidInvocation(context, x))
				.Except(x => (x.Kind() == SyntaxKind.InvocationExpression) && IsStringBuilder(context, x));

			if (allNodes.None()) return;

			allNodes = allNodes.Where(x => x.ToString().Length > MinSizeLimitation);

			var toWarn = allNodes.GroupBy(n => n.ToString()).Where(group => group.HasMany())
				.Where(group => group.Count() >= GetAllowedRepetition(group.Key));

			var nodesToWarn = toWarn.WithMax(group => group.Key.Length);
			if (nodesToWarn == null) return;
			if (nodesToWarn.None()) return;

			var remainednodesToWarn = ExcludeNodsFromMinimumLineRepetitions(nodesToWarn);
			var numberofRepeat = toWarn.WithMax(group => group.Key.Length).Count();

			remainednodesToWarn.Do(node => ReportDiagnostic(context, node as CSharpSyntaxNode, numberofRepeat.ToString(), node.Kind().ToString()));
		}

		private bool IsStringBuilder(SyntaxNodeAnalysisContext context, CSharpSyntaxNode node)
		{
			if (node == null) return false;
			var invoc = node as InvocationExpressionSyntax;
			if (invoc == null) return false;
			var identifier = invoc.GetIdentifierSyntax();
			if (identifier == null)
			{// checking the member access  sb.AppendLine();
				var memberAccess = invoc.ChildNodes().OfType<MemberAccessExpressionSyntax>().FirstOrDefault();
				if (memberAccess == null) return false;

				var varibale = memberAccess.ChildNodes().First();
				if (varibale == null) return false;
				identifier = varibale.GetIdentifierSyntax();
				if (identifier == null) return false;
			}
			if (identifier == null) return false;
			var variableInfo = context.SemanticModel.GetSymbolInfo(identifier).Symbol as IFieldSymbol;
			if (variableInfo == null)
			{
				var localSymbol = context.SemanticModel.GetSymbolInfo(identifier).Symbol as ILocalSymbol;
				if (localSymbol == null) return false;

				if (localSymbol.Type.ToString().Contains("StringBuilder")) return true;
			}
			else
			{
				if (variableInfo.Type.ToString().Contains("StringBuilder")) return true;
				return false;
			}

			return false;
		}

		private bool IsVoidInvocation(SyntaxNodeAnalysisContext context, SyntaxNode node)
		{
			if (node == null) return false;
			var invoc = node as InvocationExpressionSyntax;
			if (invoc == null) return false;
			var identifier = invoc.GetIdentifierSyntax();
			if (identifier == null)
			{// checking the member access  student.CalcSTH();
				var memberAccess = invoc.ChildNodes().OfType<MemberAccessExpressionSyntax>().FirstOrDefault();
				if (memberAccess == null) return false;

				var methodCall = memberAccess.ChildNodes().LastOrDefault();
				if (methodCall == null) return false;
				identifier = methodCall.GetIdentifierSyntax();
				if (identifier == null) return false;
			}
			if (identifier == null) return false;
			var methodInfo = context.SemanticModel.GetDeclaredSymbol(identifier) as IMethodSymbol;
			if (methodInfo == null) return false;
			return methodInfo.ReturnsVoid;
		}

		private int GetAllowedRepetition(string code)
		{
			var length = code.Length;
			var operators = code.Except(c => c.IsLetterOrDigit() || c == ' ' || c == '\"').Count();

			if (operators < 4) return 10;

			if (length <= MinSizeLimitation) return 5;

			else if (length >= MaxSizeLimitation) return 3;

			else //MiddleSize: 50 < x <70 
				return 3;
		}

		private IEnumerable<CSharpSyntaxNode> ExcludeNodsFromMinimumLineRepetitions(IEnumerable<CSharpSyntaxNode> nodes)
		{
			var count = nodes.Count() - 1;

			var excludedNodefFromGroup = nodes.Where((item, index) =>

												(index < count) &&

												 (nodes.ToArray()[index + 1].GetLineNumberToReport() - item.GetLineNumberToReport()) > SkipLineNumberofRepeatedItems).Select(x => x);

			return nodes.Except(excludedNodefFromGroup);
		}
	}
}
