namespace GCop.Common.Rules.Style
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    /// <summary>
    /// @17662
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class StaticMethodsBeingCalledinPropertyGetterAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
	{
		protected override SyntaxKind Kind => SyntaxKind.GetAccessorDeclaration;

		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "434",
				Category = Category.Style,
				Severity = DiagnosticSeverity.Warning,
				Message = "Class name is unnecessary here. Static members can be called directly."
			};
		}

		protected override void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var getAccesor = context.Node as AccessorDeclarationSyntax;
			if (getAccesor.Body == null) return;

			//looking for any calls like this => MyClass.Method2();
			var bodyMembers = getAccesor.Body?.DescendantNodes()?.OfType<MemberAccessExpressionSyntax>();
			if (bodyMembers.None()) return;

			var parentClass = getAccesor.GetSingleAncestor<ClassDeclarationSyntax>();
			if (parentClass == null) return;
			var parentClassSymbol = context.SemanticModel.GetDeclaredSymbol(parentClass) as ISymbol;
			if (parentClassSymbol == null) return;

			foreach (var memberAccess in bodyMembers)
			{
				var firstIdentifier = memberAccess.ChildNodes().OfType<IdentifierNameSyntax>().FirstOrDefault();

				if (firstIdentifier == null) continue;
				if (firstIdentifier.GetIdentifier() != parentClass.GetName()) continue;

				var lastIdentifier = memberAccess.ChildNodes().OfType<IdentifierNameSyntax>().LastOrDefault();
				if (lastIdentifier == null) continue;

				//checking the Another memeber method
				var secondStaticMethod = lastIdentifier.GetIdentifierSyntax();
				if (secondStaticMethod == null) continue;

				var secondMethod = context.SemanticModel.GetSymbolInfo(secondStaticMethod).Symbol as IMethodSymbol;
				if (secondMethod == null) continue;

				if (secondMethod.IsStatic == false) continue;

				if (secondMethod.ContainingType.Name != parentClass.GetName()) continue;
				ReportDiagnostic(context, firstIdentifier);
			}
		}
	}
}
