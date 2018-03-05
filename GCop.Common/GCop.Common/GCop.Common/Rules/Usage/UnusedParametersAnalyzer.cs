namespace GCop.Common.Rules.Usage
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Collections.Generic;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class UnusedParametersAnalyzer : GCopAnalyzer
	{
		private static readonly string[] ThrowExpression = new string[] { "throw", "new", "NotImplementedException()" };

		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "502",
				Category = Category.Usage,
				Severity = DiagnosticSeverity.Warning,
				Message = "The parameter '{0}' doesn't seem to be used in this method. Consider removing it. If the argument must be declared for compiling reasons, rename it to contain only underscore character."
			};
		}

		protected override void Configure()
		{
			RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration, SyntaxKind.ConstructorDeclaration);
		}

		private void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var methodOrConstructor = context.Node as BaseMethodDeclarationSyntax;
			if (methodOrConstructor == null) return;

			var semanticModel = context.SemanticModel;
			if (!IsCandidateForRemoval(methodOrConstructor, semanticModel)) return;

			if (methodOrConstructor.AttributeLists.Any(it => it.Attributes.Any(attr => attr.Name.ToString().Contains("DllImport")))) return;

			//We should not look at parameters which are decorated with [this] keyword or thier type is AutomatedTask
			var parameters = methodOrConstructor.ParameterList.Parameters
				.Where(it => it.Type.ToString() != "AutomatedTask" && it.ChildTokens().None(x => x.IsKind(SyntaxKind.ThisKeyword)))
				.ToDictionary(p => p, p => semanticModel.GetDeclaredSymbol(p));

			var ctor = methodOrConstructor as ConstructorDeclarationSyntax;

			if (ctor?.Initializer != null)
			{
				var symbolsTouched = new List<ISymbol>();
				foreach (var arg in ctor.Initializer.ArgumentList.Arguments)
				{
					var dataFlowAnalysis = semanticModel.AnalyzeDataFlow(arg.Expression);
					if (!dataFlowAnalysis.Succeeded) continue;
					symbolsTouched.AddRange(dataFlowAnalysis.ReadInside);
					symbolsTouched.AddRange(dataFlowAnalysis.WrittenInside);
				}

				var parametersToRemove = parameters.Where(p => symbolsTouched.Contains(p.Value)).ToList();
				foreach (var parameter in parametersToRemove)
					parameters.Remove(parameter.Key);
			}

			if (methodOrConstructor.Body.Statements.Any())
			{
				foreach (var parameter in parameters)
				{
					if (parameter.Key.Identifier.ValueText.None(@char => @char != '_') || parameter.Key.Identifier.ValueText == "e") continue;

					var used = methodOrConstructor.Body
						.DescendantNodesAndSelf()
						.OfType<IdentifierNameSyntax>()
						.Any(iName => IdentifierRefersToParam(iName, parameter.Key));

					if (!used)
					{
						ReportDiagnostic(context, parameter.Key, parameter.Key.Identifier.ValueText);
					}
				}
				//
				// THIS IS THE RIGHT WAY TO DO THIS VERIFICATION. 
				// BUT, WE HAVE TO WAIT FOR A "BUGFIX" FROM ROSLYN TEAM
				// IN DataFlowAnalysis
				//
				// https://github.com/dotnet/roslyn/issues/6967
				//
				//var dataFlowAnalysis = semanticModel.AnalyzeDataFlow(methodOrConstructor.Body);
				//if (!dataFlowAnalysis.Succeeded) return;
				//foreach (var parameter in parameters)
				//{

				//    var parameterSymbol = parameter.Value;
				//    if (parameterSymbol == null) continue;
				//    if (!dataFlowAnalysis.ReadInside.Contains(parameterSymbol) &&
				//        !dataFlowAnalysis.WrittenInside.Contains(parameterSymbol))
				//    {
				//        ReportDiagnostic(context, parameter.Key);
				//    }
				//}
			}
			else
			{
				foreach (var parameter in parameters.Keys)
					ReportDiagnostic(context, parameter, parameter.Identifier.ValueText);
			}
		}

		private static bool IdentifierRefersToParam(IdentifierNameSyntax iName, ParameterSyntax param)
		{
			if (iName.Identifier.ToString() != param.Identifier.ToString())
				return false;

			var mae = iName.Parent as MemberAccessExpressionSyntax;
			if (mae == null)
				return true;

			return mae.DescendantNodes().FirstOrDefault() == iName;
		}

		private static bool IsCandidateForRemoval(BaseMethodDeclarationSyntax methodOrConstructor, SemanticModel semanticModel)
		{
			if (methodOrConstructor.Modifiers.Any(m => m.ValueText.IsAnyOf("partial", "virtual", "override"))
				|| !methodOrConstructor.ParameterList.Parameters.Any()
				|| methodOrConstructor.Body == null)
				return false;

			if (methodOrConstructor is MethodDeclarationSyntax method)
			{
				if (method.ExplicitInterfaceSpecifier != null) return false;
				var methodSymbol = semanticModel.GetDeclaredSymbol(method);
				if (methodSymbol == null) return false;
				var typeSymbol = methodSymbol.ContainingType;
				if (typeSymbol.AllInterfaces.SelectMany(i => i.GetMembers())
					.Any(member => methodSymbol.Equals(typeSymbol.FindImplementationForInterfaceMember(member))))
					return false;
				if (IsEventHandlerLike(method, semanticModel)) return false;
				if (MethodBodyIsNotImplemented(method)) return false;
			}
			else
			{
				if (methodOrConstructor is ConstructorDeclarationSyntax constructor)
				{
					if (IsSerializationConstructor(constructor, semanticModel)) return false;
				}
				else
				{
					return false;
				}
			}

			return true;
		}

		private static bool IsSerializationConstructor(ConstructorDeclarationSyntax constructor, SemanticModel semanticModel)
		{
			if (constructor.ParameterList.Parameters.Count != 2) return false;
			var constructorSymbol = semanticModel.GetDeclaredSymbol(constructor);
			var typeSymbol = constructorSymbol?.ContainingType;
			if (typeSymbol?.AllInterfaces.None(i => i.ToString() == "System.Runtime.Serialization.ISerializable") ?? true) return false;
			if (typeSymbol.GetAttributes().None(a => a.AttributeClass.ToString() == "System.SerializableAttribute")) return false;
			var serializationInfoType = semanticModel.GetTypeInfo(constructor.ParameterList.Parameters[0].Type).Type as INamedTypeSymbol;
			if (serializationInfoType == null) return false;
			if (serializationInfoType.AllBaseTypesAndSelf().None(type => type.ToString() == "System.Runtime.Serialization.SerializationInfo"))
				return false;
			var streamingContextType = semanticModel.GetTypeInfo(constructor.ParameterList.Parameters[1].Type).Type as INamedTypeSymbol;
			if (streamingContextType == null) return false;
			return streamingContextType.AllBaseTypesAndSelf().Any(type => type.ToString() == "System.Runtime.Serialization.StreamingContext");
		}

		private static bool IsEventHandlerLike(MethodDeclarationSyntax method, SemanticModel semanticModel)
		{
			//If there is _ in method names 
			if (method.Identifier.ValueText.Contains("_")) return true;

			if (method.ParameterList.Parameters.None()
				|| method.ReturnType.ToString() != "void")
				return false;

			var firstParameter = method.ParameterList.Parameters[0];
			if (firstParameter.Identifier.ValueText == "sender") return true;

			var senderType = semanticModel.GetTypeInfo(firstParameter.Type).Type;
			if (senderType.SpecialType != SpecialType.System_Object) return false;

			return method.ParameterList.Parameters.TrueForAtLeastOnce(it =>
			{
				var eventArgsType = semanticModel.GetTypeInfo(it.Type).Type as INamedTypeSymbol;
				if (eventArgsType == null) return false;
				return eventArgsType.AllBaseTypesAndSelf().Any(type => type.ToString() == "System.EventArgs");
			});
		}

		private static bool MethodBodyIsNotImplemented(MethodDeclarationSyntax method)
		{
			var bodyImplementation = method.Body.DescendantNodes();
			if (bodyImplementation.None()) return false;

			var statement = bodyImplementation.First();

			if (statement.IsKind(SyntaxKind.ThrowStatement) && statement.ToString().ContainsAll(ThrowExpression, caseSensitive: true))
			{
				return true;
			}
			return false;
		}
	}
}
