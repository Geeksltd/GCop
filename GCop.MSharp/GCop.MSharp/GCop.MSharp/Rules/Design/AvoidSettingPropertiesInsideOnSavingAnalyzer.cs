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
    public class AvoidSettingPropertiesInsideOnSavingAnalyzer : GCopAnalyzer
    {
        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "167",
                Category = Category.Design,
                Message = "{0}",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Configure() => RegisterCodeBlockAction(context => Analyze(context));

        void Analyze(CodeBlockAnalysisContext context)
        {
            var method = context.CodeBlock as MethodDeclarationSyntax;
            if (method == null) return;
            NodeToAnalyze = method;

            var methodInfo = context.SemanticModel.GetDeclaredSymbol(method) as IMethodSymbol;
            if (methodInfo == null || methodInfo.Name.IsNoneOf("OnSaving", "Validate")) return;

            var containingTypeName = methodInfo.ContainingType.ToString();

            method.Body.DescendantNodes().OfType<AssignmentExpressionSyntax>().ForEach(assignment =>
           {
               if (assignment.Left is IdentifierNameSyntax property)
               {
                   var propertyInfo = context.SemanticModel.GetSymbolInfo(property).Symbol as IPropertySymbol;
                   if (propertyInfo?.ContainingType.ToString() == containingTypeName)
                   {
                       context.ReportDiagnostic(Diagnostic.Create(Description, assignment.Left.GetLocation(), GetMessage(methodInfo.Name)));
                   }
               }
               else
               {
                   property = (assignment.Left as MemberAccessExpressionSyntax)?.GetIdentifierSyntax();
                   if (property != null)
                   {
                       var propertyInfo = context.SemanticModel.GetSymbolInfo(property).Symbol as IPropertySymbol;
                       if (propertyInfo?.ContainingType.ToString() == containingTypeName)
                       {
                           context.ReportDiagnostic(Diagnostic.Create(Description, assignment.Left.GetLocation(), GetMessage(methodInfo.Name)));
                       }
                   }
               }
           });
        }

        string GetMessage(string methodName)
        {
            if (methodName == "OnSaving")
                return "OnSaving() is called after Validate() and property values set in OnSaving() won't be validated. Instead use OnValidating() to write your prep logic.";

            else if (methodName == "Validate")
            {
                return "No properties should be set in Validate(). Instead use OnValidating() to write your assignment.";
            }

            return "";
        }
    }
}
