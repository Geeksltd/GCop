namespace GCop.Common.Rules.Design
{
	using Core;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using Microsoft.CodeAnalysis.Diagnostics;
	using System;
	using System.Linq;
	using Utilities;

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AvoidUsingValuePropertyOfNullableTypesAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        SyntaxKind[] ExcludedKinds = new SyntaxKind[] { SyntaxKind.AddExpression, SyntaxKind.DivideExpression, SyntaxKind.MultiplyExpression, SyntaxKind.SubtractExpression, SyntaxKind.ModuloExpression, SyntaxKind.ExclusiveOrExpression };
        protected override SyntaxKind Kind => SyntaxKind.SimpleMemberAccessExpression;


        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "171",
                Category = Category.Design,
                Message = "There is no need for calling .Value. Replace with {0}.",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            var memberAccess = context.Node as MemberAccessExpressionSyntax;
            if (memberAccess == null) return;
            NodeToAnalyze = memberAccess;

            var property = context.SemanticModel.GetSymbolInfo(memberAccess).Symbol as IPropertySymbol;
            if (property == null) return;

            if (property?.ContainingType?.ToString().Contains("System.Collections.Generic.KeyValuePair") ?? true) return;

            if (property.Name != "Value") return;
            if (property.ContainingType.ToString().Lacks("?")) return;

            if (!(memberAccess.Parent is BinaryExpressionSyntax)) return;

            var variableName = memberAccess.GetIdentifier();
            if (variableName.IsEmpty()) return;

            var binaryExpression = memberAccess.GetParent<BinaryExpressionSyntax>().As<BinaryExpressionSyntax>();
            if (binaryExpression == null) return;
            if (ExcludedKinds.Contains(binaryExpression.Kind())) return;

            // if (!HasAnyNullCheck(binaryExpression, variableName))
            ReportDiagnostic(context, memberAccess, binaryExpression.ToString().ReplaceWholeWord(".Value", ""));
        }
        #region
        //private bool HasAnyNullCheck(BinaryExpressionSyntax usingValueExpression, string variableName)
        //{
        //    var conditions = new List<ConditionDefinition>();

        //    BinaryExpressionSyntax nullCheckingExpression = null;
        //    var lastLogicalAnd = usingValueExpression.GetParent<BinaryExpressionSyntax>().As<BinaryExpressionSyntax>();
        //    nullCheckingExpression = lastLogicalAnd;

        //    //conditions.Add(ConditionDefinition.Parse(nullCheckingExpression));

        //    while (lastLogicalAnd != null)
        //    {
        //        lastLogicalAnd = lastLogicalAnd.GetParent<BinaryExpressionSyntax>().As<BinaryExpressionSyntax>();
        //        if (lastLogicalAnd != null)
        //        {
        //            nullCheckingExpression = lastLogicalAnd;
        //            conditions.Add(ConditionDefinition.Parse(nullCheckingExpression));
        //        }
        //    }

        //    //    conditions.Reverse();

        //    if (nullCheckingExpression != null && nullCheckingExpression.IsKind(SyntaxKind.LogicalAndExpression))
        //    {
        //        var hasNullChecking = nullCheckingExpression.DescendantNodes().OfType<BinaryExpressionSyntax>().TrueForAtLeastOnce(it =>
        //        {
        //            if (it.Kind() == SyntaxKind.NotEqualsExpression && it.ChildNodes().Any(token => token.IsKind(SyntaxKind.NullLiteralExpression)))
        //            {
        //                return it.GetIdentifier() == variableName;
        //            }
        //            return false;
        //        });

        //        if (!hasNullChecking)
        //        {
        //            return nullCheckingExpression.DescendantNodes().OfType<MemberAccessExpressionSyntax>().TrueForAtLeastOnce(it =>
        //            {
        //                if (it.GetIdentifier() != variableName) return false;

        //                var propertyInfo = SemanticModel.GetSymbolInfo(it).Symbol as IPropertySymbol;
        //                var expressionIsHasValue = propertyInfo?.Name == "HasValue" && (propertyInfo?.ContainingType.ToString().IsAnyOf("int?", "double?", "decimal?", "System.DateTime?", "bool?") ?? false);
        //                return expressionIsHasValue;
        //            });
        //        }
        //        return hasNullChecking;
        //    }
        //    return false;
        //}

        //private class ConditionDefinition:NodeDefinition
        //{
        //    public string Expression { get; set; }

        //    public static ConditionDefinition Parse(BinaryExpressionSyntax expression)
        //    {
        //        return new ConditionDefinition { Index = -1, Expression = expression.ToString(), Name = expression.GetIdentifier() };
        //    }
        //}               
        #endregion
    }
}
