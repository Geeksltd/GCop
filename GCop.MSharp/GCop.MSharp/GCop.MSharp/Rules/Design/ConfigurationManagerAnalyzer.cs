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
    public class ConfigurationManagerAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.SimpleMemberAccessExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "164",
                Category = Category.Design,
                Message = "Instead use {0}",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            var invocation = context.Node as MemberAccessExpressionSyntax;
            if (invocation == null) return;

            NodeToAnalyze = invocation;

            var listOfIdentifiers = invocation.ChildNodes().OfType<IdentifierNameSyntax>().ToList();
            if (!listOfIdentifiers.HasMany()) return;

            if (listOfIdentifiers[0].ToString() == "ConfigurationManager" && listOfIdentifiers[1].ToString() == "ConnectionStrings")
            {
                ReportDiagnostic(context, invocation, "Config.GetConnectionString() ");
            }
            else if (listOfIdentifiers[0].ToString() == "ConfigurationManager" && listOfIdentifiers[1].ToString() == "AppSettings")
            {
                ReportDiagnostic(context, invocation, "Config.Get() ");
            }
        }
    }
}
