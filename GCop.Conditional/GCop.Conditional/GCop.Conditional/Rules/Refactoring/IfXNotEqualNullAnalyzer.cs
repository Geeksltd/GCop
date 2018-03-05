namespace GCop.Conditional.Rules.Refactoring
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    /// <summary>
    /// this class is supposed to cover task @16758 which includes 6 items:
    /// if (x != null) return x; [else] return y; Then warn to say: Change to return x ?? y; 
    ///----------------------------- 
    ///Also support: if (x.HasValue) return x; [else] return y; 
    ///------------------------------- 
    ///Also support: if (x != null) z = x; [else]    z = y;
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class IfXNotEqualNullAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.IfStatement;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "643",
                Category = Category.Refactoring,
                Message = "{0}",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;

            if (this.CheckIfElse(NodeToAnalyze as IfStatementSyntax, context)) return;

            if (CheckIfElseXHasValue(NodeToAnalyze as IfStatementSyntax, context)) return;

            if (CheckIfElseWithZ(NodeToAnalyze as IfStatementSyntax, context)) return;

            if (CheckIfWithoutElseByImmediateReturn(NodeToAnalyze as IfStatementSyntax, context)) return;

            if (CheckIfXHasValueWithoutElseByImmediateReturn(NodeToAnalyze as IfStatementSyntax, context)) return;

            if (CheckIfWithoutElseWithImmediateAssignZ(NodeToAnalyze as IfStatementSyntax, context)) return;
        }

        #region IF ELSE PART

        /// <summary>
        /// Looking for this pattern 
        /// if (x != null) return x; 
        /// else return y; 
        /// </summary>                
        bool CheckIfElse(IfStatementSyntax ifStatement, SyntaxNodeAnalysisContext context)
        {
            var notEquals = ifStatement.ChildNodes().OfKind(SyntaxKind.NotEqualsExpression);
            if (notEquals == null) return false;
            if (notEquals.None()) return false;
            if (notEquals.HasMany()) return false;

            //check else it is compulsory part
            if (ifStatement.Else == null) return false;

            //if (x !=null) 
            var firstNotEqualExperssion = notEquals.First();
            if (firstNotEqualExperssion.ChildNodes().OfKind(SyntaxKind.NullLiteralExpression).None()) return false;

            //keeping x
            var xVariable = firstNotEqualExperssion.GetIdentifier();

            //checking block{} in the if() {} 
            var block = ifStatement.ChildNodes().OfKind(SyntaxKind.Block);
            if (block.Any())
            {
                // if we have more than on line in block then skip it.
                if (block.FirstOrDefault().ChildNodes().OfType<CSharpSyntaxNode>().HasMany()) return false;

                var returns = block.FirstOrDefault().ChildNodes().OfType<ReturnStatementSyntax>();
                if (returns.None()) return false;
                //checking both x in => if(x!=null){return x;}
                if (returns?.FirstOrDefault()?.GetIdentifier() == null) return false;
                if (returns?.FirstOrDefault()?.GetIdentifier() != xVariable) return false;
                if (IsSimpleReturnStatment(returns?.FirstOrDefault()) == false) return false;
            }
            else
            {
                //checking both x in => if(x!=null)return x;
                if (ifStatement.GetReturnStatement()?.GetIdentifier() == null) return false;
                if (ifStatement.GetReturnStatement()?.GetIdentifier() != xVariable) return false;
                if (IsSimpleReturnStatment(ifStatement.GetReturnStatement()) == false) return false;
            }

            string yVariable = "";
            // Parsing the else part for =>  else return y; OR else { return y;}
            var elseClauseBlock = ifStatement.Else.ChildNodes().OfKind(SyntaxKind.Block);
            if (elseClauseBlock.Any())
            {
                var elseBlock = elseClauseBlock.FirstOrDefault().ChildNodes();
                if (elseBlock.OfType<CSharpSyntaxNode>().HasMany()) return false;

                if (elseBlock.OfType<ReturnStatementSyntax>().None()) return false;

                var first = elseBlock.OfType<ReturnStatementSyntax>().FirstOrDefault();
                if (IsSimpleReturnStatment(first) == false) return false;
                yVariable = first?.GetIdentifier();
            }
            else
            {
                if (ifStatement.Else.ChildNodes().OfType<CSharpSyntaxNode>().HasMany()) return false;

                var allElseReturns = ifStatement.Else.ChildNodes().OfType<ReturnStatementSyntax>();
                if (allElseReturns.None()) return false;
                if (IsSimpleReturnStatment(allElseReturns.FirstOrDefault()) == false) return false;
                yVariable = allElseReturns.FirstOrDefault()?.GetIdentifier();
            }

            if (yVariable.IsEmpty()) return false;

            ReportDiagnostic(context, ifStatement, $"Change to return {xVariable} ?? {yVariable};[1]");
            return true;
        }

        /// <summary>
        /// Looking for this pattern 
        /// if (x.HasValue) return x; 
        /// else return y; 
        /// </summary>       
        bool CheckIfElseXHasValue(IfStatementSyntax ifStatement, SyntaxNodeAnalysisContext context)
        {
            var condition = ifStatement.Condition;
            if (condition == null) return false;
            if (condition.ToString().Lacks(".HasValue"))
                return false;

            var conditionExpression = condition as MemberAccessExpressionSyntax;
            if (conditionExpression == null) return false;

            //keeping x
            var xVariableName = conditionExpression.ChildNodes().OfType<IdentifierNameSyntax>().FirstOrDefault()?.GetIdentifierSyntax();
            if (xVariableName == null) return false;
            if (xVariableName.GetIdentifier().IsEmpty()) return false;

            var hasValueProperty = conditionExpression.ChildNodes().OfType<IdentifierNameSyntax>().LastOrDefault()?.GetIdentifierSyntax();
            if (hasValueProperty == null) return false;
            if (hasValueProperty.GetIdentifier().IsEmpty()) return false;

            var propertyInfo = context.SemanticModel.GetSymbolInfo(hasValueProperty).Symbol as IPropertySymbol;
            var expressionIsHasValue = propertyInfo?.Name == "HasValue" && (propertyInfo?.ContainingType.ToString().IsAnyOf("int?", "double?", "decimal?", "System.DateTime?", "bool?") ?? false);

            if (expressionIsHasValue == false) return false;

            //check else it is compulsory part
            if (ifStatement.Else == null) return false;


            //checking block{} in the if() {} 
            var block = ifStatement.ChildNodes().OfKind(SyntaxKind.Block);
            if (block.Any())
            {
                // if we have more than on line in block then skip it.
                if (block.FirstOrDefault().ChildNodes().OfType<CSharpSyntaxNode>().HasMany()) return false;

                var returns = block.FirstOrDefault().ChildNodes().OfType<ReturnStatementSyntax>();
                if (returns.None()) return false;

                if (IsSimpleReturnStatment(returns?.FirstOrDefault()) == false) return false;

                //checking both x in => if(x!=null){return x;}
                if (returns?.FirstOrDefault()?.GetIdentifier() == null) return false;
                if (returns?.FirstOrDefault()?.GetIdentifier() != xVariableName.GetIdentifier()) return false;
            }
            else
            {
                //checking both x in => if(x!=null)return x;
                if (ifStatement.GetReturnStatement()?.GetIdentifier() == null) return false;
                if (ifStatement.GetReturnStatement()?.GetIdentifier() != xVariableName.GetIdentifier()) return false;

                if (IsSimpleReturnStatment(ifStatement.GetReturnStatement()) == false) return false;
            }

            string yVariable = "";
            // Parsing the else part for =>  else return y; OR else { return y;}
            var elseClauseBlock = ifStatement.Else.ChildNodes().OfKind(SyntaxKind.Block);
            if (elseClauseBlock.Any())
            {
                var elseBlock = elseClauseBlock.FirstOrDefault().ChildNodes();
                if (elseBlock.OfType<CSharpSyntaxNode>().HasMany()) return false;

                var allReturnTypes = elseBlock.OfType<ReturnStatementSyntax>();
                if (allReturnTypes.None()) return false;
                if (IsSimpleReturnStatment(allReturnTypes.FirstOrDefault()) == false) return false;
                yVariable = allReturnTypes.FirstOrDefault()?.GetIdentifier();
            }
            else
            {
                var ifelseChildNodes = ifStatement.Else.ChildNodes();
                if (ifelseChildNodes.OfType<CSharpSyntaxNode>().HasMany()) return false;
                if (ifelseChildNodes.OfType<ReturnStatementSyntax>().None()) return false;
                if (IsSimpleReturnStatment(ifelseChildNodes.OfType<ReturnStatementSyntax>().FirstOrDefault()) == false) return false;

                yVariable = ifStatement.Else.ChildNodes().OfType<ReturnStatementSyntax>().FirstOrDefault()?.GetIdentifier();
            }

            if (yVariable.IsEmpty()) return false;

            ReportDiagnostic(context, ifStatement, $"Change to return {xVariableName.GetIdentifier()} ?? {yVariable};[2]");
            return true;
        }

        /// <summary>
        /// Looking for this pattern 
        ///  if (x != null) z = x; 
        ///  else z = y;
        /// </summary>        
        bool CheckIfElseWithZ(IfStatementSyntax ifStatement, SyntaxNodeAnalysisContext context)
        {
            var notEquals = ifStatement.ChildNodes().OfKind(SyntaxKind.NotEqualsExpression);
            if (notEquals == null) return false;
            if (notEquals.None()) return false;
            if (notEquals.HasMany()) return false;

            //checking else it is compulsory part
            if (ifStatement.Else == null) return false;

            //if (x !=null) 
            var firstNotEqualExperssion = notEquals.First();
            if (firstNotEqualExperssion.ChildNodes().OfKind(SyntaxKind.NullLiteralExpression).None()) return false;

            //keeping x
            var xVariable = firstNotEqualExperssion.GetIdentifier();
            if (xVariable.IsEmpty())
            {
                // this.XVarible  ==> keeping x 
                var left = (firstNotEqualExperssion as BinaryExpressionSyntax).Left;
                if (left != null)
                {
                    if (left is MemberAccessExpressionSyntax membr)
                    { xVariable = membr.GetIdentifier(); }
                }
            }
            if (xVariable.IsEmpty()) return false;

            //Finding z and  z=x; OR { z=x;}
            string insideZ1 = "";
            AssignmentExpressionSyntax assignInsideIf = null;
            var block = ifStatement.ChildNodes().OfKind(SyntaxKind.Block);
            if (block.Any())
            {
                // if we have more than on line in block then skip it.
                if (block.FirstOrDefault().ChildNodes().OfType<CSharpSyntaxNode>().HasMany()) return false;
                var experssion = block.FirstOrDefault().ChildNodes().OfType<ExpressionStatementSyntax>().FirstOrDefault();
                if (experssion != null)
                    if (experssion.IsNone() == false)
                        assignInsideIf = experssion?.ChildNodes()?.OfType<AssignmentExpressionSyntax>().FirstOrDefault();
            }
            else
            {
                assignInsideIf = ifStatement.ChildNodes().OfType<ExpressionStatementSyntax>().FirstOrDefault()?.ChildNodes()?.OfType<AssignmentExpressionSyntax>()?.FirstOrDefault();
            }
            if (assignInsideIf == null) return false;

            var allIdentifiers = assignInsideIf?.ChildNodes().OfKind(SyntaxKind.IdentifierName);
            if (allIdentifiers.None()) return false;

            var insideX = allIdentifiers.LastOrDefault()?.GetIdentifier();
            if (insideX.IsEmpty()) return false;
            if (insideX != xVariable) return false;

            insideZ1 = allIdentifiers.FirstOrDefault()?.GetIdentifier();
            if (insideZ1.IsEmpty()) return false;

            // Parsing the else part for =>  else Z1 = Y1; OR {else Z1 = Y1;}
            string insideZ2 = "";
            AssignmentExpressionSyntax assignInsideElse = null;
            var elseClauseBlock = ifStatement.Else.ChildNodes().OfKind(SyntaxKind.Block);
            if (elseClauseBlock.Any())
            {
                // if we have more than on line in else block then skip it.
                if (elseClauseBlock.FirstOrDefault().ChildNodes().OfType<CSharpSyntaxNode>().HasMany()) return false;
                var experssion = elseClauseBlock.FirstOrDefault().ChildNodes().OfType<ExpressionStatementSyntax>().FirstOrDefault();
                if (experssion.IsNone() == false)
                    assignInsideElse = experssion.ChildNodes().OfType<AssignmentExpressionSyntax>().FirstOrDefault();
            }
            else
            {
                assignInsideElse = ifStatement.Else.ChildNodes().OfType<ExpressionStatementSyntax>().FirstOrDefault()?.ChildNodes()?.OfType<AssignmentExpressionSyntax>().FirstOrDefault();
            }
            if (assignInsideElse == null) return false;

            var elseIdentifiers = assignInsideElse?.ChildNodes().OfKind(SyntaxKind.IdentifierName);

            var yVariable = elseIdentifiers.LastOrDefault()?.GetIdentifier();
            if (yVariable.IsEmpty()) return false;

            insideZ2 = elseIdentifiers.FirstOrDefault()?.GetIdentifier();
            if (insideZ2.IsEmpty()) return false;

            if (insideZ1 != insideZ2) return false;

            ReportDiagnostic(context, ifStatement, $"Change to {insideZ2} = {xVariable} ?? {yVariable};[3]");
            return true;
        }

        #endregion

        #region If statement with Immediate RETURN code after if (there is no else inside of if statement)

        /// <summary>
        /// Looking for this pattern 
        /// if (x != null) return x; 
        /// return y; 
        /// </summary>                
        bool CheckIfWithoutElseByImmediateReturn(IfStatementSyntax ifStatement, SyntaxNodeAnalysisContext context)
        {
            var notEquals = ifStatement.ChildNodes().OfKind(SyntaxKind.NotEqualsExpression);
            if (notEquals == null) return false;
            if (notEquals.None()) return false;
            if (notEquals.HasMany()) return false;

            //checking else , in this method we should not have else part
            if (ifStatement.Else != null) return false;

            //checking return y location which should be exactly at next line after if statement
            var ifLocation = ifStatement.GetLocation();
            var allNodesInParent = ifStatement.GetSingleAncestor<BlockSyntax>().ChildNodes().ToArray();
            var nextNode = allNodesInParent.FirstOrDefault(x => x.GetLocation().SourceSpan.Start > ifLocation.SourceSpan.End);

            var returnExperssion = nextNode as ReturnStatementSyntax;
            if (returnExperssion == null) return false;
            if (IsSimpleReturnStatment(returnExperssion) == false) return false;

            //if (x !=null) 
            var firstNotEqualExperssion = notEquals.First();
            if (firstNotEqualExperssion.ChildNodes().OfKind(SyntaxKind.NullLiteralExpression).None()) return false;

            //keeping x
            var xVariable = firstNotEqualExperssion.GetIdentifier();

            //checking block{} in the if() {} 
            var block = ifStatement.ChildNodes().OfKind(SyntaxKind.Block);
            if (block.Any())
            {
                // if we have more than on line in block then skip it.
                if (block.FirstOrDefault().ChildNodes().OfType<CSharpSyntaxNode>().HasMany()) return false;

                var returns = block.FirstOrDefault().ChildNodes().OfType<ReturnStatementSyntax>();
                if (returns.None()) return false;

                if (IsSimpleReturnStatment(returns?.FirstOrDefault()) == false) return false;

                //checking both x in => if(x!=null){return x;}
                if (returns?.FirstOrDefault()?.GetIdentifier() == null) return false;
                if (returns?.FirstOrDefault()?.GetIdentifier() != xVariable) return false;
            }
            else
            {
                //checking both x in => if(x!=null)return x;
                if (ifStatement.GetReturnStatement()?.GetIdentifier() == null) return false;
                if (ifStatement.GetReturnStatement()?.GetIdentifier() != xVariable) return false;
                if (IsSimpleReturnStatment(ifStatement.GetReturnStatement()) == false) return false;
            }

            // Parsing the return type return y;            
            var yVariable = returnExperssion.DescendantNodes().OfKind(SyntaxKind.IdentifierName).FirstOrDefault();
            if (yVariable == null) return false;
            if (yVariable.GetIdentifier().IsEmpty()) return false;
            if (yVariable.GetIdentifier() == xVariable) return false;

            ReportDiagnostic(context, ifStatement, $"Change to return {xVariable} ?? {yVariable};[4]");
            ReportDiagnostic(context, returnExperssion, $"Change to return {xVariable} ?? {yVariable};[4]");
            return true;
        }

        /// <summary>
        /// Looking for this pattern 
        /// if (x.HasValue) return x; 
        /// return y; 
        /// </summary>       
        bool CheckIfXHasValueWithoutElseByImmediateReturn(IfStatementSyntax ifStatement, SyntaxNodeAnalysisContext context)
        {
            var condition = ifStatement.Condition;
            if (condition == null) return false;
            if (condition.ToString().Lacks(".HasValue"))
                return false;

            var conditionExpression = condition as MemberAccessExpressionSyntax;
            if (conditionExpression == null) return false;

            //checking else , in this method we should not have else part
            if (ifStatement.Else != null) return false;

            //checking return y location which should be exactly at next line after if statement
            var ifLocation = ifStatement.GetLocation();
            var allNodesInParent = ifStatement.GetSingleAncestor<BlockSyntax>().ChildNodes().ToArray();
            var nextNode = allNodesInParent.FirstOrDefault(x => x.GetLocation().SourceSpan.Start > ifLocation.SourceSpan.End);

            var returnExperssion = nextNode as ReturnStatementSyntax;
            if (returnExperssion == null) return false;

            //keeping x
            var xVariableName = conditionExpression.ChildNodes().OfType<IdentifierNameSyntax>().FirstOrDefault()?.GetIdentifierSyntax();
            if (xVariableName == null) return false;
            if (xVariableName.GetIdentifier().IsEmpty()) return false;

            var hasValueProperty = conditionExpression.ChildNodes().OfType<IdentifierNameSyntax>().LastOrDefault()?.GetIdentifierSyntax();
            if (hasValueProperty == null) return false;
            if (hasValueProperty.GetIdentifier().IsEmpty()) return false;

            var propertyInfo = context.SemanticModel.GetSymbolInfo(hasValueProperty).Symbol as IPropertySymbol;
            var expressionIsHasValue = propertyInfo?.Name == "HasValue" && (propertyInfo?.ContainingType.ToString().IsAnyOf("int?", "double?", "decimal?", "System.DateTime?", "bool?") ?? false);

            if (expressionIsHasValue == false) return false;

            //checking block{} in the if() {} 
            var block = ifStatement.ChildNodes().OfKind(SyntaxKind.Block);
            if (block.Any())
            {
                // if we have more than one line in block then skip it.
                if (block.FirstOrDefault().ChildNodes().OfType<CSharpSyntaxNode>().HasMany()) return false;

                var returns = block.FirstOrDefault().ChildNodes().OfType<ReturnStatementSyntax>();
                if (returns.None()) return false;

                if (IsSimpleReturnStatment(returns?.FirstOrDefault()) == false) return false;

                //checking both x in => if(x!=null){return x;}
                if (returns?.FirstOrDefault()?.GetIdentifier() == null) return false;
                if (returns?.FirstOrDefault()?.GetIdentifier() != xVariableName.GetIdentifier()) return false;
            }
            else
            {
                //checking both x in => if(x!=null)return x;
                if (ifStatement.GetReturnStatement()?.GetIdentifier() == null) return false;
                if (ifStatement.GetReturnStatement()?.GetIdentifier() != xVariableName.GetIdentifier()) return false;

                if (IsSimpleReturnStatment(ifStatement.GetReturnStatement()) == false) return false;
            }


            // Parsing the return type return y;            
            var yVariable = returnExperssion.DescendantNodes().OfKind(SyntaxKind.IdentifierName).FirstOrDefault();
            if (yVariable == null) return false;
            if (yVariable.GetIdentifier().IsEmpty()) return false;
            if (yVariable.GetIdentifier() == xVariableName.GetIdentifier()) return false;

            ReportDiagnostic(context, yVariable, $"Change to return {xVariableName.GetIdentifier()} ?? {yVariable.GetIdentifier()};[5]");
            ReportDiagnostic(context, ifStatement, $"Change to return {xVariableName.GetIdentifier()} ?? {yVariable.GetIdentifier()};[5]");

            return true;
        }

        /// <summary>
        /// Looking for this pattern 
        ///  if (x != null) z = x; 
        ///  z = y;
        /// </summary>        
        bool CheckIfWithoutElseWithImmediateAssignZ(IfStatementSyntax ifStatement, SyntaxNodeAnalysisContext context)
        {
            var notEquals = ifStatement.ChildNodes().OfKind(SyntaxKind.NotEqualsExpression);
            if (notEquals == null) return false;
            if (notEquals.None()) return false;
            if (notEquals.HasMany()) return false;

            //checking else , in this method we should not have else part
            if (ifStatement.Else != null) return false;

            //if (x !=null) 
            var firstNotEqualExperssion = notEquals.First();
            if (firstNotEqualExperssion.ChildNodes().OfKind(SyntaxKind.NullLiteralExpression).None()) return false;

            //keeping x
            var xVariable = firstNotEqualExperssion.GetIdentifier();

            //checking z=y; location which should be exactly at next line after if statement
            var ifLocation = ifStatement.GetLocation();
            var allNodesInParent = ifStatement.GetSingleAncestor<BlockSyntax>().ChildNodes().ToArray();
            var nextNode = allNodesInParent.FirstOrDefault(x => x.GetLocation().SourceSpan.Start > ifLocation.SourceSpan.End);

            var nextLineStatement = nextNode as ExpressionStatementSyntax;
            if (nextLineStatement == null) return false;
            var assignAfterIf = nextLineStatement.ChildNodes().OfType<AssignmentExpressionSyntax>().FirstOrDefault();
            if (assignAfterIf == null) return false;

            //Finding z and  z=x; OR { z=x;}
            string insideZ1 = "";
            AssignmentExpressionSyntax assignInsideIf = null;
            var block = ifStatement.ChildNodes().OfKind(SyntaxKind.Block);
            if (block.Any())
            {
                // if we have more than on line in block then skip it.
                if (block.FirstOrDefault().ChildNodes().OfType<CSharpSyntaxNode>().HasMany()) return false;
                var experssion = block.FirstOrDefault().ChildNodes().OfType<ExpressionStatementSyntax>().FirstOrDefault();
                if (experssion != null)
                    assignInsideIf = experssion?.ChildNodes()?.OfType<AssignmentExpressionSyntax>().FirstOrDefault();
            }
            else
            {
                assignInsideIf = ifStatement.ChildNodes().OfType<ExpressionStatementSyntax>().FirstOrDefault()?.ChildNodes()?.OfType<AssignmentExpressionSyntax>()?.FirstOrDefault();
            }

            if (assignInsideIf == null) return false;

            var allIdentifiers = assignInsideIf?.ChildNodes().OfKind(SyntaxKind.IdentifierName);
            if (allIdentifiers.None()) return false;

            var insideX = allIdentifiers.LastOrDefault()?.GetIdentifier();
            if (insideX.IsEmpty()) return false;
            if (insideX != xVariable) return false;

            insideZ1 = allIdentifiers.FirstOrDefault()?.GetIdentifier();
            if (insideZ1.IsEmpty()) return false;

            // Parsing the next line after if statment  for =>  Z1 = Y1; 
            string insideZ2 = "";
            //AssignmentExpressionSyntax assignAfterIf = null;
            // assignAfterIf = assignmentExpressionSyntax.ChildNodes()?.OfType<AssignmentExpressionSyntax>().FirstOrDefault();

            if (assignAfterIf == null) return false;

            var afterIfAssignmentIdentifiers = assignAfterIf?.ChildNodes().OfKind(SyntaxKind.IdentifierName);

            var yVariable = afterIfAssignmentIdentifiers.LastOrDefault()?.GetIdentifier();
            if (yVariable.IsEmpty()) return false;

            insideZ2 = afterIfAssignmentIdentifiers.FirstOrDefault()?.GetIdentifier();
            if (insideZ2.IsEmpty()) return false;

            if (insideZ1 != insideZ2) return false;

            ReportDiagnostic(context, ifStatement, $"Change to {insideZ2} = {xVariable} ?? {yVariable};[6]");
            ReportDiagnostic(context, nextLineStatement, $"Change to {insideZ2} = {xVariable} ?? {yVariable};[6]");
            return true;
        }

        #endregion

        bool IsSimpleReturnStatment(ReturnStatementSyntax returnStatement)
        {
            if (returnStatement == null) return false;
            if (returnStatement.ChildNodes().HasMany()) return false;

            var firstNode = returnStatement.ChildNodes().FirstOrDefault();
            if ((firstNode as InvocationExpressionSyntax) != null) return false;
            if ((firstNode as BinaryExpressionSyntax) != null) return false;
            if ((firstNode as AnonymousObjectCreationExpressionSyntax) != null) return false;
            if ((firstNode as MemberAccessExpressionSyntax) != null) return false;
            if (firstNode is ParenthesizedExpressionSyntax parentezz)
            {
                if (parentezz.ChildNodes().HasMany()) return false;
                if (parentezz.ChildNodes().FirstOrDefault().GetIdentifier().IsEmpty()) return false;
            }
            else
     if (returnStatement.ChildNodes().FirstOrDefault().GetIdentifier().IsEmpty()) return false;

            return true;
        }
    }
}