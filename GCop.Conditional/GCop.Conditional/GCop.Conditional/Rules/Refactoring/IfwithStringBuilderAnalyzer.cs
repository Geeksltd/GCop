namespace GCop.Conditional.Rules.Refactoring
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class IfwithStringBuilderAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.IfStatement;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "626",
                Category = Category.Refactoring,
                Severity = DiagnosticSeverity.Warning,
                Message = "The condition is unnecessary."
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var ifStatement = NodeToAnalyze as IfStatementSyntax;

            var varibaleinIf = GetVariableIfHasValue(ifStatement, context);
            if (varibaleinIf.IsEmpty()) return;

            var block = ifStatement.ChildNodes().OfKind(SyntaxKind.Block);
            if (block.Any())
            {
                if (block.FirstOrDefault().ChildNodes().OfType<CSharpSyntaxNode>().HasMany()) return;
            }

            //here we are sure the  if statment has only one line after itself
            var expresStatement = ifStatement.ChildNodes().OfKind(SyntaxKind.ExpressionStatement).LastOrDefault() as ExpressionStatementSyntax;
            if (expresStatement == null)
            {
                expresStatement = block.FirstOrDefault()?.ChildNodes().OfKind(SyntaxKind.ExpressionStatement).LastOrDefault() as ExpressionStatementSyntax;
            }
            if (expresStatement == null) return;

            var invocation = expresStatement.ChildNodes().OfKind(SyntaxKind.InvocationExpression).LastOrDefault() as InvocationExpressionSyntax;
            if (invocation == null) return;

            var membreAccess = invocation.ChildNodes().OfKind(SyntaxKind.SimpleMemberAccessExpression).FirstOrDefault();
            if (membreAccess == null) return;

            //finding myStringBuilder variable in this statemetn=>   //myStringBuilder.Append(XX);
            var identifier = membreAccess.ChildNodes().OfKind(SyntaxKind.IdentifierName).FirstOrDefault();
            if (identifier == null) return;

            var symbol = context.SemanticModel.GetSymbolInfo(identifier).Symbol;
            if (symbol == null) return;

            var localVariable = symbol as ILocalSymbol;
            if (localVariable == null)
            {
                var pubVariable = symbol as IFieldSymbol;
                if (pubVariable == null) return;
                if (pubVariable.Type.ToString().Lacks("StringBuilder")) return;
            }
            if (localVariable.Type.ToString().Lacks("StringBuilder")) return;

            //finding Append method variable in this statemetn=>   //myStringBuilder.Append(XX);
            var method = membreAccess.ChildNodes().OfKind(SyntaxKind.IdentifierName).LastOrDefault();
            if (method == null) return;
            if (method.GetIdentifier() != "Append") return;

            //Finding the argument XX in here  //myStringBuilder.Append(XX);

            if (invocation.ArgumentList == null) return;
            if (invocation.ArgumentList.IsNone()) return;
            if (invocation.ArgumentList.Arguments == null) return;
            if (invocation.ArgumentList.Arguments.None()) return;

            //@16355 : if the argument of Append() contains any expression other than just the variable, then the rule should be skipped
            if (invocation.ArgumentList.Arguments.HasMany()) return;

            var argument = invocation.ArgumentList.Arguments.FirstOrDefault();
            if (argument == null) return;

            var expersion = argument.Expression;
            if (expersion.IsNotKind(SyntaxKind.IdentifierName)) return;  //like @16355 : if the ...

            if (argument.ChildNodes().Any(x => x.GetIdentifier() != null && x.GetIdentifier() == varibaleinIf))
                ReportDiagnostic(context, ifStatement);
        }

        string GetVariableIfHasValue(IfStatementSyntax ifstatement, SyntaxNodeAnalysisContext context)
        {
            //if(XX.HasValue()) ==> HasValue() is msharp method ?
            var invocation = ifstatement.ChildNodes().OfKind(SyntaxKind.InvocationExpression).FirstOrDefault() as InvocationExpressionSyntax;
            if (invocation == null) return null;

            var simpleMember = invocation.ChildNodes().OfKind(SyntaxKind.SimpleMemberAccessExpression).FirstOrDefault() as MemberAccessExpressionSyntax;
            if (simpleMember == null) return null;

            var lastIdentifier = simpleMember.ChildNodes().OfKind(SyntaxKind.IdentifierName).LastOrDefault();
            if (lastIdentifier == null) return null;

            if (lastIdentifier.GetIdentifier() == "HasValue")
            {
                // Check Msharp method 
                var methodInfo = context.SemanticModel.GetSymbolInfo(lastIdentifier).Symbol as IMethodSymbol;

                if (methodInfo == null) return null;
                if (methodInfo?.ContainingAssembly == null) return null;

                if (methodInfo?.ContainingAssembly?.ToString().StartsWith("MSharp.Framework") == false) return null;

                var first = simpleMember.ChildNodes().OfKind(SyntaxKind.IdentifierName).FirstOrDefault();
                if (first == null) return null;
                if (first.GetIdentifier() == lastIdentifier.GetIdentifier()) return null;
                return first.GetIdentifier();
            }
            else
                return null;
        }
    }
}
