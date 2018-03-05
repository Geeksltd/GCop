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
    public class UseIHierarchyOrISortableInMSharpAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        private readonly string FolderName = "Logic";
        private readonly string[] Interfaces = new string[] { "IHierarchy", "ISortable" };
        protected override SyntaxKind Kind => SyntaxKind.ClassDeclaration;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "128",
                Category = Category.Design,
                Message = "{0} interface should be set in M# on the entity definition",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var @class = context.Node as ClassDeclarationSyntax;

            if (@class == null || @class.SyntaxTree.FilePath.Lacks(FolderName)) return;

            @class.BaseList?.Types.OfType<SimpleBaseTypeSyntax>().ToList().ForEach(@base =>
           {
               var baseType = (@base.Type as IdentifierNameSyntax)?.Identifier;
               if (baseType.HasValue && Interfaces.Contains(baseType.Value.ValueText))
               {
                   context.ReportDiagnostic(Diagnostic.Create(Description, baseType.Value.GetLocation(), baseType.Value.ValueText));
               }
           });
        }
    }
}
