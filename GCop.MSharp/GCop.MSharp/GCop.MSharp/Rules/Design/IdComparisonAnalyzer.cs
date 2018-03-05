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
    public class IdComparisonAnalyzer : GCopAnalyzer
    {
        private SyntaxKind[] Kinds = new SyntaxKind[] { SyntaxKind.EqualsExpression, SyntaxKind.NotEqualsExpression };
        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "147",
                Category = Category.Design,
                Message = "Instead of comparing the Id properties, just compare the objects directly.",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Configure()
        {
            RegisterSyntaxNodeAction(context => AnalyzeContext(context), SyntaxKind.EqualsExpression, SyntaxKind.NotEqualsExpression);
        }

        protected void AnalyzeContext(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var expression = context.Node as BinaryExpressionSyntax;
            if (Kinds.Lacks(expression.Kind()))
                return;

            var invocation = expression.GetParent<InvocationExpressionSyntax>();
            if (invocation == null)
                return;

            var method = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
            if (method == null || method.ContainingType.Name != "Database" || method.ContainingAssembly.Name != "MSharp.Framework")
                return;

            SyntaxNode leftHand = expression.Left as MemberAccessExpressionSyntax;
            SyntaxNode rightHand = expression.Right as MemberAccessExpressionSyntax;

            if (rightHand == null || leftHand == null)
                return;

            if (!(rightHand.ToString().ToLower().EndsWith("id") && leftHand.ToString().ToLower().EndsWith("id"))) return;
            //if (rightHand.ToString().ToLower().EndsWith("id") && leftHand.ToString().ToLower().EndsWith("id"))

            var leftObjectCreationIdentifier = GetIdentifier(rightHand);
            var rightObjectCreationIdentifier = GetIdentifier(leftHand);
            if (leftObjectCreationIdentifier == null || rightObjectCreationIdentifier == null)
                return;

            var leftHandObject = context.SemanticModel.GetSymbolInfo(leftObjectCreationIdentifier).Symbol;
            var rightHandObject = context.SemanticModel.GetSymbolInfo(rightObjectCreationIdentifier).Symbol;

            //var showError = false;
            //if(rightHand.ToString() != "Guid.Empty")
            //    showError = IsInheritedFromIEntity(rightHandObject);
            //if(leftHand.ToString() != "Guid.Empty")
            //    showError = IsInheritedFromIEntity(leftHandObject);

            if (IsInheritedFromIEntity(rightHandObject) && IsInheritedFromIEntity(leftHandObject))
            {
                ReportDiagnostic(context, expression);
            }
        }

        private bool IsInheritedFromIEntity(ISymbol symbol)
        {
            if (symbol is IFieldSymbol)
                return (symbol as IFieldSymbol).Type.AllInterfaces.Any(it => it.Name == "IEntity");
            if (symbol is ILocalSymbol)
                return (symbol as ILocalSymbol).Type.AllInterfaces.Any(it => it.Name == "IEntity");
            if (symbol is IPropertySymbol)
                return (symbol as IPropertySymbol).Type.AllInterfaces.Any(it => it.Name == "IEntity");
            return false;
        }

        IdentifierNameSyntax GetIdentifier(SyntaxNode node)
        {
            var identifiers = node.DescendantNodes().OfType<IdentifierNameSyntax>().ToList();
            var idIdentifier = identifiers.FirstOrDefault(it => it.Identifier.ValueText.ToLower() == "id");
            if (idIdentifier == null)
                return null;
            var containingTypeIndex = identifiers.IndexOf(idIdentifier) - 1;
            if (containingTypeIndex < 0)
                return null;
            return identifiers[containingTypeIndex];
        }
    }
}