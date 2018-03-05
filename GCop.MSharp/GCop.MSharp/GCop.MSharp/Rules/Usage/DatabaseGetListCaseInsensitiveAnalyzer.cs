namespace GCop.MSharp.Rules.Usage
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
    public class DatabaseGetListCaseInsensitiveAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        string[] CheckingStrFunctions =
            new string[] {
                  "StringComparison.OrdinalIgnoreCase"
                , "StringComparison.InvariantCultureIgnoreCase"
                , "StringComparison.CurrentCultureIgnoreCase"
                , ".ToUpper().Equals"
                , ".ToUpper() =="
                , ".ToLower().Equals"
                , ".ToLower() =="
                , ".Equals" };


        protected override SyntaxKind Kind => SyntaxKind.InvocationExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "523",
                Category = Category.Usage,
                Severity = DiagnosticSeverity.Warning,
                Message = "Database search is case insensitive. Simply use {0}.{1} == yourValue for a faster execution."
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var invocExpres = NodeToAnalyze as InvocationExpressionSyntax;

            if (invocExpres == null)
                return;

            if (invocExpres.ArgumentList.IsNone()) return;
            if (invocExpres.ArgumentList.Arguments == null) return;

            var memberAccessExpression = invocExpres.ChildNodes().OfType<MemberAccessExpressionSyntax>().FirstOrDefault();
            if (memberAccessExpression == null) return;

            //checking Databae + GetList
            var variableIdentifier = memberAccessExpression.ChildNodes()?.FirstOrDefault()?.GetIdentifierSyntax();
            if (variableIdentifier == null) return;
            if (variableIdentifier.Identifier == null) return;
            if (variableIdentifier.Identifier.ValueText != "Database") return;

            var variableInfo = context.SemanticModel.GetSymbolInfo(variableIdentifier).Symbol;
            if (variableInfo == null) return;
            if (variableInfo.ContainingNamespace == null) return;
            if (variableInfo.ContainingNamespace.ToString().Lacks("MSharp")) return;

            //GetList<T> checking
            var getList = memberAccessExpression.ChildNodes().LastOrDefault();
            if (getList == null) return;
            if (getList.Kind() != SyntaxKind.GenericName) return;
            if (getList.ToString().Lacks("GetList")) return;

            // Parsing Lambda experssion to Finding  str.Equal / str.ToLower / Str Comparison
            var arguments = invocExpres.ArgumentList;
            var firstArg = arguments.ChildNodes().FirstOrDefault() as ArgumentSyntax;
            if (firstArg == null) return;
            var lambdaNode = firstArg.Expression as SimpleLambdaExpressionSyntax;
            if (lambdaNode == null) return;

            var lambdaString = lambdaNode.ToString();

            //it => it.MyStringProperty.Equals(someValue, StringComparison.OrdinalIgnoreCase)
            if (!lambdaString.ContainsAny(CheckingStrFunctions)) return;

            var lambdaInvocationExperssions = lambdaNode.ChildNodes().OfType<InvocationExpressionSyntax>().ToList();

            if (lambdaInvocationExperssions.None())
            { // binary experssion like str.MyStringProperty.ToLower()
                var binaryEqual = lambdaNode.ChildNodes().OfType<BinaryExpressionSyntax>().FirstOrDefault();
                if (binaryEqual != null)
                    lambdaInvocationExperssions = binaryEqual.ChildNodes().OfType<InvocationExpressionSyntax>().ToList();
            }

            foreach (var lambdaInvoc in lambdaInvocationExperssions)
            {
                if (lambdaInvoc == null) continue;

                var internalMemberAccess = lambdaInvoc.DescendantNodes().OfType<MemberAccessExpressionSyntax>().ToList();
                foreach (var member in internalMemberAccess)
                {
                    if (member == null) continue;
                    var methodName = member.ChildNodes().LastOrDefault()?.GetIdentifierSyntax();
                    if (methodName == null) continue;
                    if (methodName.Identifier == null) continue;
                    if (!(methodName.Identifier.ValueText.IsAnyOf("Equals", "ToLower", "ToUpper"))) continue;
                    var symbol = context.SemanticModel.GetSymbolInfo(methodName).Symbol;

                    // find propety
                    var innerMemberAccess = member.ChildNodes().OfType<MemberAccessExpressionSyntax>()?.FirstOrDefault();
                    if (innerMemberAccess == null) continue;

                    var property = innerMemberAccess.ChildNodes().OfType<IdentifierNameSyntax>().LastOrDefault();
                    if (property == null) continue;
                    if (property.GetIdentifierSyntax() == null) continue;
                    var propertyInfo = context.SemanticModel.GetSymbolInfo(property.GetIdentifierSyntax()).Symbol as IPropertySymbol;
                    if (propertyInfo == null) continue;
                    if (propertyInfo.Type == null) continue;
                    if (propertyInfo.Type.ToString().ToLower().Lacks("string")) continue;
                    ReportDiagnostic(context, methodName, lambdaNode.Parameter.Identifier.ValueText, property.GetIdentifier());
                }
            }
        }
    }
}