namespace GCop.Conditional.Rules.Refactoring
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class IfXisNullANDXyIsNullAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        SyntaxNode P1;

        protected override SyntaxKind Kind => SyntaxKind.IfStatement;
        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "639",
                Category = Category.Refactoring,
                Severity = DiagnosticSeverity.Warning,
                Message = "It should be written as if ({0}?.{1} != null)"
            };
        }

        /// <summary>
        /// this analyzer looks for below pattern 
        /// if (x != null && x.P2 != null)        
        /// </summary>       
        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var condition = (NodeToAnalyze as IfStatementSyntax).Condition;

            var tokens = condition.DescendantTokens().Select(x => x.ValueText.Trim());
            //if (P1 != null && P1.P2 != null)           
            var pattern = new[] { "!=", "null", "&&" };
            if (tokens.LacksAny(pattern)) return;

            var binaryAndExpression = condition as BinaryExpressionSyntax;
            if (binaryAndExpression == null) return;


            var allAndExperssion = (condition as BinaryExpressionSyntax).DescendantNodes().OfKind(SyntaxKind.NotEqualsExpression);


            foreach (var item in allAndExperssion)
            {
                var hasNull = item.ChildNodes().OfKind(SyntaxKind.NullLiteralExpression).FirstOrDefault();
                if (hasNull == null) continue;


                if (item.ChildNodes().OfKind(SyntaxKind.SimpleMemberAccessExpression).None())
                {
                    if (P1 == null)
                        P1 = item.ChildNodes().OfKind(SyntaxKind.IdentifierName).FirstOrDefault();
                    if (P1 == null) continue;
                }
                else
                {
                    //P1.P2 != null
                    var rightSideInvocation = item.ChildNodes().OfType<InvocationExpressionSyntax>().FirstOrDefault();
                    MemberAccessExpressionSyntax rightNodeMemberAccess;
                    if (rightSideInvocation != null)
                    {
                        rightNodeMemberAccess = rightSideInvocation.ChildNodes().OfType<MemberAccessExpressionSyntax>().FirstOrDefault();
                    }
                    else
                        rightNodeMemberAccess = item.ChildNodes().OfType<MemberAccessExpressionSyntax>().FirstOrDefault();

                    if (rightNodeMemberAccess == null) continue;

                    var rightP2 = rightNodeMemberAccess.ChildNodes().OfType<IdentifierNameSyntax>().LastOrDefault();
                    if (rightP2 == null) continue;

                    if (rightNodeMemberAccess.GetIdentifier() != P1.GetIdentifier()) continue;
                    ReportDiagnostic(context, condition, P1.GetIdentifier(), rightP2.GetIdentifier());
                }
            }
        }

    }
}
