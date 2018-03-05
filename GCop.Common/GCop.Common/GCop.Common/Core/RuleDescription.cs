namespace GCop.Common.Core
{
    using Microsoft.CodeAnalysis;

    public class RuleDescription
    {
        public static readonly RuleDescription Empty = default(RuleDescription);

        public string ID, Title, Message = "{0}";

        public Category Category;

        public DiagnosticSeverity Severity = DiagnosticSeverity.Error;

        public bool IsEnabledByDefault = true;

        public RuleDescription ChangeSeverity(DiagnosticSeverity severity)
        {
            Severity = severity;
            return this;
        }

        public RuleDescription ChangeID(string id)
        {
            ID = id;
            return this;
        }

        public RuleDescription ChangeTitle(string title)
        {
            Title = title;
            return this;
        }

        public RuleDescription ChangeMessage(string message)
        {
            Message = message;
            return this;
        }

        public RuleDescription ChangeCategory(Category category)
        {
            Category = category;
            return this;
        }

        public DiagnosticDescriptor Create()
        {
            return new DiagnosticDescriptor($"GCop{ID}", Title.Or(Message), Message, Category.ToString(), Severity, IsEnabledByDefault);
        }
    }
}
