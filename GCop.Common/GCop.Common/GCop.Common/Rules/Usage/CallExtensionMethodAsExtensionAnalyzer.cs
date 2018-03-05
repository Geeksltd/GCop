namespace GCop.Common.Rules.Usage
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Collections.Generic;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class CallExtensionMethodAsExtensionAnalyzer : GCopAnalyzer
	{
		private static readonly SyntaxAnnotation IntroduceExtensionMethodAnnotation = new SyntaxAnnotation("CallExtensionMethodAsExtensionAnalyzerIntroduceExtensionMethod");
		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "501",
				Category = Category.Usage,
				Severity = DiagnosticSeverity.Warning,
				Message = "Do not call '{0}' method of class '{1}' as a static method"
			};
		}

		protected override void Configure() => Context.RegisterCompilationStartAction(AnalyzeCompilation);

		private void AnalyzeCompilation(CompilationStartAnalysisContext compilationContext)
		{
			var compilation = compilationContext.Compilation;
			compilationContext.RegisterSyntaxNodeAction(context => Analyze(context, compilation), SyntaxKind.InvocationExpression);
		}

		private void Analyze(SyntaxNodeAnalysisContext context, Compilation compilation)
		{
			NodeToAnalyze = context.Node;
			var methodInvokeSyntax = context.Node as InvocationExpressionSyntax;
			var childNodes = methodInvokeSyntax.ChildNodes();
			var methodCaller = childNodes.OfType<MemberAccessExpressionSyntax>().FirstOrDefault();
			if (methodCaller == null) return;

			var argumentsCount = CountArguments(childNodes);
			var classSymbol = GetCallerClassSymbol(context.SemanticModel, methodCaller.Expression);
			if (classSymbol == null || !classSymbol.MightContainExtensionMethods) return;
			var methodSymbol = GetCallerMethodSymbol(context.SemanticModel, methodCaller.Name, argumentsCount);
			if (methodSymbol == null || !methodSymbol.IsExtensionMethod) return;
			if (ContainsDynamicArgument(context.SemanticModel, childNodes)) return;

			if (IsSelectingADifferentMethod(childNodes, methodCaller.Name, context.Node.SyntaxTree, methodSymbol, methodInvokeSyntax, compilation)) return;

			ReportDiagnostic(context, methodCaller.GetLocation(), methodSymbol.Name, classSymbol.Name);
		}

		private static bool IsSelectingADifferentMethod(IEnumerable<SyntaxNode> childNodes, SimpleNameSyntax methodName, SyntaxTree tree, IMethodSymbol methodSymbol, ExpressionSyntax invocationExpression, Compilation compilation)
		{
			var parameterExpressions = GetParameterExpressions(childNodes);
			var firstArgument = parameterExpressions.FirstOrDefault();

			var argumentList = CreateArgumentListSyntaxFrom(parameterExpressions.Skip(1));
			var newInvocationStatement = CreateInvocationExpression(firstArgument, methodName, argumentList)
				.WithAdditionalAnnotations(IntroduceExtensionMethodAnnotation);

			var extensionMethodNamespaceUsingDirective = SyntaxFactory.UsingDirective(methodSymbol.ContainingNamespace.ToNameSyntax());
			var speculativeRootWithExtensionMethod = tree.GetCompilationUnitRoot()
				.ReplaceNode(invocationExpression, newInvocationStatement)
				.AddUsings(extensionMethodNamespaceUsingDirective);

			var speculativeModel = compilation.ReplaceSyntaxTree(tree, speculativeRootWithExtensionMethod.SyntaxTree)
				.GetSemanticModel(speculativeRootWithExtensionMethod.SyntaxTree);

			var speculativeInvocationStatement = speculativeRootWithExtensionMethod.SyntaxTree.GetCompilationUnitRoot()
				.GetAnnotatedNodes(IntroduceExtensionMethodAnnotation).Single() as InvocationExpressionSyntax;

			var speculativeExtensionMethodSymbol = speculativeModel.GetSymbolInfo(speculativeInvocationStatement.Expression).Symbol as IMethodSymbol;

			var speculativeFormOfTheMethodSymbol = speculativeExtensionMethodSymbol?.GetConstructedReducedFrom();

			return speculativeFormOfTheMethodSymbol == null || !speculativeFormOfTheMethodSymbol.Equals(methodSymbol);
		}

		private static int CountArguments(IEnumerable<SyntaxNode> childNodes) =>
			childNodes.OfType<ArgumentListSyntax>().Select(s => s.Arguments.Count).FirstOrDefault();

		private static IMethodSymbol GetCallerMethodSymbol(SemanticModel semanticModel, SimpleNameSyntax name, int argumentsCount)
		{
			var symbolInfo = semanticModel.GetSymbolInfo(name);
			return symbolInfo.Symbol as IMethodSymbol ??
					symbolInfo
						.CandidateSymbols
						.OfType<IMethodSymbol>()
						.FirstOrDefault(s => s.Parameters.Length == argumentsCount + 1);
		}

		private static INamedTypeSymbol GetCallerClassSymbol(SemanticModel semanticModel, ExpressionSyntax expression) =>
			semanticModel.GetSymbolInfo(expression).Symbol as INamedTypeSymbol;

		private static bool ContainsDynamicArgument(SemanticModel sm, IEnumerable<SyntaxNode> childNodes)
		{
			return childNodes.OfType<ArgumentListSyntax>().SelectMany(s => s.Arguments)
				  .Any(a => sm.GetTypeInfo(a.Expression).Type?.Name == "dynamic");
		}

		public static IEnumerable<ExpressionSyntax> GetParameterExpressions(IEnumerable<SyntaxNode> childNodes) =>
			childNodes.OfType<ArgumentListSyntax>().SelectMany(s => s.Arguments).Select(s => s.Expression);

		public static ArgumentListSyntax CreateArgumentListSyntaxFrom(IEnumerable<ExpressionSyntax> expressions)
		{
			return
				SyntaxFactory.ArgumentList().AddArguments(expressions.Select(s => SyntaxFactory.Argument(s)).ToArray());
		}

		public static InvocationExpressionSyntax CreateInvocationExpression(ExpressionSyntax sourceExpression, SimpleNameSyntax methodName, ArgumentListSyntax argumentList)
		{
			return
				SyntaxFactory.InvocationExpression(
				SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, sourceExpression, methodName),
				argumentList);
		}
	}
}
