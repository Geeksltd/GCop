namespace GCop.ErrorHandling.Rules.Design
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ThrowExceptionAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        private readonly string[] ExcludedExceptionTypes = new string[] { "NotSupportedException", "NotImplementedException", "InvalidOperationException", "NullReferenceException " };
        protected override SyntaxKind Kind => SyntaxKind.ThrowStatement;

        private ThrowStatementSyntax Throw;
        private ObjectCreationExpressionSyntax ObjectCreation;
        protected bool IsExceptionThrownByInstantiation => Throw.ChildNodes().OfType<ObjectCreationExpressionSyntax>().Any();
        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "301",
                Category = Category.Performance,
                Message = "Do not throw exceptions using default constructor or with empty message",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            Throw = context.Node as ThrowStatementSyntax;

            if (!IsExceptionThrownByInstantiation) return;

            ObjectCreation = GetExceptionCreationInstance();

            if (IsInExcludeList()) return;

            var arguments = ObjectCreation?.ArgumentList?.Arguments.OrEmpty();

            if (arguments.None())
                ReportDiagnostic(context, ObjectCreation);

            //var parameterWithStringType = arguments.Where( arg => arg.ChildNodes().OfType<LiteralExpressionSyntax>().Any() );

            //parameterWithStringType.ForEach( it =>
            //{
            //    var parameter = it.ChildNodes().OfType<LiteralExpressionSyntax>().FirstOrDefault();
            //    if( parameter.IsKind( SyntaxKind.StringLiteralExpression ) )
            //    {
            //        if( parameter.Token.ValueText.IsEmpty() )
            //        {
            //            ReportDiagnostic( context, parameter );
            //        }
            //    }
            //} );
        }

        private ObjectCreationExpressionSyntax GetExceptionCreationInstance() => Throw.ChildNodes().OfType<ObjectCreationExpressionSyntax>().First();

        private bool IsInExcludeList() => ExcludedExceptionTypes.Contains(ObjectCreation.Type.ToString());
    }
}
