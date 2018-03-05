namespace GCop.Common.Rules.Style
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class BooleanParameterAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
	{
		protected override SyntaxKind Kind => SyntaxKind.MethodDeclaration;

		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "408",
				Category = Category.Style,
				Severity = DiagnosticSeverity.Warning,
				Message = "Boolean parameters should go after all non-optional parameters."
			};
		}

		protected override void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var methodDeclaration = NodeToAnalyze as MethodDeclarationSyntax;
			var method = context.SemanticModel.GetDeclaredSymbol(methodDeclaration) as IMethodSymbol;
			if (method == null) return;

			if (method.IsOverride) return;
			if (method.Parameters.None()) return;

			var parameters = method.Parameters.Select((node, index) =>
			{
				return new ParameterDefinitionSyntax
				{
					Index = index,
					Name = node.Name,
					IsParams = node.IsParams,
					TypeName = node.Type.ToString(),
					IsOptional = node.IsOptional,
					Location = methodDeclaration.ParameterList.Parameters.First(it => it.Identifier.ValueText == node.Name).GetLocation(),
				};
			});

			parameters.Where(it => it.TypeName == "bool").ForEach(parameter =>
			{
				var notBooleanParameters = parameters.Where(it => it.TypeName != "bool");
				bool isErrorShown = false;
				foreach (var item in notBooleanParameters)
				{
					if (!isErrorShown && !item.IsOptional && !item.IsParams && item.Index > parameter.Index)
					{
						ReportDiagnostic(context, parameter.Location);
						isErrorShown = true;
					}
				}
			});
		}

		private class ParameterDefinitionSyntax : ParameterDefinition
		{
			public bool IsParams { get; set; }
			public bool IsOptional { get; set; }
		}
	}
}
