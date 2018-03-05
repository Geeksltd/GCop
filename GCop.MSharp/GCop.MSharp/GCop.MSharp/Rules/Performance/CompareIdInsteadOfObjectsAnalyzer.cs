namespace GCop.MSharp.Rules.Performance
{
    using Core;
    using Core.Attributes;
    using Core.Syntax;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Collections.Generic;
    using System.Linq;

    [MSharpExclusive]
    [ZebbleExclusive]
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CompareIdInsteadOfObjectsAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        string Criterion = "MSharp.Framework.Data.Criterion";
        private string Type = "MSharp.Framework.Database";
        protected override SyntaxKind Kind => SyntaxKind.InvocationExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "309",
                Category = Category.Performance,
                Message = "It should be written as {0}, Because the criteria will be faster and eliminate unnecessary database fetches",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var invocation = context.Node as InvocationExpressionSyntax;
            if (invocation == null) return;

            var method = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
            var receiverType = method?.ReceiverType?.ToString();
            if (receiverType.IsEmpty()) return;

            if (receiverType.IsAnyOf(Type, Criterion)) return;

            invocation.ArgumentList.Arguments.Where(it => it.Expression.IsKind(SyntaxKind.SimpleLambdaExpression)).ForEach(it =>
            {
                var lambda = it.Expression as SimpleLambdaExpressionSyntax;
                if (lambda == null || lambda.Body == null) return;

                if (lambda.Body.IsKind(SyntaxKind.EqualsExpression))
                {
                    var validation = Validation(context.SemanticModel, lambda.Body as BinaryExpressionSyntax);
                    if (!validation.IsValid)
                        ReportDiagnostic(context, lambda.Body, validation.Errors.First().Message);
                }
                else
                {
                    if (!(lambda.Body.IsKind(SyntaxKind.SimpleMemberAccessExpression) && ItIsNotDatabaseCall(context.SemanticModel, lambda.Body)))
                    {
                        lambda.Body.DescendantNodes().OfType<BinaryExpressionSyntax>().ForEach(equalsClause =>
                        {
                            //@16501
                            //Check for possbile casting and igonre it
                            if (equalsClause.OperatorToken.IsKind(SyntaxKind.AsKeyword)) return;
                            if (CheckForCasting(equalsClause.Left)) return;
                            if (CheckForCasting(equalsClause.Right)) return;
                            //
                            if (!equalsClause.IsKind(SyntaxKind.IsExpression))
                            {
                                var validation = Validation(context.SemanticModel, equalsClause);
                                if (!validation.IsValid)
                                {
                                    ReportDiagnostic(context, validation.Errors.First().ErrorLocation, validation.Errors.First().Message);
                                }
                            }
                        });
                    }
                }
            });
        }

        private bool CheckForCasting(ExpressionSyntax input)
        {
            if (input is MemberAccessExpressionSyntax equalMemberAccess)
            {
                if (equalMemberAccess.Expression is ParenthesizedExpressionSyntax parenthesized)
                {
                    if (parenthesized.Expression is BinaryExpressionSyntax binaryExpression && binaryExpression.OperatorToken.IsKind(SyntaxKind.AsKeyword))
                        return true;
                }
            }
            return false;
        }

        private ValidationResult Validation(SemanticModel semanticModel, BinaryExpressionSyntax equalsClause)
        {
            ExpressionSyntax anotherHand = null;
            ExpressionSyntax originObject = null;

            if (equalsClause.Right.IsKind(SyntaxKind.ThisExpression) || equalsClause.Right.IsKind(SyntaxKind.IdentifierName) || equalsClause.Right.IsKind(SyntaxKind.SimpleMemberAccessExpression))
            {
                originObject = equalsClause.Right;
                anotherHand = equalsClause.Left;
            }
            else if (equalsClause.Left.IsKind(SyntaxKind.ThisExpression) || equalsClause.Left.IsKind(SyntaxKind.IdentifierName))
            {
                originObject = equalsClause.Left;
                anotherHand = equalsClause.Right;
            }

            var invocation = anotherHand as MemberAccessExpressionSyntax;
            if (invocation == null) return ValidationResult.Ok;

            var parent = invocation.GetParent<InvocationExpressionSyntax>();
            if (parent != null)
            {
                if (!ItIsNotDatabaseCall(semanticModel, parent)) return ValidationResult.Ok;
            }

            var identifiers = invocation.DescendantNodes().OfType<IdentifierNameSyntax>().Select((it, index) => new ParameterDefinition { Index = index, Name = it.ToString(), Identifier = it }).ToList();

            if (identifiers.None()) return ValidationResult.Ok;

            string rightHandIdentifier = null;
            var lastItem = identifiers.LastOrDefault();

            if (lastItem?.Index > 0)
            {
                if (HasSmallTableAttribute(semanticModel, lastItem.Identifier)) return ValidationResult.Ok;

                string message = null;
                var lastItemContainer = identifiers[lastItem?.Index - 1 ?? 0];
                IdentifierNameSyntax originIdentifier = null;

                if (equalsClause.Right.IsKind(SyntaxKind.SimpleMemberAccessExpression))
                {
                    originIdentifier = originObject.ChildNodes().OfType<IdentifierNameSyntax>().LastOrDefault();
                    rightHandIdentifier = originIdentifier?.ToString() + "Id";
                }
                else
                {
                    originIdentifier = originObject.GetIdentifierSyntax();
                    if (originIdentifier == null && originObject.IsKind(SyntaxKind.ThisExpression))
                    {
                        rightHandIdentifier = "this.ID";
                    }
                    else
                        rightHandIdentifier = originIdentifier != null ? originIdentifier.ToString().ToPascalCaseId() + "Id" : "ID";
                }

                if (SymbolIsIEntity(semanticModel, lastItem.Identifier))
                {
                    if (SymbolHasId(semanticModel, lastItemContainer.Identifier, lastItem.Name + "Id"))
                    {
                        return CheckSymbol(semanticModel, equalsClause, originObject, rightHandIdentifier, lastItem, originIdentifier);
                    }
                    else
                    {
                        return CheckValidation(semanticModel, equalsClause, lastItem, originIdentifier);
                    }
                }
                else if (SymbolIsIEntity(semanticModel, originIdentifier) && SymbolHasId(semanticModel, originIdentifier, lastItem.Name + "Id"))
                {
                    message = equalsClause.ToString().ReplaceWholeWord(originIdentifier.ToString(), string.Format("{0}.{1}Id", originIdentifier, lastItem.Name));
                    return ValidationResult.Error(new ValidationError { Message = message, ErrorLocation = lastItem.Identifier.GetLocation() });
                }
            }

            return ValidationResult.Ok;
        }

        private ValidationResult CheckValidation(SemanticModel semanticModel, BinaryExpressionSyntax equalsClause, ParameterDefinition lastItem, IdentifierNameSyntax originIdentifier)
        {
            var message = "";
            if (SymbolIsIEntity(semanticModel, originIdentifier) && SymbolHasId(semanticModel, originIdentifier, lastItem.Name + "Id"))
            {
                var newWord = string.Format("{0}.{1}Id", originIdentifier, lastItem.Name);
                message = equalsClause.ToString().ReplaceWholeWord(originIdentifier.ToString(), newWord);
                return ValidationResult.Error(new ValidationError { Message = message, ErrorLocation = lastItem.Identifier.GetLocation() });
            }
            return ValidationResult.Ok;
        }

        private ValidationResult CheckSymbol(SemanticModel semanticModel, BinaryExpressionSyntax equalsClause, ExpressionSyntax originObject, string rightHandIdentifier, ParameterDefinition lastItem, IdentifierNameSyntax originIdentifier)
        {
            var equalValue = equalsClause.ToString().Split(equalsClause.OperatorToken.ToString().ToCharArray());
            var message = "";
            if (SymbolIsIEntity(semanticModel, originIdentifier) && !SymbolHasId(semanticModel, originIdentifier, rightHandIdentifier))
            {
                var methodDeclaration = equalsClause.GetSingleAncestor<MethodDeclarationSyntax>();

                if (methodDeclaration != null)
                {
                    if (semanticModel.GetSymbolInfo(originIdentifier).Symbol is ILocalSymbol || methodDeclaration.ParameterList.Parameters.Any(p => p.Identifier.ValueText == originIdentifier.Identifier.ValueText))
                    {
                        var word = $"{originIdentifier}?.ID";
                        message = equalsClause.ToString().ReplaceWholeWord(lastItem.ToString(), lastItem.Name + "Id").ReplaceWholeWord(originIdentifier.ToString(), word);
                        return ValidationResult.Error(new ValidationError { Message = message, ErrorLocation = lastItem.Identifier.GetLocation() }); ;
                    }
                }

                var leftSide = equalValue[0];
                var rightSide = equalValue[2];
                var newWord = $"{originIdentifier}.ID (or {originIdentifier}?.ID if it can be null)";
                message = leftSide.ReplaceWholeWord(lastItem.ToString(), lastItem.Name + "Id ") + equalsClause.OperatorToken + " " + rightSide.ReplaceWholeWord(originIdentifier.ToString(), newWord);
            }
            else if (SymbolIsIEntity(semanticModel, originIdentifier) && SymbolHasId(semanticModel, originIdentifier, rightHandIdentifier))
            {
                var newWord = string.Format("{0}.{1} (or {1}?.ID if it can be null", originIdentifier, rightHandIdentifier);
                var leftSide = equalValue[0];
                var rightSide = equalValue[2];
                message = leftSide.ReplaceWholeWord(lastItem.ToString(), lastItem.Name + "Id ") + equalsClause.OperatorToken + " " + rightSide.ReplaceWholeWord(originIdentifier.ToString(), newWord);
            }
            else
                message = equalsClause.ToString().ReplaceWholeWord(lastItem.ToString(), lastItem.Name + "Id").ReplaceWholeWord(originObject.ToString(), rightHandIdentifier);

            return ValidationResult.Error(new ValidationError { Message = message, ErrorLocation = lastItem.Identifier.GetLocation() });
        }

        private bool SymbolIsIEntity(SemanticModel semanticModel, IdentifierNameSyntax identifier)
        {
            var result = false;
            var property = GetSymbolInfo(semanticModel, identifier);

            if (property is IPropertySymbol)
                result = GetInterface(property, 0);

            if (property is IParameterSymbol)
                result = GetInterface(property, 1);

            if (property is IFieldSymbol)
                result = GetInterface(property, 2);

            if (property is ILocalSymbol)
                result = GetInterface(property, 3);

            return result;
        }

        private bool GetInterface(ISymbol input, int type)
        {
            var allInterfaces = new List<INamedTypeSymbol>();
            switch (type)
            {
                case 0:
                    allInterfaces = input.As<IPropertySymbol>().Type.AllInterfaces.ToList();
                    break;
                case 1:
                    allInterfaces = input.As<IParameterSymbol>().Type.AllInterfaces.ToList();
                    break;
                case 2:
                    allInterfaces = input.As<IFieldSymbol>().Type.AllInterfaces.ToList();
                    break;
                case 3:
                    allInterfaces = input.As<ILocalSymbol>().Type.AllInterfaces.ToList();
                    break;
                default:
                    allInterfaces = null;
                    break;
            };
            return allInterfaces.Any(it => it.Name == "IEntity");
        }

        private bool SymbolHasId(SemanticModel semanticModel, IdentifierNameSyntax identifier, string fieldNameToSearch)
        {
            var result = false;
            var property = GetSymbolInfo(semanticModel, identifier);

            if (property is IPropertySymbol)
                result = property.As<IPropertySymbol>().Type.GetMembers().Any(it => it.Name.ToLower() == fieldNameToSearch.ToLower());

            if (property is IParameterSymbol)
                result = property.As<IParameterSymbol>().Type.GetMembers().Any(it => it.Name.ToLower() == fieldNameToSearch.ToLower());

            if (property is IFieldSymbol)
                result = property.As<IFieldSymbol>().Type.GetMembers().Any(it => it.Name.ToLower() == fieldNameToSearch.ToLower());

            if (property is ILocalSymbol)
                result = property.As<ILocalSymbol>().Type.GetMembers().Any(it => it.Name.ToLower() == fieldNameToSearch.ToLower());

            return result;
        }

        private ISymbol GetSymbolInfo(SemanticModel semanticModel, IdentifierNameSyntax identifier)
        {
            if (SymbolValidation(identifier))
                return semanticModel.GetSymbolInfo(identifier).Symbol;

            return default(ISymbol);
        }

        private bool SymbolValidation(IdentifierNameSyntax identifier)
        {
            return !(identifier == null || identifier.ToString().EndsWith("ID") || identifier.ToString().EndsWith("Id"));
        }

        private bool HasSmallTableAttribute(SemanticModel semanticModel, SyntaxNode node)
        {
            return semanticModel.GetTypeInfo(node).Type.GetAttributes().Any(it => it.AttributeClass.Name == "SmallTableAttribute");
        }

        private bool ItIsNotDatabaseCall(SemanticModel semanticModel, SyntaxNode node)
        {
            if (node == null) return true;
            var method = semanticModel.GetSymbolInfo(node).Symbol as IMethodSymbol;
            var receiverType = method?.ReceiverType?.ToString();
            if (receiverType.IsEmpty()) return true;
            return receiverType != Type && receiverType != Criterion;
        }
    }
}

