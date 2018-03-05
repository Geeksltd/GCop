namespace GCop.MSharp.Rules.Refactoring
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
    public class MethodCallInDatabaseUpdateAnalyzer : GCopAnalyzer
    {
        protected override void Configure() => RegisterSyntaxNodeAction(Analyze, SyntaxKind.InvocationExpression);

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "637",
                Category = Category.Refactoring,
                Severity = DiagnosticSeverity.Warning,
                Message = @"This will be invoked twice: once for a clone of the object, and once for the actual object.
The method result may be different on the second call, leading to inconsistency.
Instead save the result of the method call in a local variable before Database.Update() call and set [{0}] to that variable."
            };
        }

        protected void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var invocation = context.Node as InvocationExpressionSyntax;

            var memberaccess = invocation.ChildNodes().OfType<MemberAccessExpressionSyntax>().FirstOrDefault();
            if (memberaccess == null) return;

            var memberIdenfier = memberaccess.ChildNodes().OfType<IdentifierNameSyntax>().FirstOrDefault();
            if (memberIdenfier == null) return;
            if (memberIdenfier.Identifier == null) return;
            if (memberIdenfier.Identifier.ValueText != "Database") return;

            var method = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
            if (method == null || method.Name != "Update") return;

            if (invocation.ArgumentList.IsNone()) return;
            if (invocation.ArgumentList.Arguments.None()) return;
            var updateItemsExperssion = invocation.ArgumentList.Arguments.Last()?.Expression;

            foreach (var invoc in updateItemsExperssion.DescendantNodes().OfType<InvocationExpressionSyntax>())
            {
                if (IsStringMethod(invoc, context)) return;

                var parentSimpleMember = invoc.Parent as AssignmentExpressionSyntax;
                if (parentSimpleMember == null) continue;

                var access = parentSimpleMember.ChildNodes().OfKind(SyntaxKind.SimpleMemberAccessExpression).FirstOrDefault();
                if (access == null) continue;

                var lastIdentifier = access.ChildNodes().OfKind(SyntaxKind.IdentifierName).LastOrDefault();
                var property = lastIdentifier?.GetIdentifier();
                if (property.IsEmpty()) continue;



                ReportDiagnostic(context, invoc, property);
            }
        }

        bool IsStringMethod(InvocationExpressionSyntax invoc, SyntaxNodeAnalysisContext context)
        {
            // for skipping string function Like: string.Format, "".Trim() , ....
            if (invoc.ChildNodes().OfKind(SyntaxKind.SimpleMemberAccessExpression).None()) return false;

            var member = invoc.ChildNodes().OfKind(SyntaxKind.SimpleMemberAccessExpression).FirstOrDefault();
            if (member == null) return false;

            var invocIdentifirer = member.GetIdentifierSyntax();
            if (invocIdentifirer != null)
            {
                if (context.SemanticModel.GetSymbolInfo(invocIdentifirer).Symbol is IMethodSymbol symbol)
                {
                    if (symbol.Is<string>()) return true;
                }
            }

            return false;
        }
    }
}