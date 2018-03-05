namespace GCop.Conditional.Rules.Refactoring
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class IfNullablePropertyAccessAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.IfStatement;
        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "636",
                Category = Category.Refactoring,
                Severity = DiagnosticSeverity.Warning,
                Message = "It should be written as if ( {0}?.{1} == someValue)"
            };
        }

        /// <summary>
        /// this analyzer looks for below pattern 
        /// if (P1 != null && P1.P2 == someValue)
        ///      Then warn to say it should be written as: 
        ///      if (P1?.P2 == someValue)
        ///      if someValue is "null" then skip the rule*/
        /// </summary>       
        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var condition = (NodeToAnalyze as IfStatementSyntax).Condition;

            var tokens = condition.DescendantTokens().Select(x => x.ValueText.Trim());
            //if (P1 != null && P1.P2 == someValue)           
            var pattern = new[] { "!=", "null", "&&", "==" };
            if (tokens.LacksAny(pattern)) return;

            // also each sign in pattern: ["!=", "null", "&&", "=="] should be exist only one time in if condition
            var groups = tokens.GroupBy(x => x).Where(x => pattern.Contains(x.Key));
            var multi = groups.Where(grp => grp.HasMany());

            if (multi.Any()) return;

            var binaryAndExpression = condition as BinaryExpressionSyntax;
            if (binaryAndExpression == null) return;

            var leftSide = binaryAndExpression.Left;
            var rightSide = binaryAndExpression.Right;

            if (leftSide.ToString().KeepReplacing(" ", "").Lacks("!=")) return;
            if (rightSide.ToString().Lacks("==")) return;

            // parsing the left side for:  p1 != null  or  null != p1            
            if (leftSide == null) return;

            var p1LeftVariable = leftSide.GetIdentifier();
            if (p1LeftVariable.IsEmpty()) return;

            //parsing right side :   P1.P2 == someValue

            var someValue = rightSide.GetIdentifier();  // if some value == null skip it
            if (someValue == "null") return;

            //P1.P2() == someValue
            var rightSideInvocation = rightSide.ChildNodes().OfType<InvocationExpressionSyntax>().FirstOrDefault();
            MemberAccessExpressionSyntax rightNodeMemberAccess;
            if (rightSideInvocation != null)
            {
                rightNodeMemberAccess = rightSideInvocation.ChildNodes().OfType<MemberAccessExpressionSyntax>().FirstOrDefault();
            }
            else
                rightNodeMemberAccess = rightSide.ChildNodes().OfType<MemberAccessExpressionSyntax>().FirstOrDefault();

            if (rightNodeMemberAccess == null) return;

            var rightP2 = rightNodeMemberAccess.ChildNodes().OfType<IdentifierNameSyntax>().LastOrDefault();
            if (rightP2 == null) return;

            if (rightNodeMemberAccess.GetIdentifier() != p1LeftVariable) return;
            ReportDiagnostic(context, condition, p1LeftVariable, rightP2.GetIdentifier());
        }
    }
}
