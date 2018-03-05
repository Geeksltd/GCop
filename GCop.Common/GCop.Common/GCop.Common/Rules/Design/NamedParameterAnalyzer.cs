namespace GCop.Common.Rules.Design
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NamedParameterAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        private readonly SyntaxKind[] NotAllowedKinds = new SyntaxKind[] { SyntaxKind.TrueLiteralExpression, SyntaxKind.FalseLiteralExpression, SyntaxKind.NullLiteralExpression };
        protected override SyntaxKind Kind => SyntaxKind.InvocationExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "117",
                Category = Category.Design,
                Message = "The meaning of \"{0}\" is not obvious. Specify the parameter's name explicitly.",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var invocation = context.Node as InvocationExpressionSyntax;
            if (invocation == null) return;

            var method = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
            var parameters = method?.Parameters.Select((param, index) => new ParameterDefinition { Index = index, Name = param.Name, TypeName = param.Type.ToString() }).ToList();
            if (parameters == null) return;

            var parameterIndex = -1;

            invocation.ArgumentList.Arguments.ToList().ForEach(parameter =>
            {
                parameterIndex++;

                if (!IsValid(parameter))
                {
                    var isBool = false;
                    if (parameters.Count >= parameterIndex + 1)
                        isBool = parameters[parameterIndex].TypeName == "bool";

                    var isNullableBool = false;
                    if (parameters.Count >= parameterIndex + 1)
                        isNullableBool = parameters[parameterIndex].TypeName == "bool?";

                    //There is no way to get the parameter name within InvocationExpression, so I have to achive that with parameter index which is provided by SemanticModel
                    if (parameter.ChildNodes().OfType<NameColonSyntax>().None() && (isBool || isNullableBool))
                    {
                        ReportDiagnostic(context, parameter, parameter.ToString());
                    }
                }
            });
        }

        private bool IsValid(ArgumentSyntax argument)
        {
            var expression = argument?.Expression;
            if (expression == null) return true;

            if (expression.Ancestors().OfType<SimpleLambdaExpressionSyntax>().Any()) return true;

            //If expression is String.Empty rather than string.Empty, it will handle with another analyzer
            if (expression.IsKind(SyntaxKind.SimpleMemberAccessExpression) && expression.ToString() == "string.Empty") return false;

            return NotAllowedKinds.Lacks(expression.Kind());
        }
    }
}
