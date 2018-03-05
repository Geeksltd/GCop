namespace GCop.Conditional.Core.Syntax
{
    using Microsoft.CodeAnalysis;
    using System.Collections.Generic;
    using System.Linq;

    public class ValidationResult
    {
        public static ValidationResult Ok = new ValidationResult();
        private IList<ValidationError> _errors;
        public ValidationResult()
        {
            _errors = new List<ValidationError>();
        }

        public ValidationResult(params ValidationError[] errors)
        {
            _errors = errors.ToList();
        }
        public bool IsValid => _errors.None();

        public IEnumerable<ValidationError> Errors => _errors;


        public void AddError(ValidationError error)
        {
            _errors.Add(error);
        }

        public static ValidationResult Error(params ValidationError[] errors) => new ValidationResult(errors);
    }

    public class ValidationError
    {
        public string Message { get; set; }
        public Location ErrorLocation { get; set; }
    }
}
