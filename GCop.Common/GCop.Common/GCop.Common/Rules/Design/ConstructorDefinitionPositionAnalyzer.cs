namespace GCop.Common.Rules.Design
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer( LanguageNames.CSharp )]
    public class ConstructorDefinitionPositionAnalyzer:GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.ConstructorDeclaration;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "148",
                Category = Category.Design,
                Severity = DiagnosticSeverity.Warning,
                Message = "All constructors should be before all methods in a class."
            };
        }

        protected override void Analyze( SyntaxNodeAnalysisContext context )
        {
            var constructor = context.Node as ConstructorDeclarationSyntax;
            if( constructor == null ) return;
            NodeToAnalyze = constructor;

            var @class = constructor.GetParent( typeof( ClassDeclarationSyntax ) );
            if( @class == null ) return;

            var allChildNodes = @class.ChildNodes();

            var constructorIndex = allChildNodes.IndexOf( constructor );

            var allMethods = allChildNodes.OfType<MethodDeclarationSyntax>().Select( ( node, index ) => new MethodDefinition
            {
                Index = allChildNodes.IndexOf( node ),
                Name = node.Identifier.ValueText,
                Location = node.Identifier.GetLocation()
            } ).ToList();

            if( !allMethods.TrueForAll( it => it.Index > constructorIndex ) )
            {
                ReportDiagnostic( context, constructor.Identifier );
            }
        }
    }
}
