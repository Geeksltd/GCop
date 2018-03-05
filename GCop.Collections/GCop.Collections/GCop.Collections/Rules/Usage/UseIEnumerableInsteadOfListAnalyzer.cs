namespace GCop.Collections.Rules.Usage
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Collections.Generic;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class UseIEnumerableInsteadOfListAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
	{
		static List<string> MethodsName = new List<string>();
		protected override SyntaxKind Kind => SyntaxKind.MethodDeclaration;

		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "529",
				Category = Category.Usage,
				Severity = DiagnosticSeverity.Warning,
				Message = "Use {0} instead."
			};
		}

		protected override void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var method = (MethodDeclarationSyntax)context.Node;

			if (method.Body == null) return;

			foreach (var parameter in method.ParameterList.Parameters)
			{
				var parameterInfo = context.SemanticModel.GetDeclaredSymbol(parameter) as IParameterSymbol;
				if (parameterInfo == null) continue;
				if (parameterInfo.Type.As<INamedTypeSymbol>()?.ConstructedFrom?.ToString() != "System.Collections.Generic.List<T>") continue;

				if (MethodsName.None())
					MethodsName.AddRange(parameterInfo.Type
						.GetMembers()
						.Where(it => it.Kind == SymbolKind.Method && it.As<IMethodSymbol>()?.MethodKind == MethodKind.Ordinary)
						.Select(it => it.Name));

				MethodsName.AddRange(new[] { "RemoveWhere", "AddLine", "AddFormat" });

				if (!method.Body.DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>().TrueForAtLeastOnce(it =>
				{
					var argument = it.ArgumentList.Arguments.FirstOrDefault(x => x.Expression.ToString() == parameter.Identifier.ToString());
					var argumentIndex = it.ArgumentList.Arguments.IndexOf(argument);
					if (argument != null && argumentIndex > -1)
					{
						var invocationInfo = context.SemanticModel.GetSymbolInfo(it).Symbol as IMethodSymbol;
						var argumentInfo = invocationInfo.Parameters.ToList()[argumentIndex];
						return argumentInfo?.Type.Name.IsAnyOf("IList", "List") ?? false;
					}
					var methodInfo = context.SemanticModel.GetSymbolInfo(it).Symbol as IMethodSymbol;
					return MethodsName.Contains(methodInfo?.Name);
				}))
					ReportDiagnostic(context, parameter.Type, parameter.Type.ToString().Replace("List<", "IEnumerable<"));
			}
		}
	}
}
