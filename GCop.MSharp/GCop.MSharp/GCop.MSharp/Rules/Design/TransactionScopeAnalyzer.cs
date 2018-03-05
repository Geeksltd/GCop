namespace GCop.MSharp.Rules.Design
{
    using Core;
    using Core.Attributes;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [MSharpExclusive]
    [ZebbleExclusive]
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class TransactionScopeAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.ObjectCreationExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "122",
                Category = Category.Design,
                Message = "Use Database.CreateTransactionScope() instead of {0}",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var creation = context.Node as ObjectCreationExpressionSyntax;

            var transactionType = context.SemanticModel.Compilation.GetTypeByMetadataName("System.Transactions.TransactionScope");
            var type = context.SemanticModel.GetTypeInfo(creation, context.CancellationToken).Type;
            if (type?.Equals(transactionType) == true)
            {
                var diagnostic = Diagnostic.Create(Description, creation.GetLocation(), creation.Type);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
