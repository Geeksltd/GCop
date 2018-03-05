namespace GCop.Linq.Core
{
    using Core.Attributes;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;

    public abstract class Rule : DiagnosticAnalyzer
    {
        public DiagnosticDescriptor Description;
        internal AnalysisContext Context { get; private set; }

        protected Rule()
        {
            Description = GetDescription().Create();
        }

        public sealed override void Initialize(AnalysisContext context)
        {
            Context = context;

            try
            {
                Configure();
            }
            catch (Exception exp)
            {
                Logger.Log(exp, "Rule initilization error!");
            }
        }

        protected abstract RuleDescription GetDescription();

        protected virtual IEnumerable<RuleDescription> GetDescriptions()
        {
            yield return RuleDescription.Empty;
        }

        protected abstract void Configure();

        public bool SupportMultiDiagnostic => GetType().GetCustomAttributes(typeof(SupportMultipleDiagnosticAttribute), inherit: true).Any();

        public sealed override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Description);
    }
}
