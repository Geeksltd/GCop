namespace GCop.Common.Rules.Naming
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class MeaningfulNameForVariablesAnalyzer : GCopAnalyzer
	{
		private readonly string[] ExcludedTypes = new string[] { "StringBuilder" };
		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "204",
				Category = Category.Naming,
				Message = "Rename the variable '{0}' to something clear and meaningful.",
                Severity = DiagnosticSeverity.Warning
			};
		}

		protected override void Configure()
		{
			RegisterSyntaxNodeAction(context => AnalyzeName(context), SyntaxKind.ParameterList,
																	  SyntaxKind.VariableDeclaration);
		}

		private void AnalyzeName(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var node = context.Node;

			var method = node.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();
			if (method != null && method.AttributeLists.Any(it => it.Attributes.Any(attr => attr.Name.ToString().Contains("DllImport")))) return;

			if (node is VariableDeclarationSyntax variableDeclartion)
			{
				//Skip ForStatements
				if (variableDeclartion.Parent.IsKind(SyntaxKind.ForStatement)) return;

				//Skip excluded types such as StringBuilder
				var objectCreation = variableDeclartion.DescendantNodes().OfType<ObjectCreationExpressionSyntax>().FirstOrDefault();
				if (objectCreation != null && ExcludedTypes.Contains((objectCreation.Type as IdentifierNameSyntax)?.Identifier.ValueText ?? string.Empty)) return;

				variableDeclartion.Variables.ToList().ForEach(it =>
			   {
				   var identifier = it.Identifier.ValueText;

				   var variableTypeName = context.SemanticModel.GetDeclaredSymbol(it).GetSymbolType().Name;
				   if (variableTypeName.EndsWith("EventArgs")) return;

				   //var symbol = context.SemanticModel.GetSymbolInfo(identifier).Symbol as I
				   if (identifier.IsSingleCharacter() && (identifier?.Lacks("_") ?? false))
					   context.ReportDiagnostic(Diagnostic.Create(Description, it.Identifier.GetLocation(), identifier));
			   });
			}
			else if (node is ParameterListSyntax parameterDeclartion)
			{
				if (parameterDeclartion.Parent is ParenthesizedLambdaExpressionSyntax) return;

				parameterDeclartion.Parameters.ToList().ForEach(it =>
				{
					var parameterTypeName = context.SemanticModel.GetDeclaredSymbol(it).GetSymbolType().Name;
					if (parameterTypeName.EndsWith("EventArgs")) return;

					var parameterSymbol = context.SemanticModel.GetDeclaredSymbol(it).Type as ITypeSymbol;
					var parameterName = it.Identifier.ValueText;

					if (parameterName.IsSingleCharacter() && !parameterSymbol.Is(typeof(EventArgs).Name) && (parameterName?.Lacks("_") ?? false))
						context.ReportDiagnostic(Diagnostic.Create(Description, it.GetLocation(), parameterName));
				});
			}
		}
	}
}
