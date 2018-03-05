namespace GCop.Conditional.Rules.Refactoring
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UnnecessaryIfListAnyAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.IfStatement;
        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "652",
                Category = Category.Refactoring,
                Severity = DiagnosticSeverity.Warning,
                Message = "if ({0}.Any()) is unnecessary when using foreach."
            };
        }

        /// <summary>
        ///if(x.Any()) { 
        ///foreach (var item in x) 
        ///{ 
        /// do something 
        ///} 
        ///}
        /// </summary>       
        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var condition = (NodeToAnalyze as IfStatementSyntax).Condition;

            // skip if there is an else after it
            if ((NodeToAnalyze as IfStatementSyntax).Else != null) return;

            // looking for .Any()
            var invoc = condition as InvocationExpressionSyntax;
            if (invoc == null) return;
            if (invoc.ArgumentList.Arguments.Any()) return;

            var memberAccesses = invoc.ChildNodes().OfType<MemberAccessExpressionSyntax>();
            if (memberAccesses.None()) return;
            if (memberAccesses.HasMany()) return;

            var member = memberAccesses.FirstOrDefault();
            if (member == null) return;

            var allIdentifiresInIf = member.ChildNodes().OfKind(SyntaxKind.IdentifierName);

            if (allIdentifiresInIf.LastOrDefault()?.GetIdentifier() != "Any") return;

            ForEachStatementSyntax foreachStatment;
            // checking the block code of if to chec block has only one foreach statment
            var block = (NodeToAnalyze as IfStatementSyntax).ChildNodes().OfKind(SyntaxKind.Block).FirstOrDefault();
            if (block == null)
            {
                //Chekcing if statement without block {}=> if(list.Any()) foreach(var item in list){}
                foreachStatment = (NodeToAnalyze as IfStatementSyntax).ChildNodes().OfKind(SyntaxKind.ForEachStatement).FirstOrDefault() as ForEachStatementSyntax;
            }
            else
            {
                if (block.ChildNodes().HasMany()) return;
                foreachStatment = block.ChildNodes().OfKind(SyntaxKind.ForEachStatement).FirstOrDefault() as ForEachStatementSyntax;
            }

            if (foreachStatment == null) return;

            var variable = foreachStatment.ChildNodes().OfKind(SyntaxKind.IdentifierName).LastOrDefault();
            if (variable == null) return;

            if (allIdentifiresInIf.FirstOrDefault() == null) return;
            if (allIdentifiresInIf.FirstOrDefault().GetIdentifier() == null) return;
            if (variable.GetIdentifier() == null) return;

            if (variable.GetIdentifier() != member.ChildNodes().OfKind(SyntaxKind.IdentifierName).First().GetIdentifier()) return;
            ReportDiagnostic(context, condition, variable.GetIdentifier());
        }
    }
}
