namespace GCop.MSharp.Rules.Refactoring
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Collections.Generic;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ValidateMethodForHardCodingTheBoundryAnalyzer : GCopAnalyzer
    {
        SyntaxKind[] IncludeConditions = new[]{SyntaxKind.GreaterThanExpression, SyntaxKind.GreaterThanOrEqualExpression ,
                                                         SyntaxKind.LessThanExpression, SyntaxKind.LessThanOrEqualExpression};
        protected override void Configure() => RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration);

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "633",
                Category = Category.Refactoring,
                Severity = DiagnosticSeverity.Warning,
                Message = "Instead of hard-coding the boundary check, set the boundary of the NumericProperty in its M# definition."
            };
        }

        protected void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            if ((NodeToAnalyze as MethodDeclarationSyntax).GetName().StartsWith("Validate") == false) return;

            //checking the inheritance from Entity
            var classNode = NodeToAnalyze.GetSingleAncestor<ClassDeclarationSyntax>() as ClassDeclarationSyntax;
            if (classNode == null) return;

            var symbolClass = context.SemanticModel.GetDeclaredSymbol(classNode);
            if (symbolClass == null) return;

            //var baseClass = symbolClass.BaseType;
            //var inherit = symbolClass.IsInherited<MSharp.Framework.IEntity>();
            //if (inherit == false) return;

            var methodbody = (NodeToAnalyze as MethodDeclarationSyntax).Body;
            if (methodbody == null) return;

            //finding the code like below item
            //if (NumericProperty < VALUE) throw new ValidationException(...) - NumericProperty is not calculated
            var ifChilds = methodbody.DescendantNodes().OfKind(SyntaxKind.IfStatement);
            if (ifChilds.None()) return;

            // for this situation: if(NumericProperty < VALUE)
            var selectedIfs = ifChilds.Where(x => x.ChildNodes().OfKind(IncludeConditions).Any());
            if (selectedIfs.None()) return;

            // Checking the Validation Exception
            var thoseHaveThrow = selectedIfs.Where(x => (x as IfStatementSyntax).DescendantNodes().OfType<ThrowStatementSyntax>().Any());

            // cheking the numeric property in  if( x< y)
            Report(context, thoseHaveThrow);
        }

        private void Report(SyntaxNodeAnalysisContext context, IEnumerable<SyntaxNode> thoseHaveThrow)
        {
            foreach (var item in thoseHaveThrow)
            {
                // nested if for AMP @17994
                var nestedIf = (item as IfStatementSyntax).GetSingleAncestor<IfStatementSyntax>();
                if (nestedIf != null) continue;

                var condition = (item as IfStatementSyntax).Condition as BinaryExpressionSyntax;
                if (condition == null) continue;

                var countOfMemberAccess = condition.Left.Kind() == SyntaxKind.SimpleMemberAccessExpression ? condition.Left.ChildNodes().OfType<ThisExpressionSyntax>().Any() ? 0 : 1 : 0;
                countOfMemberAccess += condition.Left.DescendantNodesAndSelf().OfType<MemberAccessExpressionSyntax>().Count();
                if (countOfMemberAccess > 1) return;

                var left = condition.Left?.GetIdentifierSyntax();
                var right = condition.Right?.GetIdentifierSyntax();

                IPropertySymbol mySymbol = null;

                if (left != null)
                {
                    mySymbol = context.SemanticModel.GetSymbolInfo(left).Symbol as IPropertySymbol;
                }
                if (right != null)
                {
                    mySymbol = context.SemanticModel.GetSymbolInfo(right).Symbol as IPropertySymbol;
                }

                if (mySymbol == null) continue;
                if (mySymbol.Type == null) continue;
                if (Extensions.IsNumeric(mySymbol.Type.Name))
                {
                    ReportDiagnostic(context, condition);
                }
            }
        }
    }
}
