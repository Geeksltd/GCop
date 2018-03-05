namespace GCop.Common.Rules.Naming
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class AttributeClassMustEndWithAttributeAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
	{
		protected override SyntaxKind Kind => SyntaxKind.ClassDeclaration;

		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "200",
				Category = Category.Naming,
				Message = "Since the class is an attribute, the name of the class must end with 'Attribute'",
                Severity = DiagnosticSeverity.Warning
			};
		}

		protected override void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var classDeclaration = context.Node as ClassDeclarationSyntax;
			if (classDeclaration.BaseList == null) return;

			foreach (var baseType in classDeclaration.BaseList?.Types)
			{
				var type = context.SemanticModel.GetSymbolInfo(baseType.Type);
				var isAttributeClass = type.Symbol?.ToString() == "System.Attribute";

				if (isAttributeClass)
				{
					// must end with Attribute - 'A' being capital
					var className = classDeclaration.Identifier.ValueText;
					if (className.EndsWith("attribute") || !className.EndsWith("Attribute"))
					{
						var diagnostic = Diagnostic.Create(Description, classDeclaration.Identifier.GetLocation());
						context.ReportDiagnostic(diagnostic);
					}
				}
			}
		}
	}
}