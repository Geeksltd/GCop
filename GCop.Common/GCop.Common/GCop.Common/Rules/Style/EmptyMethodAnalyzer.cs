namespace GCop.Common.Rules.Style
{
    using Core;
    using Core.Attributes;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [Delay(2)]
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class EmptyMethodAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
	{
		protected override SyntaxKind Kind => SyntaxKind.MethodDeclaration;

		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "420",
				Category = Category.Style,
				Severity = DiagnosticSeverity.Warning,
				Message = "Methods should not be empty. {0}"
			};
		}

		protected override void Analyze(SyntaxNodeAnalysisContext context)
		{
			Await();
			if (!CanContinue) return;

			NodeToAnalyze = context.Node;
			var methodNode = (MethodDeclarationSyntax)NodeToAnalyze;
			var message = "";

			if (methodNode.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.OverrideKeyword))) return;
			if (methodNode.ChildNodes().OfType<ExplicitInterfaceSpecifierSyntax>().Any()) return;

			if (methodNode.Body != null
				&& methodNode.Modifiers.None(modifier => modifier.IsKind(SyntaxKind.VirtualKeyword))
				&& IsEmpty(methodNode.Body))
			{
				if (methodNode.GetParent<ClassDeclarationSyntax>() is ClassDeclarationSyntax classDeclaration && classDeclaration.BaseList != null && classDeclaration.BaseList.Types.Any())
				{
					var interfaceSymbol = context.SemanticModel.GetSymbolInfo(classDeclaration.BaseList.Types[0].GetIdentifierSyntax()).Symbol as ITypeSymbol;
					var interfaceMethods = interfaceSymbol.GetMembers().OfType<IMethodSymbol>().ToList();
					if (interfaceMethods.Any(x => x.Name == methodNode.GetName()))
						message = "If it's only for " + interfaceSymbol.Name + " interface compliance, use explicit interface method implementation.";
					ReportDiagnostic(context, methodNode.Identifier.GetLocation(), message);
				}
			}
		}

		private bool IsEmpty(BlockSyntax node) => node.Statements.None() && !ContainsComment(node);

		private bool ContainsComment(BlockSyntax node)
		{
			return ContainsComment(node.OpenBraceToken.TrailingTrivia) || ContainsComment(node.CloseBraceToken.LeadingTrivia);
		}

		private bool ContainsComment(SyntaxTriviaList trivias)
		{
			return trivias.Any(trivia => trivia.IsKind(SyntaxKind.SingleLineCommentTrivia) || trivia.IsKind(SyntaxKind.MultiLineCommentTrivia));
		}
	}
}
