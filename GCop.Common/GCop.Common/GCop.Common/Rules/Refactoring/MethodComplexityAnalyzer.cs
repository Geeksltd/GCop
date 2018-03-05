namespace GCop.Common.Rules.Refactoring
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class MethodComplexityAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
	{
		readonly int MaximumOfMethods = 6;
		protected override SyntaxKind Kind => SyntaxKind.MethodDeclaration;
		Dictionary<string, List<InvocationNode>> Invocations;
		List<string> Parameters;
		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "628",
				Category = Category.Refactoring,
				Severity = DiagnosticSeverity.Info,
				Message = "Maybe define this method on '{0}' class as it's using {1} of its members (compared to {2} from this type)"
			};
		}

		protected override void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;

			var method = NodeToAnalyze as MethodDeclarationSyntax;

			Parameters = new List<string>();
			Invocations = new Dictionary<string, List<InvocationNode>>();

			if (method.Modifiers.Any(it => it.Kind() == SyntaxKind.StaticKeyword || it.Kind() != SyntaxKind.PublicKeyword)) return;
			if (method.Body == null) return;

			//Rule should be skipped when there is any type which is instantiated by NEW keyword
			if (method.Body.DescendantNodes().OfType<ExpressionSyntax>().Any(it => it.Kind() == SyntaxKind.ObjectCreationExpression)) return;

			var classDeclaration = method.GetSingleAncestor<ClassDeclarationSyntax>();
			if (classDeclaration == null) return;

			var classInfo = context.SemanticModel.GetDeclaredSymbol(classDeclaration);
			//if (!classInfo.IsInherited<IEntity>()) return;

			//Rule should be skipped when the type is in MSharp namespace
			if (classInfo.ContainingNamespace.ToString().StartsWith("MSharp.Framework")) return;

			var methodInfo = context.SemanticModel.GetDeclaredSymbol(method) as IMethodSymbol;
			if (methodInfo == null) return;

			if (methodInfo.Parameters.Any())
				Parameters.AddRange(methodInfo.Parameters.Select(it => it.ToString()));

			var validParameter = new List<IParameterSymbol>();

			foreach (var param in method.ParameterList.Parameters)
			{
				var parameter = context.SemanticModel.GetDeclaredSymbol(param) as IParameterSymbol;

				//When the parameter type is not inherited from IEntity or is inherited from IUser the rule should be skipped
				//if (!parameter.IsInherited<IEntity>() || parameter.IsInherited<IUser>()) continue;

				FindAllInvocation(classInfo, method.Body, context.SemanticModel);
			}

			if (Invocations.None()) return;

			var maximumInvocations = Invocations.Aggregate((left, right) => CalculateWeight(left.Value) > CalculateWeight(right.Value) ? left : right);
			var rightClassToOwnMethod = maximumInvocations;

			var className = (classDeclaration.Parent?.As<NamespaceDeclarationSyntax>().Name.ToString()).Or("") + "." + classDeclaration.Identifier.ValueText;
			if (rightClassToOwnMethod.Key == className) return;

			var rightClass = rightClassToOwnMethod.Value.FirstOrDefault()?.ContainingSymbol; //rightClassToOwnMethod.FirstOrDefault().Value.FirstOrDefault()?.ContainingSymbol;
			if (rightClass == null) return;

			if (rightClass.GetSymbolType().GetMembers().Count(it => it.As<IMethodSymbol>()?.MethodKind == MethodKind.Ordinary) > MaximumOfMethods) return;

			var thisClassMembers = Invocations.FirstOrDefault(it => it.Key == className).Value?.Count ?? 0;

			var rightClassMemberWithoutNamespace = rightClassToOwnMethod.Value?.FirstOrDefault()?.ContainingSymbol?.Name ?? rightClassToOwnMethod.Key;

			ReportDiagnostic(context, method.Identifier, rightClassMemberWithoutNamespace, maximumInvocations.Value.Count.ToString(), thisClassMembers.ToString());
		}

		private void FindAllInvocation(ISymbol declaringType, BlockSyntax block, SemanticModel semanticModel)
		{
			foreach (var invocation in block.DescendantNodes().OfType<IdentifierNameSyntax>()
				.Where(it => it.Parent is MemberAccessExpressionSyntax ||
				it.Parent is InvocationExpressionSyntax ||
				it.Parent is AssignmentExpressionSyntax))
			{
				ExtractInvocationNode(declaringType, invocation, semanticModel);
			}
		}

		private void ExtractInvocationNode(ISymbol declaringType, SyntaxNode node, SemanticModel semanticModel)
		{
			var symbol = semanticModel.GetSymbolInfo(node).Symbol;
			if (symbol == null) return;

			if (symbol.DeclaredAccessibility == Accessibility.Private) return;
			//if (!symbol.ContainingSymbol.IsInherited<IEntity>()) return;
			if (symbol.IsStatic) return;

			if (symbol.IsInherited<IEnumerable>())
			{
				if (Parameters.Intersects(symbol.GetSymbolType().ToString())) return;
			}

			if (symbol.ContainingSymbol != declaringType && Parameters.Lacks(symbol.ContainingSymbol.ToString())) return;

			AddToDictionary(new InvocationNode
			{
				Name = symbol.ToString(),
				IsMethod = symbol.Kind == SymbolKind.Method,
				ContainingSymbol = symbol.ContainingSymbol
			});
		}

		private void AddToDictionary(InvocationNode node)
		{
			var key = node.ContainingSymbol.ToString();
			if (Invocations.ContainsKey(key))
			{
				if (Invocations[key].Any(it => it.Name == node.Name)) return;
				Invocations[key].Add(node);
				return;
			}
			Invocations.Add(node.ContainingSymbol.ToString(), new List<InvocationNode>(new[] { node }));
		}

		private ISymbol GetOriginalContainingType(ISymbol symbol)
		{
			var parent = symbol.ContainingType;
			while (parent != null)
			{
				if (parent.ContainingType != null)
					parent = parent.ContainingType;
				else break;
			}
			return parent;
		}

		private int CalculateWeight(IEnumerable<InvocationNode> invocations)
		{
			if (invocations == null) return 0;
			if (invocations.None()) return 0;

			var countMethods = invocations.Count(it => it.IsMethod) * 5;
			var countOthers = invocations.Count(it => !it.IsMethod);
			return countMethods + countOthers;
		}

		private class InvocationNode : NodeDefinition
		{
			public static readonly InvocationNode Default = default(InvocationNode);
			public bool IsMethod { get; set; }
			public ISymbol ContainingSymbol { get; set; }
		}
	}
}
