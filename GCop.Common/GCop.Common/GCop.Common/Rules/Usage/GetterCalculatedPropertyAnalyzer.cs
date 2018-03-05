namespace GCop.Common.Rules.Usage
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class GetterCalculatedPropertyAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
	{
		protected override SyntaxKind Kind => SyntaxKind.GetAccessorDeclaration;

		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "514",
				Category = Category.Usage,
				Severity = DiagnosticSeverity.Warning,
				Message = "the method '{0}' must be private as it being used in Getter Accessor."
			};
		}

		protected override void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;

			var getAccessorDeclration = NodeToAnalyze as AccessorDeclarationSyntax;
			if (getAccessorDeclration == null) return;

			// get {return CalculateFullName();}
			var getterBlock = getAccessorDeclration.ChildNodes().FirstOrDefault(x => x.IsKind(SyntaxKind.Block));
			if (getterBlock == null) return;
			if (getterBlock.ChildNodes().HasMany()) return;
			if (getterBlock.ChildNodes().Any(x => x.Kind() == SyntaxKind.IfStatement)) return;
			if (getterBlock.DescendantNodes().Any(x => x.Kind() == SyntaxKind.ConditionalExpression)) return;

			var returnStatement = getterBlock.ChildNodes().FirstOrDefault(x => x.IsKind(SyntaxKind.ReturnStatement));
			if (returnStatement == null) return;

			var invocationExpersion = (returnStatement as ReturnStatementSyntax).ChildNodes()?.Where(x => x.IsKind(SyntaxKind.InvocationExpression))?.FirstOrDefault() as InvocationExpressionSyntax;
			if (invocationExpersion == null) return;

			// here we only have a method in front of return statement like : [return FnX();] OR [return FnX(args:a,b,4);]
			if (invocationExpersion.ArgumentList?.Arguments.Count > 0) return;
			if (invocationExpersion.DescendantNodesAndSelf().Any(it => it.IsAnyKind(SyntaxKind.Argument))) return;

			var invocationIdentifier = invocationExpersion.GetIdentifierSyntax();

			if (invocationIdentifier != null)
			{
				#region     

				var symbolMethod1 = context.SemanticModel.GetSymbolInfo(invocationIdentifier).Symbol as IMethodSymbol;
				if (symbolMethod1 == null) return;

				// if method has any parameters we should skipp it
				if (symbolMethod1.Parameters.Any()) return;

				if (symbolMethod1.ContainingType?.GetMembers() == null) return;
				if (symbolMethod1.ContainingType?.GetMembers().Count(it => it.Name == symbolMethod1.Name) > 1) return; //Method has several signatures/overloads

				var nameClass1 = invocationExpersion.GetSingleAncestor<ClassDeclarationSyntax>()?.GetName();
				var nameNameSpace1 = (invocationExpersion.GetSingleAncestor<NamespaceDeclarationSyntax>() as NamespaceDeclarationSyntax)?.Name;
				if (nameNameSpace1 == null)
				{
					if (symbolMethod1.ContainingSymbol.Name != nameClass1) return;
					if (symbolMethod1.DeclaredAccessibility != Accessibility.Private && symbolMethod1.Name.StartsWith("Calculate"))
					{
						ReportDiagnostic(context, symbolMethod1.Locations.FirstOrDefault(), invocationIdentifier.Identifier.ValueText);
					}
				}
				else
				{
					if (nameNameSpace1.ToString().Lacks(symbolMethod1.ContainingNamespace?.Name)) return;
					if (symbolMethod1.ContainingSymbol.Name != nameClass1) return;
					if (symbolMethod1.DeclaredAccessibility != Accessibility.Private && symbolMethod1.Name.StartsWith("Calculate"))
					{
						ReportDiagnostic(context, symbolMethod1.Locations.FirstOrDefault(), invocationIdentifier.Identifier.ValueText);
					}
				}

				#endregion
			}

			//OR[return this.FnX();] or Other SecondClass().CalculateSTH() ; or new NameSpace.Class().FnName();
			#region
			var memberaccess = getAccessorDeclration.DescendantNodes().OfType<MemberAccessExpressionSyntax>().FirstOrDefault();
			if (memberaccess == null) return;

			invocationIdentifier = memberaccess.GetIdentifierSyntax();
			if (invocationIdentifier == null) return;

			var symbolMethod = context.SemanticModel.GetSymbolInfo(invocationIdentifier).Symbol as IMethodSymbol;
			if (symbolMethod == null) return;

			// if method has any parameters we should skipp it
			if (symbolMethod.Parameters.Any()) return;

			if (symbolMethod.ContainingType?.GetMembers() == null) return;
			if (symbolMethod.ContainingType?.GetMembers().Count(it => it.Name == symbolMethod.Name) > 1) return; //Method has several signatures/overloads

			var nameClass = invocationExpersion.GetSingleAncestor<ClassDeclarationSyntax>()?.GetName();
			var nameNameSpace = (invocationExpersion.GetSingleAncestor<NamespaceDeclarationSyntax>() as NamespaceDeclarationSyntax)?.Name;
			if (nameNameSpace == null)
			{
				if (symbolMethod.ContainingSymbol.Name != nameClass) return;
				if (symbolMethod.DeclaredAccessibility != Accessibility.Private && symbolMethod.Name.StartsWith("Calculate"))
				{
					ReportDiagnostic(context, symbolMethod.Locations.FirstOrDefault(), invocationIdentifier.Identifier.ValueText);
				}
			}
			else
			{
				if (nameNameSpace.ToString().Lacks(symbolMethod.ContainingNamespace?.Name)) return;
				if (symbolMethod.ContainingSymbol.Name != nameClass) return;
				if (symbolMethod.DeclaredAccessibility != Accessibility.Private && symbolMethod.Name.StartsWith("Calculate"))
				{
					ReportDiagnostic(context, symbolMethod.Locations.FirstOrDefault(), invocationIdentifier.Identifier.ValueText);
				}
			}
			#endregion
		}
	}
}
