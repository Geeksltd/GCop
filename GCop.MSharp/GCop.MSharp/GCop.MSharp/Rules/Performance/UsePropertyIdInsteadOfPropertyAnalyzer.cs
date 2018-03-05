namespace GCop.MSharp.Rules.Performance
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
    public class UsePropertyIdInsteadOfPropertyAnalyzer : GCopAnalyzer
    {
        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "316",
                Category = Category.Performance,
                Message = "Use {0} instead as it will be quicker",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Configure()
        {
            RegisterSyntaxNodeAction(Analyze, SyntaxKind.NotEqualsExpression, SyntaxKind.EqualsExpression);
        }

        protected void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var expression = context.Node as BinaryExpressionSyntax;

            if (expression.Ancestors().OfType<InvocationExpressionSyntax>().TrueForAtLeastOnce(it =>
            {
                var method = context.SemanticModel.GetSymbolInfo(it).Symbol as IMethodSymbol;
                var receiverType = method?.ReceiverType?.ToString();
                if (receiverType.IsEmpty()) return false;
                return receiverType.IsAnyOf("MSharp.Framework.Database", "MSharp.Framework.Data.Criterion");
            })) return;

            IdentifierNameSyntax objectIdentifier = null;
            var leftSide = expression.Left as IdentifierNameSyntax;
            if (leftSide == null)
            {
                leftSide = expression.Left.As<MemberAccessExpressionSyntax>()?.ChildNodes().OfType<IdentifierNameSyntax>().LastOrDefault();
                objectIdentifier = expression.Left.As<MemberAccessExpressionSyntax>()?.ChildNodes().OfType<IdentifierNameSyntax>().FirstOrDefault();
            }
            if (leftSide == null) return;

            var leftSideInfo = context.SemanticModel.GetSymbolInfo(leftSide).Symbol;
            //if (!leftSideInfo.IsInherited<IEntity>()) return;

            var propertyId = leftSideInfo.Name + "Id";

            ITypeSymbol containingType = null;

            if (objectIdentifier != null)
            {
                containingType = context.SemanticModel.GetTypeInfo(objectIdentifier).Type;
            }
            else
                containingType = leftSideInfo.ContainingType;

            if (containingType.GetMembers().Any(it => it.Name == propertyId) && (!leftSideInfo?.HasAttribute("SmallTableAttribute") ?? true))
            {
                ReportDiagnostic(context, leftSide, propertyId);
            }
        }
    }
}
