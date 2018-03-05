namespace GCop.Conditional.Core
{
    using GCop.Conditional.Core.Attributes;
    using GCop.Conditional.Core.Watch;
    using GCop.Conditional.Utilities;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System;
    using System.Collections.Immutable;
    using System.Diagnostics;
    using System.Linq;

    public abstract class GCopAnalyzer<TContext, TKind> : GCopAnalyzer where TContext : struct
    {
        protected abstract TKind Kind
        {
            get;
        }

        protected abstract void Analyze(TContext context);

        protected sealed override void Configure()
        {
            if (typeof(TContext) == typeof(SyntaxNodeAnalysisContext))
            {
                RegisterSyntaxNodeAction(x => Analyze((TContext)(object)x), (SyntaxKind)(object)Kind);
            }
            else if (typeof(TContext) == typeof(SymbolAnalysisContext))
            {
                RegisterSymbolAction(x => Analyze((TContext)(object)x), (SymbolKind)(object)Kind);
            }
            else
            {
                throw new NotImplementedException($"GCopAnaylzer.Configure() is not implemented for '{typeof(TContext).Name}' yet!");
            }
        }
    }

    public abstract class GCopAnalyzer : Rule
    {
        static DateTime? FirstInvocation;
        static int? Minute;

        protected internal SyntaxNode NodeToAnalyze { get; protected set; }

        #region Register Analyzer 

        /// <summary>
        /// Register an action to be executed after semantic analysis of a method body or
        /// an expression appearing outside a method body. A code block action reports Microsoft.CodeAnalysis.Diagnostics
        /// about code blocks.
        /// </summary>
        public void RegisterCodeBlockAction(Action<CodeBlockAnalysisContext> action)
        {
            Context.RegisterCodeBlockAction(c =>
            {
                Run(() => action.Invoke(c));
            });
        }

        /// <summary>
        /// Register an action to be executed at the start of semantic analysis of a method
        /// body or an expression appearing outside a method body. A code block start action
        /// can register other actions and/or collect state information to be used in diagnostic
        /// analysis, but cannot itself report any Microsoft.CodeAnalysis.Diagnostics.
        /// </summary>
        public void RegisterCodeBlockStartAction<TLanguageKindEnum>(Action<CodeBlockStartAnalysisContext<TLanguageKindEnum>> action) where TLanguageKindEnum : struct
        {
            Context.RegisterCodeBlockStartAction<TLanguageKindEnum>(c =>
            {
                Run(() => action?.Invoke(c));
            });
        }

        /// <summary>
        /// Register an action to be executed for a complete compilation. A compilation action
        /// reports Microsoft.CodeAnalysis.Diagnostics about the Microsoft.CodeAnalysis.Compilation.
        /// </summary>
        public void RegisterCompilationAction(Action<CompilationAnalysisContext> action)
        {
            Context.RegisterCompilationAction(c =>
            {
                Run(() => action?.Invoke(c));
            });
        }

        /// <summary>
        /// Register an action to be executed at compilation start. A compilation start action
        /// can register other actions and/or collect state information to be used in diagnostic
        /// analysis, but cannot itself report any Microsoft.CodeAnalysis.Diagnostics.
        /// </summary>
        /// <param name = "action"> describe action parameter on RegisterCompilationStartAction</param>
        public void RegisterCompilationStartAction(Action<CompilationStartAnalysisContext> action)
        {
            Context.RegisterCompilationStartAction(c =>
            {
                Run(() => action?.Invoke(c));
            });
        }

        /// <summary>
        /// Register an action to be executed at completion of semantic analysis of a document,
        /// which will operate on the Microsoft.CodeAnalysis.SemanticModel of the document.
        /// A semantic model action reports Microsoft.CodeAnalysis.Diagnostics about the
        /// model.
        /// </summary>
        /// <param name = "action"> describe action parameter on RegisterSemanticModelAction</param>
        public void RegisterSemanticModelAction(Action<SemanticModelAnalysisContext> action)
        {
            Context.RegisterSemanticModelAction(c =>
            {
                Run(() => action?.Invoke(c));
            });
        }

        /// <summary>
        /// Register an action to be executed at completion of semantic analysis of an Microsoft.CodeAnalysis.ISymbol
        /// with an appropriate Kind.> A symbol action reports Microsoft.CodeAnalysis.Diagnostics
        /// about Microsoft.CodeAnalysis.ISymbols.
        /// </summary>
        public void RegisterSymbolAction(Action<SymbolAnalysisContext> action, ImmutableArray<SymbolKind> symbolKinds)
        {
            if (Context == null) return;
            Context?.RegisterSymbolAction(c =>
            {
                Run(() => action?.Invoke(c));
            }, symbolKinds);
        }

        /// <summary>
        /// Register an action to be executed at completion of semantic analysis of an Microsoft.CodeAnalysis.ISymbol
        /// with an appropriate Kind.> A symbol action reports Microsoft.CodeAnalysis.Diagnostics
        /// about Microsoft.CodeAnalysis.ISymbols.
        /// </summary>
        public void RegisterSymbolAction(Action<SymbolAnalysisContext> action, params SymbolKind[] symbolKinds)
        {
            Context.RegisterSymbolAction(c =>
            {
                Run(() => action?.Invoke(c));
            }, symbolKinds);
        }

        /// <summary>
        /// Register an action to be executed at completion of semantic analysis of a Microsoft.CodeAnalysis.SyntaxNode
        /// with an appropriate Kind. A syntax node action can report Microsoft.CodeAnalysis.Diagnostics
        /// about Microsoft.CodeAnalysis.SyntaxNodes, and can also collect state information
        /// to be used by other syntax node actions or code block end actions.
        /// </summary>
        public void RegisterSyntaxNodeAction<TLanguageKindEnum>(Action<SyntaxNodeAnalysisContext> action, ImmutableArray<TLanguageKindEnum> syntaxKinds) where TLanguageKindEnum : struct
        {
            Context.RegisterSyntaxNodeAction(c =>
            {
                if (HasEscapeGcopAttribute(c.SemanticModel, c.Node)) return;
                Run(() => action?.Invoke(c));
            }, syntaxKinds);
        }

        /// <summary>
        /// Register an action to be executed at completion of semantic analysis of a Microsoft.CodeAnalysis.SyntaxNode
        /// with an appropriate Kind. A syntax node action can report Microsoft.CodeAnalysis.Diagnostics
        /// about Microsoft.CodeAnalysis.SyntaxNodes, and can also collect state information
        /// to be used by other syntax node actions or code block end actions.
        /// </summary>
        public void RegisterSyntaxNodeAction<TLanguageKindEnum>(Action<SyntaxNodeAnalysisContext> action, params TLanguageKindEnum[] syntaxKinds) where TLanguageKindEnum : struct
        {
            Context.RegisterSyntaxNodeAction(c =>
            {
                if (HasEscapeGcopAttribute(c.SemanticModel, c.Node)) return;
                Run(() => action?.Invoke(c));
            }, syntaxKinds);
        }

        /// <summary>
        /// Register an action to be executed at completion of parsing of a code document.
        /// A syntax tree action reports Microsoft.CodeAnalysis.Diagnostics about the Microsoft.CodeAnalysis.SyntaxTree
        /// of a document.
        /// </summary>
        public void RegisterSyntaxTreeAction(Action<SyntaxTreeAnalysisContext> action)
        {
            Context.RegisterSyntaxTreeAction(c =>
            {
                Run(() => action?.Invoke(c));
            });
        }

        public void RegisterSyntaxNodeActionForVersionLower<TLanguageKindEnum>(Action<SyntaxNodeAnalysisContext> action, LanguageVersion lowerThanLanguageVersion = LanguageVersion.CSharp6, params TLanguageKindEnum[] syntaxKinds) where TLanguageKindEnum : struct
        {
            Context.RegisterCompilationStartAction(c =>
            {
                Context.RegisterCompilationStartActionForVersionLower(lowerThanLanguageVersion, compilationContext => compilationContext.RegisterSyntaxNodeAction(action, syntaxKinds));
            });
        }

        #endregion

        async void Run(Action action)
        {
            var name = GetType().Name;

            try
            {
                var timer = new Stopwatch();

                timer.Start();

                action?.Invoke();

                timer.Stop();

            }
            catch (Exception error)
            {
                Logger.Log(error);
                await DiagnosticsHandler.Instance.SendError(error, name, NodeToAnalyze, DateTime.Now);
            }
        }

        bool HasEscapeGcopAttribute(SemanticModel semanticModel, SyntaxNode node)
        {
            if (node == null || GetType().Name == "EscapeGCopAttributeAnalyzer") return false;
            MethodDeclarationSyntax method = null;
            if (node.IsKind(SyntaxKind.MethodDeclaration))
                method = node.As<MethodDeclarationSyntax>();
            else
            {
                method = node.GetSingleAncestor<MethodDeclarationSyntax>();
            }
            var result = method?.AttributeLists.Any(it => it.Attributes.Any(attr => attr.Name.ToString() == "EscapeGCop")) ?? false;
            if (!result)
            {
                if (node.IsKind(SyntaxKind.ClassDeclaration))
                {
                    var mainClass = node as ClassDeclarationSyntax;
                    result = mainClass.AttributeLists.Any(it => it.Attributes.Any(attr => attr.Name.ToString() == "EscapeGCop"));
                    if (!result)
                    {
                        result = semanticModel.GetSymbolInfo(node).Symbol.HasAttribute("EscapeGCop");
                    }
                }
                else if (!result)
                {
                    var @class = node.GetSingleAncestor<ClassDeclarationSyntax>();
                    if (@class != null)
                    {
                        result = @class.AttributeLists.Any(it => it.Attributes.Any(attr => attr.Name.ToString() == "EscapeGCop"));
                        if (!result)
                        {
                            result = semanticModel.GetSymbolInfo(node).Symbol.HasAttribute("EscapeGCop");
                        }
                    }
                }
                else
                {
                    var @struct = node.GetSingleAncestor<StructDeclarationSyntax>();
                    result = @struct?.AttributeLists.Any(it => it.Attributes.Any(attr => attr.Name.ToString() == "EscapeGCop")) ?? false;
                    if (!result)
                    {
                        result = semanticModel.GetSymbolInfo(node).Symbol.HasAttribute("EscapeGCop");
                    }
                }
            }
            return result;
        }

        protected void Await()
        {
            if (FirstInvocation == null)
                FirstInvocation = LocalTime.Now;
        }

        protected bool CanContinue
        {
            get
            {
                var minute = Minute ?? GetType().GetCustomAttributes(typeof(DelayAttribute), inherit: false).OfType<DelayAttribute>().FirstOrDefault()?.Minute ?? 0;
                return FirstInvocation.HasValue ? LocalTime.Now.Subtract(FirstInvocation.Value).TotalMinutes > minute : true;
            }
        }

        protected void ResetAnalyzer() => FirstInvocation = null;

        protected void ReportDiagnostic(SyntaxNodeAnalysisContext context, SyntaxNodeOrToken element, params string[] messageArgs)
        {
            ReportDiagnostic(context, element.GetLocation(), messageArgs);
        }

        protected void ReportDiagnostic(SyntaxNodeAnalysisContext context, Location location, params string[] messageArgs)
        {
            if (messageArgs.Any())
                context.ReportDiagnostic(Diagnostic.Create(Description, location, messageArgs));
            else
                context.ReportDiagnostic(Diagnostic.Create(Description, location));
        }

        protected void ReportDiagnostic(SyntaxNodeAnalysisContext context, RuleDescription description, Location location)
        {
            context.ReportDiagnostic(Diagnostic.Create(description.Create(), location));
        }
    }
}
