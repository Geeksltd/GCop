namespace GCop.MSharp.Rules.Design
{
    using Core;
    using Core.Attributes;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [MSharpExclusive]
    [ZebbleExclusive]
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class IsEmptyOrHasValueAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.LogicalNotExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "142",
                Category = Category.Design,
                Message = "Replace {0} with {1}",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var logicalExpression = context.Node as PrefixUnaryExpressionSyntax;
            if (logicalExpression == null) return;

            var notLogicalChilds = logicalExpression.ChildNodes();
            if (notLogicalChilds.HasMany() || notLogicalChilds.None()) return;
            if (notLogicalChilds.First().IsNotKind(SyntaxKind.InvocationExpression)) return;

            var invocation = notLogicalChilds.First().As<InvocationExpressionSyntax>();
            if (invocation == null) return;
            var memberAccessExpression = invocation.Expression as MemberAccessExpressionSyntax;
            if (memberAccessExpression == null) return;

            var method = context.SemanticModel.GetSymbolInfo(memberAccessExpression).Symbol as IMethodSymbol;
            var firstChild = invocation.ChildNodesAndTokens().FirstOrDefault();

            if (method?.Name == "IsEmpty")
            {
                string messageArg1 = "!IsEmpty()";
                string messageArg2 = "HasValue";
                if (firstChild != null)
                {
                    if (firstChild.ToString().EndsWith("IsEmpty"))
                    {
                        messageArg1 = $"!{firstChild}()";
                        messageArg2 = $"{firstChild.ToString().ReplaceWholeWord("IsEmpty", "HasValue")}()";
                    }
                    else
                    {
                        messageArg1 = $"!{firstChild}.IsEmpty()";
                        messageArg2 = $"{firstChild}.HasValue()";
                    }
                }
                ReportDiagnostic(context, logicalExpression, messageArg1, messageArg2);
            }
            else if (method?.Name == "HasValue")
            {
                string messageArg1 = "!HasValue()";
                string messageArg2 = "IsEmpty()";
                if (firstChild != null)
                {
                    if (firstChild.ToString().EndsWith("HasValue"))
                    {
                        messageArg1 = $"!{firstChild}()";
                        messageArg2 = $"{firstChild.ToString().ReplaceWholeWord("HasValue", "IsEmpty")}()";
                    }
                    else
                    {
                        messageArg1 = $"!{firstChild}.HasValue()";
                        messageArg2 = $"{firstChild}.IsEmpty()";
                    }
                }
                ReportDiagnostic(context, logicalExpression, messageArg1, messageArg2);
            }
        }
    }
}
