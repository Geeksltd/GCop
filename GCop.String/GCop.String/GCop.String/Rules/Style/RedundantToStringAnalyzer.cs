namespace GCop.String.Rules.Style
{
    using Core;
    using Core.Syntax;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;
    using System;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RedundantToStringAnalyzer : GCopAnalyzer
    {
        private const string AdditionOperatorName = "op_Addition";
        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "414",
                Category = Category.Style,
                Severity = DiagnosticSeverity.Warning,
                Message = "Remove .ToString() as it's unnecessary."
            };
        }

        protected override void Configure()
        {
            RegisterSyntaxNodeAction(CheckSidesOfAddExpressionsForToStringCall, SyntaxKind.AddExpression);
            RegisterSyntaxNodeAction(CheckRightSideOfAddAssignmentsForToStringCall, SyntaxKind.AddAssignmentExpression);
            RegisterSyntaxNodeAction(CheckToStringInvocationsOnStringAndInStringFormat, SyntaxKind.InvocationExpression);
        }

        private void CheckRightSideOfAddAssignmentsForToStringCall(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var assignment = (AssignmentExpressionSyntax)context.Node;
            var operation = context.SemanticModel.GetSymbolInfo(assignment).Symbol as IMethodSymbol;
            if (!IsOperationAddOnString(operation))
            {
                return;
            }

            CheckRightExpressionForRemovableToStringCall(context, assignment);
        }

        private void CheckSidesOfAddExpressionsForToStringCall(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var binary = (BinaryExpressionSyntax)context.Node;
            var operation = context.SemanticModel.GetSymbolInfo(binary).Symbol as IMethodSymbol;
            if (!IsOperationAddOnString(operation))
            {
                return;
            }

            CheckLeftExpressionForRemovableToStringCall(context, binary);
            CheckRightExpressionForRemovableToStringCall(context, binary);
        }

        private void CheckToStringInvocationsOnStringAndInStringFormat(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;
            var argumentless = IsArgumentlessToStringCallNotOnBaseExpression(invocation, context.SemanticModel);
            if (!argumentless.ReturnValue)
            {
                return;
            }

            if (argumentless.MethodSymbol.ContainingType.SpecialType == SpecialType.System_String)
            {
                Report(context, argumentless);
                return;
            }

            if (invocation?.Parent?.IsKind(SyntaxKind.InterpolatedStringExpression, SyntaxKind.Interpolation) == true)
            {
                Report(context, argumentless);
                return;
            }

            var stringFormatArgument = invocation?.Parent as ArgumentSyntax;
            if (stringFormatArgument?.Parent?.Parent is InvocationExpressionSyntax stringFormatInvocation)
            {
                var stringFormatSymbol = context.SemanticModel.GetSymbolInfo(stringFormatInvocation).Symbol as IMethodSymbol;

                if (IsStringFormatCall(stringFormatSymbol))
                {
                    var parameterLookup = new MethodParameterLookup(stringFormatInvocation, context.SemanticModel);
                    var argParameter = parameterLookup.GetParameterSymbol(stringFormatArgument);
                    if (argParameter != null && argParameter.Name.StartsWith("arg", StringComparison.Ordinal))
                    {
                        Report(context, argumentless);
                        return;
                    }
                }
            }
        }

        private void Report(SyntaxNodeAnalysisContext context, Argumentless argumentless)
        {
            ReportDiagnostic(context, argumentless.Location);
        }

        private void CheckLeftExpressionForRemovableToStringCall(SyntaxNodeAnalysisContext context, BinaryExpressionSyntax binary) => CheckExpressionForRemovableToStringCall(context, binary.Left, binary.Right, 0);

        private void CheckRightExpressionForRemovableToStringCall(SyntaxNodeAnalysisContext context, BinaryExpressionSyntax binary) => CheckExpressionForRemovableToStringCall(context, binary.Right, binary.Left, 1);

        private void CheckRightExpressionForRemovableToStringCall(SyntaxNodeAnalysisContext context,
            AssignmentExpressionSyntax assignment) => CheckExpressionForRemovableToStringCall(context, assignment.Right, assignment.Left, 1);


        private void CheckExpressionForRemovableToStringCall(SyntaxNodeAnalysisContext context,
            ExpressionSyntax expressionWithToStringCall, ExpressionSyntax otherOperandOfAddition, int checkedSideIndex)
        {
            var argumentless = IsArgumentlessToStringCallNotOnBaseExpression(expressionWithToStringCall, context.SemanticModel);
            if (!argumentless.ReturnValue || argumentless.MethodSymbol.ContainingType.SpecialType == SpecialType.System_String)
                return;

            var sideBType = context.SemanticModel.GetTypeInfo(otherOperandOfAddition).Type;
            if (sideBType == null ||
                sideBType.SpecialType != SpecialType.System_String)
            {
                return;
            }

            var subExpression = (((InvocationExpressionSyntax)expressionWithToStringCall).Expression as MemberAccessExpressionSyntax)?.Expression;
            if (subExpression == null)
            {
                return;
            }

            var subExpressionType = context.SemanticModel.GetTypeInfo(subExpression).Type;
            if (subExpressionType == null)
            {
                return;
            }

            var stringParameterIndex = (checkedSideIndex + 1) % 2;
            if (!DoesCollidingAdditionExist(subExpressionType, stringParameterIndex))
            {
                ReportDiagnostic(context, argumentless.Location);
            }
        }

        private bool DoesCollidingAdditionExist(ITypeSymbol subExpressionType, int stringParameterIndex)
        {
            return subExpressionType.GetMembers(AdditionOperatorName)
                .OfType<IMethodSymbol>()
                .Where(method =>
                    method.MethodKind == MethodKind.BuiltinOperator ||
                    method.MethodKind == MethodKind.UserDefinedOperator)
                .Any(method =>
                    method.Parameters.Length == 2 &&
                    method.Parameters[stringParameterIndex].Type.SpecialType == SpecialType.System_String);
        }

        private bool IsStringFormatCall(IMethodSymbol stringFormatSymbol)
        {
            return stringFormatSymbol != null &&
                stringFormatSymbol.Name.IsAnyOf("Format", "FormatWith") &&
                (stringFormatSymbol.ContainingType == null || stringFormatSymbol.ContainingType?.ToString() == "System.MSharpExtensions" ||
                stringFormatSymbol.ContainingType.SpecialType == SpecialType.System_String);
        }

        private bool IsOperationAddOnString(IMethodSymbol operation)
        {
            return operation != null &&
                operation.Name == AdditionOperatorName &&
                operation.ContainingType != null &&
                operation.ContainingType.SpecialType == SpecialType.System_String;
        }

        private Argumentless IsArgumentlessToStringCallNotOnBaseExpression(ExpressionSyntax expression, SemanticModel semanticModel)
        {
            var argumentless = new Argumentless
            {
                Location = null,
                MethodSymbol = null,
                ReturnValue = true
            };
            var invocation = expression as InvocationExpressionSyntax;
            if (invocation == null ||
                invocation.ArgumentList.CloseParenToken.IsMissing)
            {
                argumentless.ReturnValue = false;
            }

            var memberAccess = invocation.Expression as MemberAccessExpressionSyntax;
            if (memberAccess == null ||
                memberAccess.Expression is BaseExpressionSyntax)
            {
                argumentless.ReturnValue = false;
            }

            argumentless.MethodSymbol = semanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
            if (!IsParameterlessToString(argumentless.MethodSymbol))
            {
                argumentless.ReturnValue = false;
            }

            argumentless.Location = Location.Create(invocation.SyntaxTree,
                TextSpan.FromBounds(
                    memberAccess.OperatorToken.SpanStart,
                    invocation.Span.End));
            return argumentless;
        }

        private bool IsParameterlessToString(IMethodSymbol methodSymbol)
        {
            return methodSymbol != null &&
                methodSymbol.Name == "ToString" &&
                methodSymbol.Parameters.None();
        }
    }

    class Argumentless
    {
        public bool ReturnValue { get; set; }
        public Location Location { get; set; }
        public IMethodSymbol MethodSymbol { get; set; }
    }
}
