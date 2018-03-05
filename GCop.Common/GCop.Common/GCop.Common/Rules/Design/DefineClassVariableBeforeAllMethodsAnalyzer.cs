namespace GCop.Common.Rules.Design
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Collections.Generic;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DefineClassVariableBeforeAllMethodsAnalyzer : GCopAnalyzer
    {
        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "136",
                Category = Category.Design,
                Severity = DiagnosticSeverity.Warning,
                Message = "All constants and class fields should be defined at the top of the class, before all methods."
            };
        }

        protected override void Configure()
        {
            RegisterSyntaxNodeAction(Analyze, SyntaxKind.ClassDeclaration);
        }

        private void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var @class = (ClassDeclarationSyntax)context.Node;

            var allChildNodes = @class.ChildNodes().ToList();

            var allMethods = allChildNodes.OfType<MethodDeclarationSyntax>().Select((node, index) => new MethodDefinition
            {
                Index = allChildNodes.IndexOf(node),
                Name = node.Identifier.ValueText,
                Location = node.Identifier.GetLocation()
            }).ToList();

            var allVariables = new List<VariableDefinition>();

            allChildNodes.OfType<FieldDeclarationSyntax>().Select(node => node.Declaration.Variables).ForEach(node =>
            {
                var declarator = node.FirstOrDefault();

                if (declarator != null && !declarator.Identifier.ValueText.StartsWith("_"))
                    allVariables.Add(new VariableDefinition
                    {
                        Index = allChildNodes.IndexOf(declarator.Parent.Parent),
                        Name = declarator.Identifier.ValueText,
                        Location = declarator.Identifier.GetLocation()
                    });
            });

            allVariables.ForEach(variable =>
           {
               if (!allMethods.All(it => it.Index > variable.Index))
               {
                   ReportDiagnostic(context, variable.Location);
               }
           });
        }
    }
}
