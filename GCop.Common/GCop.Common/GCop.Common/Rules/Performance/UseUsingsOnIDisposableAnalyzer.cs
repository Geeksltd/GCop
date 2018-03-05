namespace GCop.Common.Rules.Performance
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class UseUsingsOnIDisposableAnalyzer : GCopAnalyzer
	{
		IgnoredType[] IgnoredTypes = new IgnoredType[]
		{
			IgnoredType.Create("WebClient" , "System.Net"),
			IgnoredType.Create("MemoryStream" , "System.IO"),
			IgnoredType.Create("Form" , "System.Windows.Forms"),
			IgnoredType.Create("Control" , "System.Windows.Forms"),
			IgnoredType.Create("Control" , "System.Web.UI"),
			IgnoredType.Create("TextWriter" , "System.IO"),
			IgnoredType.Create("TextReader" , "System.IO"),
			IgnoredType.Create("Timer" , "System.Windows.Forms"),
			IgnoredType.Create("DataColumn" , "System.Data"),
			IgnoredType.Create("DataTable" , "System.Data"),
			IgnoredType.Create("DataSet" , "System.Data"),
			IgnoredType.Create("Process" , "System.Diagnostics"),
			IgnoredType.Create("ServiceHost" , "System.ServiceModel")
		};

		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "302",
				Category = Category.Performance,
				Severity = DiagnosticSeverity.Warning,
				Message = "Since '{0}' implements IDisposable, wrap it in a using() statement"
			};
		}

		protected override void Configure()
		{
			RegisterSyntaxNodeAction(c => Analyze(c), SyntaxKind.ObjectCreationExpression);
		}

		private void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var objectCreation = context.Node as ObjectCreationExpressionSyntax;
			if (FirstEvaluate(context, objectCreation)) return;

			ISymbol identitySymbol = null;
			StatementSyntax statement = null;
			SyntaxToken? identifierToken = null;
			var semanticModel = context.SemanticModel;
			var type = semanticModel.GetSymbolInfo(objectCreation.Type).Symbol as INamedTypeSymbol;

			var evaluate = SecondEvaluate(semanticModel, objectCreation, identifierToken, identitySymbol, statement);
			var firstLocation = identifierToken?.GetLocation() ?? objectCreation.GetLocation();
			if (evaluate) return;
			else if (objectCreation.Parent.IsAnyKind(SyntaxKind.SimpleLambdaExpression, SyntaxKind.ParenthesizedLambdaExpression))
			{
				var anonymousFunction = objectCreation.Parent as AnonymousFunctionExpressionSyntax;
				var methodSymbol = semanticModel.GetSymbolInfo(anonymousFunction).Symbol as IMethodSymbol;
				if (!methodSymbol.ReturnsVoid) return;
				var location = firstLocation;
				ReportDiagnostic(context, location, type.Name);
			}
			else
			{
				ReportDiagnostic(context, firstLocation, type.Name);
				return;
			}
			if (statement == null && identitySymbol == null) return;
			var isDisposeOrAssigned = IsDisposedOrAssigned(semanticModel, statement, (ILocalSymbol)identitySymbol);
			if (isDisposeOrAssigned) return;
			var method = context.Node.GetParent<MethodDeclarationSyntax>() as MethodDeclarationSyntax;
			if (method == null) return;
			if (IsBeingPassedAsParameterToAnotherMethod(method?.Body, identifierToken)) return;
			ReportDiagnostic(context, firstLocation, type.Name);
		}

		private bool SecondEvaluate(SemanticModel semanticModel, ObjectCreationExpressionSyntax objectCreation, SyntaxToken? identifierToken, ISymbol identitySymbol, StatementSyntax statement)
		{
			var assignment = objectCreation.GetParent<EqualsValueClauseSyntax>();
			if (assignment != null)
				identifierToken = assignment.As<EqualsValueClauseSyntax>().EqualsToken.GetPreviousToken();
			else
			{
				assignment = objectCreation.GetParent<AssignmentExpressionSyntax>();
				if (assignment != null)
					identifierToken = assignment.As<AssignmentExpressionSyntax>().Left.GetFirstToken();
			}
			
			if (objectCreation.Parent.IsKind(SyntaxKind.SimpleAssignmentExpression))
			{
				var assignmentExpression = (AssignmentExpressionSyntax)objectCreation.Parent;
				if (assignmentExpression?.Left == null) return true;

				identitySymbol = semanticModel.GetSymbolInfo(assignmentExpression.Left).Symbol;
				if (identitySymbol?.Kind != SymbolKind.Local) return true;
				if (assignmentExpression.FirstAncestorOrSelf<MethodDeclarationSyntax>() == null) return true;
				if (assignmentExpression.Parent is UsingStatementSyntax usingStatement) return true;
				statement = assignmentExpression.Parent as ExpressionStatementSyntax;
			}
			else if (objectCreation.Parent.IsKind(SyntaxKind.EqualsValueClause) && objectCreation.Parent.Parent.IsKind(SyntaxKind.VariableDeclarator))
			{
				var variableDeclarator = (VariableDeclaratorSyntax)objectCreation.Parent.Parent;
				var variableDeclaration = variableDeclarator?.Parent as VariableDeclarationSyntax;
				if (variableDeclarator == null) return true;
				identitySymbol = semanticModel.GetDeclaredSymbol(variableDeclarator);
				if (identitySymbol == null) return true;
				if (variableDeclaration?.Parent is UsingStatementSyntax usingStatement) return true;
				statement = variableDeclaration.Parent as LocalDeclarationStatementSyntax;
				if ((statement?.FirstAncestorOrSelf<MethodDeclarationSyntax>()) == null) return true;
			}
			return false;
		}

		private bool FirstEvaluate(SyntaxNodeAnalysisContext context, ObjectCreationExpressionSyntax objectCreation)
		{
			if (objectCreation == null) return true;
			if (objectCreation.Parent == null) return true;
			if (objectCreation.Parent.IsAnyKind(SyntaxKind.ReturnStatement, SyntaxKind.UsingStatement, SyntaxKind.ArrowExpressionClause))
				return true;
			if (objectCreation.Ancestors().OfType<InvocationExpressionSyntax>().Any()) return true;
			if (objectCreation.Ancestors().Any(i => i.IsAnyKind(
				SyntaxKind.ThisConstructorInitializer,
				SyntaxKind.BaseConstructorInitializer,
				SyntaxKind.ObjectCreationExpression)))
				return true;
			var semanticModel = context.SemanticModel;
			if (objectCreation.Type == null) return true;
			var type = semanticModel.GetSymbolInfo(objectCreation.Type).Symbol as INamedTypeSymbol;
			if (type == null) return true;
			if (type.AllInterfaces.None(i => i.ToString() == "System.IDisposable")) return true;
			if (IsInIgnoredList(type)) return true;
			if (type.AllBaseTypes().TrueForAtLeastOnce(baseType => { return IsInIgnoredList(baseType); })) return true;
			return false;
		}

		private bool IsInIgnoredList(ITypeSymbol typeSymbol) => IgnoredTypes.TrueForAtLeastOnce(type => type.Equals(typeSymbol));

		private bool IsBeingPassedAsParameterToAnotherMethod(BlockSyntax methodBody, SyntaxToken? identifierToken)
		{
			if (methodBody == null || identifierToken == null) return false;
			return methodBody.DescendantNodes().OfType<InvocationExpressionSyntax>().TrueForAtLeastOnce(invocation =>
			{
				return invocation.ArgumentList.Arguments.Any(it => it.GetIdentifier() == identifierToken.ToString());
			});
		}

		private static bool IsDisposedOrAssigned(SemanticModel semanticModel, StatementSyntax statement, ILocalSymbol identitySymbol)
		{
			var method = statement.FirstAncestorOrSelf<MethodDeclarationSyntax>();
			if (method == null) return false;
			if (IsReturned(method, statement, semanticModel, identitySymbol)) return true;
			foreach (var childStatements in method.Body.DescendantNodes().OfType<StatementSyntax>())
			{
				if (childStatements.SpanStart > statement.SpanStart
				&& (IsCorrectDispose(childStatements as ExpressionStatementSyntax, semanticModel, identitySymbol)
				|| IsAssignedToField(childStatements as ExpressionStatementSyntax, semanticModel, identitySymbol)))
					return true;
			}
			return false;
		}

		private static bool IsReturned(MethodDeclarationSyntax method, StatementSyntax statement, SemanticModel semanticModel, ILocalSymbol identitySymbol)
		{
			var anonymousFunction = statement.FirstAncestorOfKind(SyntaxKind.ParenthesizedLambdaExpression,
				SyntaxKind.SimpleLambdaExpression, SyntaxKind.AnonymousMethodExpression) as AnonymousFunctionExpressionSyntax;
			IMethodSymbol methodSymbol;
			BlockSyntax body;
			if (anonymousFunction != null)
			{
				methodSymbol = semanticModel.GetSymbolInfo(anonymousFunction).Symbol as IMethodSymbol;
				body = anonymousFunction.Body as BlockSyntax;
			}
			else
			{
				if (method == null) return true;
				methodSymbol = semanticModel.GetDeclaredSymbol(method);
				body = method.Body;
			}

			if (body == null) return true;
			var returnExpressions = body.DescendantNodes().OfType<ReturnStatementSyntax>().Select(r => r.Expression);
			var returnTypeSymbol = methodSymbol?.ReturnType;
			if (returnTypeSymbol == null) return false;
			if (returnTypeSymbol.SpecialType == SpecialType.System_Void) return false;
			var isReturning = returnExpressions.Any(returnExpression =>
			{
				var returnSymbol = semanticModel.GetSymbolInfo(returnExpression).Symbol;
				if (returnSymbol == null) return false;
				return returnSymbol.Equals(identitySymbol);
			});
			return isReturning;
		}

		private static bool IsAssignedToField(ExpressionStatementSyntax expressionStatement, SemanticModel semanticModel, ILocalSymbol identitySymbol)
		{
			if (expressionStatement == null) return false;
			if (!expressionStatement.Expression.IsKind(SyntaxKind.SimpleAssignmentExpression)) return false;
			var assignment = (AssignmentExpressionSyntax)expressionStatement.Expression;
			if (assignment?.Left == null) return false;
			var assignmentTarget = semanticModel.GetSymbolInfo(assignment.Left).Symbol;
			if (assignmentTarget?.Kind != SymbolKind.Field) return false;
			if (assignment?.Right == null) return false;
			var assignmentSource = semanticModel.GetSymbolInfo(assignment.Right).Symbol;
			return identitySymbol.Equals(assignmentSource);
		}

		private static bool IsCorrectDispose(ExpressionStatementSyntax expressionStatement, SemanticModel semanticModel, ILocalSymbol identitySymbol)
		{
			if (expressionStatement == null) return false;
			var invocation = expressionStatement.Expression as InvocationExpressionSyntax;
			if (invocation?.ArgumentList.Arguments.Any() ?? true) return false;
			var memberAccess = invocation.Expression as MemberAccessExpressionSyntax;
			if (memberAccess == null) return false;
			ISymbol memberSymbol;
			if (memberAccess.Expression.IsKind(SyntaxKind.IdentifierName))
			{
				memberSymbol = semanticModel.GetSymbolInfo(memberAccess.Expression).Symbol;
			}
			else if (memberAccess.Expression.IsKind(SyntaxKind.ParenthesizedExpression))
			{
				var parenthesizedExpression = (ParenthesizedExpressionSyntax)memberAccess.Expression;
				var cast = parenthesizedExpression.Expression as CastExpressionSyntax;
				if (cast == null) return false;
				var catTypeSymbol = semanticModel.GetTypeInfo(cast.Type).Type;
				if (catTypeSymbol.SpecialType != SpecialType.System_IDisposable) return false;
				if (cast.Expression == null) return false;
				memberSymbol = semanticModel.GetSymbolInfo(cast.Expression).Symbol;
			}
			else return false;
			if (memberSymbol == null || !memberSymbol.Equals(identitySymbol)) return false;
			var memberAccessed = memberAccess.Name as IdentifierNameSyntax;
			if (memberAccessed == null) return false;
			if (memberAccessed.Identifier.Text != "Dispose" || memberAccessed.Arity != 0) return false;
			var methodSymbol = semanticModel.GetSymbolInfo(memberAccessed).Symbol as IMethodSymbol;
			if (methodSymbol == null) return false;
			if (methodSymbol.ToString() == "System.IDisposable.Dispose()") return true;
			var disposeMethod = (IMethodSymbol)semanticModel.Compilation.GetSpecialType(SpecialType.System_IDisposable).GetMembers("Dispose").Single();
			var isDispose = methodSymbol.Equals(methodSymbol.ContainingType.FindImplementationForInterfaceMember(disposeMethod));
			return isDispose;
		}
	}
}