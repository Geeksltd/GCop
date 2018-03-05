namespace GCop.Common.Rules.Refactoring
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class SetMethodinValidateMethodAnalyzer : GCopAnalyzer
	{
		protected override void Configure() => RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration);

		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "632",
				Category = Category.Refactoring,
				Severity = DiagnosticSeverity.Warning,
				Message = @"Use OnValidating() for setting late-bound properties. Validate() should only be used for validation, without changing the object state."
			};
		}

		protected void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			if ((NodeToAnalyze as MethodDeclarationSyntax).GetName() != "Validate") return;

			//here checking that OnValidating method is  Override ?
			//if (context.SemanticModel.GetDeclaredSymbol(NodeToAnalyze as MethodDeclarationSyntax).IsOverride == false )return;

			//checking the inheritance from Entity
			var classNode = NodeToAnalyze.GetSingleAncestor<ClassDeclarationSyntax>() as ClassDeclarationSyntax;
			if (classNode == null) return;

			var symbolClass = context.SemanticModel.GetDeclaredSymbol(classNode);
			if (symbolClass == null) return;

			//var baseClass = symbolClass.BaseType;
			//var inherit = symbolClass.IsInherited<MSharp.Framework.IEntity>();
			//if (inherit == false) return;

			var methodbody = (NodeToAnalyze as MethodDeclarationSyntax).Body;
			if (methodbody == null) return;

			// Looking for any assing propery to show warning
			// Bug : based @16229 then commenting blow code
			/*var assings = methodbody.DescendantNodes().OfKind(SyntaxKind.SimpleAssignmentExpression);
            if (assings.Any())
            {
                var firstAssignment = assings.FirstOrDefault();
                if (firstAssignment != null)
                    ReportDiagnostic(context, firstAssignment);
            }*/

			// Finding  calls another method whose name starts with Set...()
			var childs = methodbody.DescendantNodes().OfKind(SyntaxKind.InvocationExpression);
			if (childs.None()) return;

			var methodsHasValidatenTheirName = childs.Where(x => x.GetIdentifier() != null).Where(it => it.GetIdentifier().StartsWith("Set"));
			if (methodsHasValidatenTheirName.None())
			{
				// for this situation: this.Set();
				var members = childs.SelectMany(x => x.ChildNodes().OfKind(SyntaxKind.SimpleMemberAccessExpression));
				if (members.None()) return;

				methodsHasValidatenTheirName = members.Where(x => x.GetIdentifier() != null).Where(it => it.GetIdentifier().StartsWith("Set"));
			}

			var identifier = methodsHasValidatenTheirName.FirstOrDefault()?.GetIdentifierSyntax();
			if (identifier == null) return;

			var methodDecalr = context.SemanticModel.GetSymbolInfo(identifier).Symbol as IMethodSymbol;

			if (methodDecalr == null) return;
			ReportDiagnostic(context, identifier);
		}
	}
}