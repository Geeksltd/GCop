namespace GCop.Common.Rules.Usage
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class DefineAttributeAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
	{
		MemberAccessExpressionSyntax Member = null;

		protected override SyntaxKind Kind => SyntaxKind.SimpleMemberAccessExpression;

		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "534",
				Category = Category.Usage,
				Severity = DiagnosticSeverity.Warning,
				Message = "Use yourMemberInfo.Defines<TYPE> instead."
			};
		}

		protected override void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;

			Member = NodeToAnalyze as MemberAccessExpressionSyntax;
			var identifiers = Member.ChildNodes().OfType<IdentifierNameSyntax>();
			var last = identifiers.LastOrDefault();
			if (last == null) return;

			if (last.GetIdentifierSyntax() == null) return;
			var attributeAccess = new[] { "GetCustomAttributes", "IsDefined" };
			if (attributeAccess.Contains(last.GetIdentifierSyntax().GetIdentifier()) == false) return;

			if (IsDefinedMethod() == false &&
			   IsGetCustomAttributesWithLength(context) == false
			   ) return;

			var symbol = context.SemanticModel.GetSymbolInfo(last.GetIdentifierSyntax()).Symbol as IMethodSymbol;
			if (symbol == null) return;

			var containgType = symbol.ContainingType;//NamedType System.Attribute , // System.Reflection.MemberInfo

			if (containgType.ToString().IsAnyOf("System.Attribute", "System.Reflection.MemberInfo"))
			{
				ReportDiagnostic(context, NodeToAnalyze);
			}
		}


		/// <summary>
		/// checking this: Attribute.IsDefined(p1i, typeof(Class1));
		/// </summary>        
		bool IsDefinedMethod()
		{
			var identifires = Member.ChildNodes().OfType<IdentifierNameSyntax>();
			if (identifires.FirstOrDefault()?.Identifier.ValueText != "Attribute") return false;
			if (identifires.LastOrDefault()?.Identifier.ValueText != "IsDefined") return false;

			if ((Member.Parent as InvocationExpressionSyntax)?.GetLastToken().GetNextToken().ValueText == ";") return true;

			return false;
		}

		/// <summary>
		/// Checking 1=> var attr = (T[])pi.GetCustomAttributes(typeof(T), false).Lengh>0;  
		/// </summary>        
		bool IsGetCustomAttributesWithLength(SyntaxNodeAnalysisContext context)
		{
			var identifires = Member.ChildNodes().OfType<IdentifierNameSyntax>();
			var firstVariable = identifires.FirstOrDefault()?.GetIdentifierSyntax();
			if (firstVariable == null) return false;

			ISymbol fieldSymbol = context.SemanticModel.GetSymbolInfo(firstVariable).Symbol as IFieldSymbol;
			if (fieldSymbol != null)
			{
				if ((fieldSymbol as IFieldSymbol).Type.ToString() != "System.Reflection.PropertyInfo") return false;
			}
			else
			{
				fieldSymbol = context.SemanticModel.GetSymbolInfo(firstVariable).Symbol as ILocalSymbol;
				if (fieldSymbol == null) return false;

				if ((fieldSymbol as ILocalSymbol).Type.ToString() != "System.Reflection.PropertyInfo") return false;
			}

			var parent = Member.Parent as InvocationExpressionSyntax;
			if (parent == null) return false;

			var grandParent = parent.Parent as MemberAccessExpressionSyntax;
			if (grandParent == null) return false;

			if (grandParent.ChildNodes().OfType<IdentifierNameSyntax>().LastOrDefault()?.GetIdentifier() == "Length") return true;

			return false;
		}

		/// <summary>        
		/// Checking 2=> var attributes = memberInfo.GetCustomAttributes(typeof(T), false).FirstOrDefault();if (attributes == null)
		/// Checking 3->type.GetRuntimeProperties().Where(pi=>pi.GetCustomAttributes<T>(true).Any());
		/// </summary>        
		//bool IsGetCustomAttributesAny(SyntaxNodeAnalysisContext context)
		//{
		//    return false;
		//}
	}
}


