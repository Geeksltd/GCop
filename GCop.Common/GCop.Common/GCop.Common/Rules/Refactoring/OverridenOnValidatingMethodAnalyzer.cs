namespace GCop.Common.Rules.Refactoring
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class OverridenOnValidatingMethodAnalyzer : GCopAnalyzer
	{
		protected override void Configure() => RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration);

		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "631",
				Category = Category.Refactoring,
				Severity = DiagnosticSeverity.Warning,
				Message = @"All validation logic should be written inside Validate() method.
OnValidating is meant to be used for special cases such as setting late-bound default values."
			};
		}

		protected void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			if ((NodeToAnalyze as MethodDeclarationSyntax).GetName() != "OnValidating") return;

			//here checking that OnValidating method is  Override ?
			Evaluation(context);
			var methodbody = (NodeToAnalyze as MethodDeclarationSyntax).Body;
			if (methodbody == null) return;

			// Finding  calls another method whose name starts with Validate...()
			var childs = methodbody.DescendantNodes().OfKind(SyntaxKind.InvocationExpression);
			if (childs.None()) return;

			var methodsHasValidatenTheirName = childs.Where(x => x.GetIdentifier() != null).Where(it => it.GetIdentifier().StartsWith("Validate"));
			if (methodsHasValidatenTheirName.None())
			{
				// for this situation: this.Validate();
				var members = childs.SelectMany(x => x.ChildNodes().OfKind(SyntaxKind.SimpleMemberAccessExpression));
				if (members.None()) return;

				methodsHasValidatenTheirName = members.Where(x => x.GetIdentifier() != null).Where(it => it.GetIdentifier().StartsWith("Validate"));
			}

			var identifier = methodsHasValidatenTheirName.FirstOrDefault()?.GetIdentifierSyntax();
			if (identifier == null) return;
			ReportDiagnostic(context, identifier);
		}

		private void Evaluation(SyntaxNodeAnalysisContext context)
		{
			if (context.SemanticModel.GetDeclaredSymbol(NodeToAnalyze as MethodDeclarationSyntax).IsOverride == false)
				return;

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

			//finding throw new ValidationException();
			var throws = methodbody.ChildNodes().OfKind(SyntaxKind.ThrowStatement);

			foreach (var itemThrow in throws)
			{
				var newException = itemThrow.ChildNodes().OfType<ObjectCreationExpressionSyntax>().FirstOrDefault();
				if (newException == null) continue;

				var identif = (newException.ChildNodes().FirstOrDefault() as IdentifierNameSyntax).GetIdentifier();
				if (identif.IsEmpty()) continue;

				if (identif != "ValidationException") continue;
				ReportDiagnostic(context, newException);
				return;
			}
		}
	}
}