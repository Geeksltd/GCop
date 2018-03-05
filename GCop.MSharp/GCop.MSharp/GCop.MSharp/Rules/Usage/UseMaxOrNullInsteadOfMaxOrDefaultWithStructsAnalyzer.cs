namespace GCop.MSharp.Rules.Usage
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
    public class UseMaxOrNullInsteadOfMaxOrDefaultWithStructsAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.InvocationExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "518",
                Category = Category.Usage,
                Severity = DiagnosticSeverity.Warning,
                Message = "{0}"
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var expression = NodeToAnalyze as InvocationExpressionSyntax;

            if (expression == null)
                return;

            var memberAccessExpression = expression.ChildNodes().OfType<MemberAccessExpressionSyntax>().FirstOrDefault();
            var methodIdentifier = memberAccessExpression?.ChildNodes().LastOrDefault(it => (it is IdentifierNameSyntax) || (it is GenericNameSyntax));

            if (methodIdentifier == null)
                return;

            var nullableTypeCast = GetNullableTypeCast(methodIdentifier);

            var structType = GetStructTypeFromMaxMinOrDefaultMethod(methodIdentifier, context.SemanticModel, nullableTypeCast);

            if (structType.IsEmpty())
                return;

            ReportDiagnostic(context, expression, GetDescriptionMessage(structType, memberAccessExpression, nullableTypeCast));
        }

        private string GetDescriptionMessage(string structType, MemberAccessExpressionSyntax memberAccessExpression, NullableTypeSyntax nullableTypeCast)
        {
            var minOrMax = memberAccessExpression.Name.ToString().Substring(0, 3);

            if (nullableTypeCast == null)
            {
                if (structType == "System.DateTime")
                {
                    return "Default value of DateTime is the birth of Lord Jesus Christ, which is inappropriate for processing. Use MaxOrNull() instead.";
                }
                else
                    return $"Use {minOrMax}OrNull() instead as the default value of {structType} {GetDefaultTypePartOfMessage(structType)} is not a good representation for the notion of \"unknown\".";
            }
            else
            {
                return $"Use {minOrMax}OrNull() instead of {minOrMax}OrDefault() and then remove the cast to {nullableTypeCast}";
            }
        }

        private string GetDefaultTypePartOfMessage(string structType)
        {
            var zeroDefaults = new[]
            {
                "byte",
                "decimal",
                "double",
                "float",
                "int",
                "long",
                "sbyte",
                "short",
                "uint",
                "ulong",
                "ushort"
            };

            return structType.IsAnyOf(zeroDefaults)
                ? "is zero which"
                : string.Empty;
        }

        private NullableTypeSyntax GetNullableTypeCast(SyntaxNode identifier)
        {
            return (identifier.FirstAncestorOfKind(SyntaxKind.InvocationExpression) as InvocationExpressionSyntax)
                ?.ArgumentList.Arguments.FirstOrDefault()
                ?.ChildNodes().OfType<SimpleLambdaExpressionSyntax>().FirstOrDefault()
                ?.ChildNodes().OfType<CastExpressionSyntax>().FirstOrDefault()
                ?.ChildNodes().OfType<NullableTypeSyntax>().FirstOrDefault();
        }

        private string GetStructTypeFromMaxMinOrDefaultMethod(SyntaxNode identifier, SemanticModel semanticModel, NullableTypeSyntax nullableTypeCast)
        {
            var methodInfo = semanticModel.GetSymbolInfo(identifier).Symbol as IMethodSymbol;

            if (methodInfo == null
                || methodInfo.Name.IsNoneOf("MaxOrDefault", "MinOrDefault")
                || (methodInfo.IsNullable() && nullableTypeCast == null)
                || !methodInfo.IsExtensionMethod
                || !methodInfo.ContainingAssembly.ToString().StartsWith("MSharp.Framework")) return null;

            var expressionType = methodInfo.TypeArguments[1];

            return expressionType.TypeKind == TypeKind.Struct
                ? expressionType.ToString()
                : null;
        }
    }
}
