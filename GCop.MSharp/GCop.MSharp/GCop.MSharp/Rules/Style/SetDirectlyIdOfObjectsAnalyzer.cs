namespace GCop.MSharp.Rules.Style
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SetDirectlyIdOfObjectsAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.SimpleAssignmentExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "411",
                Category = Category.Style,
                Severity = DiagnosticSeverity.Warning,
                Message = "Do not set the ID of a new object. It's automatically set to a new Guid by the framework in the constructor."
            };
        }
        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var assignment = context.Node as AssignmentExpressionSyntax;

            var leftSide = assignment.Left as IdentifierNameSyntax;

            //Rule should be skipped if the value set is Guid.Empty
            if (assignment.Right?.ToString() == "Guid.Empty") return;

            if (leftSide == null)
            {
                //something.ID
                var leftSideExpression = assignment.Left as MemberAccessExpressionSyntax;
                if (leftSideExpression == null) return;

                var objectIdentifier = leftSideExpression.GetIdentifierSyntax();
                if (objectIdentifier == null) return;

                var leftSideSymbol = context.SemanticModel.GetSymbolInfo(objectIdentifier).Symbol;
                if (leftSideSymbol == null) return;

                //if (leftSideSymbol.IsInherited<IEntity>())
                //{
                //    var idProperty = leftSideExpression.ChildNodes().OfType<IdentifierNameSyntax>().LastOrDefault();
                //    if (idProperty == null) return;

                //    if (idProperty?.Identifier.ValueText == "ID" && context.SemanticModel.GetSymbolInfo(idProperty).Symbol?.As<IPropertySymbol>()?.Type.ToString() == "System.Guid")
                //    {
                //        ReportDiagnostic(context, leftSideExpression);
                //    }
                //}

                return;
            }

            //new Something{Id = Guid.NewGuid()}
            if (leftSide?.Identifier.ValueText != "ID") return;
            if (context.SemanticModel.GetSymbolInfo(leftSide as IdentifierNameSyntax).Symbol?.As<IPropertySymbol>()?.Type.ToString() != "System.Guid") return;

            var objectCreationExpression = leftSide.GetSingleAncestor<ObjectCreationExpressionSyntax>();
            if (objectCreationExpression == null)
            {
                var parent = leftSide.GetSingleAncestor<ClassDeclarationSyntax>();

                if (parent?.BaseList.Types.Any(it => it.Type.ToString() == "GuidEntity") == true)
                {
                    ReportDiagnostic(context, leftSide.Identifier);
                    return;
                }
            }
            else
            {
                var objectCreationInfo = context.SemanticModel.GetSymbolInfo(objectCreationExpression.Type).Symbol;
                if (objectCreationInfo == null) return;

                //if (objectCreationInfo.IsInherited<IEntity>())
                //{
                //    ReportDiagnostic(context, leftSide.Identifier);
                //}
            }
        }
    }
}
