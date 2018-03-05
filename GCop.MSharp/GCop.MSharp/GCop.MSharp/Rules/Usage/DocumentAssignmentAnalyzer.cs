namespace GCop.MSharp.Rules.Usage
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DocumentAssignmentAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.SimpleAssignmentExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "503",
                Category = Category.Usage,
                Severity = DiagnosticSeverity.Warning,
                Message = "Don't set one document to another without calling clone."
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;

            var assignment = context.Node as AssignmentExpressionSyntax;
            if (assignment.Left == null || assignment.Right == null) return;

            var leftSymbol = context.SemanticModel.GetSymbolInfo(assignment.Left).Symbol;
            var rightSymbol = context.SemanticModel.GetSymbolInfo(assignment.Right).Symbol;

            if (leftSymbol.Is<Document>() == false
                ||
                rightSymbol.Is<Document>() == false
                ) return;

            if ((assignment.Right.As<InvocationExpressionSyntax>()?.Expression as MemberAccessExpressionSyntax)?.Name?.ToString() == "Clone") return;

            var method = NodeToAnalyze.GetSingleAncestor<MethodDeclarationSyntax>();
            if (IsClonedInMethodBody(method, assignment.Right)) return;

            ReportDiagnostic(context, assignment.Right);
        }

        bool IsClonedInMethodBody(MethodDeclarationSyntax method, ExpressionSyntax right)
        {
            /*following sitation should be ignored  
            task @18455
            var file = MyFile.Clone(); 
            applicant = Database.Update(applicant, x => x.MyFile = file);*/

            var assigments = method.DescendantNodesAndSelf().OfKind(SyntaxKind.SimpleAssignmentExpression)
                .Where(x => x.GetIdentifier() == right.GetIdentifier());

            var declarations = method.DescendantNodesAndSelf().OfType<VariableDeclaratorSyntax>().Where(x => x.Identifier != null && x.Identifier.ValueText == right.GetIdentifier());

            if (assigments.None() && declarations.None()) return false;

            var result = false;
            assigments.ForEach(x =>
            {
                var expression = x.ChildNodes().OfType<InvocationExpressionSyntax>()?.FirstOrDefault()?.Expression;
                if ((expression as MemberAccessExpressionSyntax)?.Name?.ToString() == "Clone")
                    result = true;
            });
            declarations.ForEach(x =>
            {
                var expression = x.DescendantNodes().OfType<InvocationExpressionSyntax>()?.FirstOrDefault()?.Expression;
                if ((expression as MemberAccessExpressionSyntax)?.Name?.ToString() == "Clone")
                    result = true;
            });

            return result;
        }

    }
}
