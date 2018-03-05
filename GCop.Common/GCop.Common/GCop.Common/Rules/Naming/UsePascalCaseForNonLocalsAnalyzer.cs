namespace GCop.Common.Rules.Naming
{
    using Core;
    using GCop.Common.Utilities;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class UsePascalCaseForNonLocalsAnalyzer : GCopAnalyzer
	{
		private readonly string StructLayoutAttribute = "System.Runtime.InteropServices.StructLayoutAttribute";
		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "209",
				Category = Category.Naming,
				Severity = DiagnosticSeverity.Warning,
				Message = "Use PascalCasing for {0} names"
			};
		}

		protected override void Configure()
		{
			RegisterSymbolAction(async c => await AnalyzeName(c),
				SymbolKind.NamedType, // class, interfaces, delegates and enums
				SymbolKind.Namespace, // namespaces
				SymbolKind.Event, // Events
				SymbolKind.Method, // Methods
				SymbolKind.Field, // Fields and Enum values
				SymbolKind.Property // Properties
				);
		}

		private async Task AnalyzeName(SymbolAnalysisContext context)
		{
			var symbol = context.Symbol;

			//Rule @209
			if (await CheckSymbolAsync(context, symbol)) return;

			var typeKind = symbol.Kind.ToString().ToLower();
			if (symbol is INamedTypeSymbol)
			{
				// get typekind of namedtypes
				typeKind = ((INamedTypeSymbol)symbol).TypeKind.ToString().ToLower();
			}
			else if (symbol is IFieldSymbol && symbol.ContainingType.TypeKind.ToString() == "Enum")
			{
				// enum values are considered fields, lets rename it to enum values.
				typeKind = "enum value";
			}

			var symbolName = symbol.Name;
			if (!symbolName.IsPascalCase())
			{
				var diagnostic = Diagnostic.Create(Description, symbol.Locations[0], typeKind);

				context.ReportDiagnostic(diagnostic);
			}
		}

		private async Task<bool> CheckSymbolAsync(SymbolAnalysisContext context, ISymbol symbol)
		{
			if (symbol.ContainingSymbol.GetAttributes().Any(it => it.AttributeClass.Name == typeof(EscapeGCopAttribute).Name)) return true;

			if (symbol.Name.IsEmpty()) return true;

			if (symbol.GetAttributes().Any(it => it.ToString().StartsWith(StructLayoutAttribute))) return true;
			// only run this rule for ordinary methods.

			if (symbol is IMethodSymbol method)
			{
				if (method.MethodKind != MethodKind.Ordinary)
				{
					return true;
				}

				// TASK#12197 - If a method has any argument where it's type is or inherits from EventArgs
				foreach (var param in method.Parameters)
				{
					var type = param.Type;
					while (type != null)
					{
						var typeName = (type.ContainingType?.Name).Or(type.Name);
						if (typeName == "EventArgs")
						{
							return true;
						}

						type = type.BaseType;
					}
				}
			}
			else if (symbol is IFieldSymbol)
			{
				var isFieldInPropertyGetter = await SymbolHelper.IsFieldSymbolUseInPropertyAsync(symbol as IFieldSymbol, context.CancellationToken);
				if (isFieldInPropertyGetter)
				{
					// This rule might conflict with UseCamelCasingForPropertyContainerFields - so check for that first.
					return true;
				}

				// allow _ in field names
				if (symbol.Name[0] == '_') return true;

				//If there is a method with the same name of filed, the rule should be skipped.
				if (HasClassAnyMethodWithSameName(symbol as IFieldSymbol)) return true;
				if (HasClassAnyPropertyWithSameName(symbol as IFieldSymbol)) return true;
			}
			else if (symbol is IPropertySymbol)
			{
				// special case
				if (symbol.Name == "info") return true;

				// self-index
				if (symbol.Name == "this[]") return true;
			}
			return false;
		}

		private bool HasClassAnyMethodWithSameName(IFieldSymbol symbol)
		{
			//The ContainingType of field symbol is class 
			var methods = symbol.ContainingType.GetMembers().OfType<IMethodSymbol>().ToList();
			return methods.Any(it => it.Name == symbol.Name.ToPascalCaseId()) ||
				methods.Any(it => "get_" + symbol.Name.ToPascalCaseId() == it.Name && it.MethodKind == MethodKind.PropertyGet) ||
				methods.Any(it => "set_" + symbol.Name.ToPascalCaseId() == it.Name && it.MethodKind == MethodKind.PropertySet);
		}

		private bool HasClassAnyPropertyWithSameName(IFieldSymbol symbol)
		{
			//The ContainingType of field symbol is class 
			var properties = symbol.ContainingType.GetMembers().OfType<IFieldSymbol>().ToList();
			return properties.Any(it => it.Name == symbol.Name.ToPascalCaseId());
		}
	}
}
