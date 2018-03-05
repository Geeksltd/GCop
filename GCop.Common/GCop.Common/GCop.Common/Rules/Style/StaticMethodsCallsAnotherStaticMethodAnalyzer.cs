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
	public class StaticMethodsCallsAnotherStaticMethodAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
	{
		protected override SyntaxKind Kind => SyntaxKind.MethodDeclaration;

		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "433",
				Category = Category.Style,
				Severity = DiagnosticSeverity.Warning,
				Message = "Class name is unnecessary here. Static members can be called directly."
			};
		}

		protected override void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var method = context.Node as MethodDeclarationSyntax;

			var methodSymbol = context.SemanticModel.GetDeclaredSymbol(method) as IMethodSymbol;
			if (methodSymbol == null) return;
			if (methodSymbol.IsStatic == false) return;

			//first of all, we are looking for any calls like this => MyClass.Method2();
			var bodyMembers = method.Body?.DescendantNodes().OfType<MemberAccessExpressionSyntax>();
			if (bodyMembers == null) return;
			if (bodyMembers.None()) return;

			var parentClass = method.GetSingleAncestor<ClassDeclarationSyntax>();
			if (parentClass == null) return;
			var parentClassSymbol = context.SemanticModel.GetDeclaredSymbol(parentClass) as ISymbol;
			if (parentClassSymbol == null) return;

			/* this is a pattern which we are looking for           
             class MyClass{
                static void SomeMember() { }
                static void AnotherMember()
                {
                    MyClass.SomeMember(); // ----- Show warning under MyClass to say it's unnecessary. 
                 }                
             }*/

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
