namespace GCop.Collections.Rules.Performance
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Collections;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class IEnumerableVaribleInForeachAnalyzer : GCopAnalyzer
	{
		protected override void Configure()
		{
			RegisterSyntaxNodeAction(context => Analyze(context), SyntaxKind.ForEachStatement);
		}

		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "318",
				Category = Category.Performance,
				Severity = DiagnosticSeverity.Warning,
				Message = "This will cause the query to be computed multiple times. Instead call .ToList() on the variable declaration line to avoid unwanted extra processing."
			};
		}

		/// <summary>
		/// If you see a variable whose type is exactly IEnumerable<Something> (but not List<Something> or any other type) 
		/// If that variable is used in a foreach more than once in the same method
		/// This is an open query. It will not be evaluated until it's needed 
		/// var matches = Database.GetList<Student>().Where(//i => i.ExpensiveOperation() == true x => x.Code == 0);
		/// This will perform ExpensiveOperation on each element. 
		/// foreach (var match in matches) { ... }
		/// This will perform ExpensiveOperation on each element again! 
		/// foreach (var match in matches) { ... }        
		/// </summary>
		protected void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;

			var forEachStatement = NodeToAnalyze as ForEachStatementSyntax;
			if (forEachStatement == null) return;

			var simpleLocalIdenifier = forEachStatement.ChildNodes().OfType<IdentifierNameSyntax>().LastOrDefault();
			if (simpleLocalIdenifier == null) return;

			if (IsIEnumerbaleT(simpleLocalIdenifier, context) == false) return;

			var allforEachInSameMetod = forEachStatement.GetSingleAncestor<MethodDeclarationSyntax>()?.DescendantNodes().OfType<ForEachStatementSyntax>();
			if (allforEachInSameMetod == null) return;
			if (allforEachInSameMetod.None()) return;

			// it should be more than 1 foreach in a method
			if (allforEachInSameMetod.HasMany() == false) return;

			//Checking every foreach which its location is after the current nodeToAnalyze, and also is using the same local Ienumeable variable
			foreach (var item in allforEachInSameMetod)
			{
				if (item.GetLocation().SourceSpan.Start < forEachStatement.GetLocation().SourceSpan.End) continue;

				//checking the second and other foreach only if they are Ienumerable and has same variable with NodeToAnalyze
				var nextForeachIdentifier = item.ChildNodes().OfType<IdentifierNameSyntax>().LastOrDefault();
				if (nextForeachIdentifier == null) continue;

				if (IsIEnumerbaleT(nextForeachIdentifier, context) == false) continue;

				var identifier = nextForeachIdentifier.GetIdentifier();
				if (identifier.IsEmpty()) continue;

				if (identifier != simpleLocalIdenifier.GetIdentifier()) continue;

				ReportDiagnostic(context, simpleLocalIdenifier);
				ReportDiagnostic(context, nextForeachIdentifier);
			}
		}

		private bool IsIEnumerbaleT(SyntaxNode varible, SyntaxNodeAnalysisContext context)
		{
			// Checking Local Variable 
			// Checking the IEnumerable Type 
			// see a variable whose type is exactly IEnumerable<Something> 

			if (varible == null) return false;
			if (varible.GetIdentifierSyntax() == null) return false;

			var symbolInfo = context.SemanticModel.GetSymbolInfo(varible.GetIdentifierSyntax());

			var sourceLocalSymbol = symbolInfo.Symbol as ILocalSymbol;
			if (sourceLocalSymbol == null) return false;

			if (sourceLocalSymbol.Type is IArrayTypeSymbol arrayType)
			{
				return !arrayType.IsInherited<IList>();
			}

			if (sourceLocalSymbol.Type is INamedTypeSymbol namedTypeSymbol)
			{
				var typeArgument = namedTypeSymbol.TypeArguments.FirstOrDefault();

				if (typeArgument != null)
				{
					return !namedTypeSymbol.IsInherited<IList>();
				}
			}
			return false;
		}
	}
}
