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
    public class LacksAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.InvocationExpression;

        InvocationExpressionSyntax Invocation;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "153",
                Category = Category.Design,
                Message = "Instead use {0}.Lacks{1}",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            Invocation = context.Node as InvocationExpressionSyntax;
            if (Invocation == null) return;

            var memberAccessExpression = Invocation.Expression as MemberAccessExpressionSyntax;
            if (memberAccessExpression == null) return;

            var method = context.SemanticModel.GetSymbolInfo(memberAccessExpression).Symbol as IMethodSymbol;

            if (method == null || method.Name != "Contains" || method.ContainingAssembly.Name.IsNoneOf("mscorlib", "System.Core", "MSharp.Framework")) return;

            if (Invocation.ArgumentList.Arguments.HasMany())
            {
                var secondArgument = Invocation.ArgumentList.Arguments[1];
                var secondArgumentType = context.SemanticModel.GetTypeInfo(secondArgument.Expression).Type;

                //Since the Lacks only supports two overloads of Contains so we have to check for the other one
                if (secondArgumentType?.ToString() != "bool") return;
            }
            if (!Invocation.Parent?.IsKind(SyntaxKind.LogicalNotExpression) ?? true) return;

            //arguemnt type must be of type T
            var callerType = ((context.SemanticModel.GetSymbolInfo(memberAccessExpression.GetIdentifierSyntax()).Symbol as ILocalSymbol)?.Type as INamedTypeSymbol)?.TypeArguments[0].Name;
            var arguemntType = (context.SemanticModel.GetSymbolInfo(Invocation.ArgumentList.Arguments[0].GetIdentifierSyntax()).Symbol as ILocalSymbol)?.Type.Name;

            if (callerType != arguemntType) return;

            //var variable = memberAccessExpression.GetIdentifierSyntax();
            //if (variable == null) return;
            //if (context.SemanticModel.GetSymbolInfo(variable).Symbol?.Is("Range") == false)
            ReportDiagnostic(context, Invocation.Expression, Invocation.Expression.ChildNodes().FirstOrDefault()?.ToString(), Invocation.ArgumentList.ToString());
        }
    }
}
