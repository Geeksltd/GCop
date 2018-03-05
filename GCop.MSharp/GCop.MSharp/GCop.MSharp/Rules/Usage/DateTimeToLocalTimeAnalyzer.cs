namespace GCop.MSharp.Rules.Usage
{
    using Core;
    using Core.Attributes;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System;
    using System.Linq;

    [MSharpExclusive]
    [ZebbleExclusive]
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DateTimeToLocalTimeAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.SimpleMemberAccessExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "535",
                Category = Category.Usage,
                Severity = DiagnosticSeverity.Warning,
                Message = "{0}"
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;

            var memberAccess = NodeToAnalyze as MemberAccessExpressionSyntax;

            var identifiers = memberAccess.ChildNodes().OfKind(SyntaxKind.IdentifierName);
            if (identifiers.None()) return;

            if (identifiers.FirstOrDefault() == null) return;

            var lastIdentifier = identifiers.LastOrDefault() as IdentifierNameSyntax;
            if (lastIdentifier == null) return;
            if (lastIdentifier.Identifier.ValueText.IsNoneOf("ToLocalTime", "ToUniversalTime")) return;

            var type = context.SemanticModel.GetSymbolInfo(identifiers.FirstOrDefault()).Symbol;

            if (type.Is<DateTime>() == false) return;

            if (lastIdentifier.Identifier.ValueText == "ToLocalTime")

                ReportDiagnostic(context, identifiers.LastOrDefault(), "Use ToLocal() method instead, so you get control over it using via LocalTime.CurrentTimeZone.");

            else if (lastIdentifier.Identifier.ValueText == "ToUniversalTime")
                ReportDiagnostic(context, identifiers.LastOrDefault(), "Use ToUniversal() method instead, so you get control over it using via LocalTime.CurrentTimeZone.");
        }
    }
}


