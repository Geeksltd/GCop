namespace GCop.ErrorHandling.Core.Attributes
{
    using System;
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SupportMultipleDiagnosticAttribute : Attribute
    {
    }
}
