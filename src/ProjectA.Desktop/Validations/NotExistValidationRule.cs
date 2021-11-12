using System.Globalization;
using System.Windows.Controls;
using ProjectA.Core.Models.DocAggregate;
using ProjectA.Core.Models.DocAggregate.Specifications;
using ProjectA.SharedKernel.Interfaces;

namespace ProjectA.Desktop.Validations
{
    public class NotExistValidationRule : ValidationRule
    {
        private readonly IRepository<Document> _repository;

        public NotExistValidationRule(IRepository<Document> repository)
        {
            _repository = repository;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is not int entityId) return new ValidationResult(false, "Field is required.");
            
            var spec = new DocumentByEntityIdSpec(entityId);
            var document = _repository.GetBySpecAsync(spec).GetAwaiter().GetResult();
            return document == null
                ? ValidationResult.ValidResult
                : new ValidationResult(false, $"{entityId} already exist.");
        }
    }
}