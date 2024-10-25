using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ClipShare.Helpers
{
    public class StringCustomValidation : ValidationAttribute
    {
        private readonly string _name;
        private readonly bool _required;
        private readonly int _minLength;
        private readonly int _maxLength;
        private readonly string _regex;
        private readonly string _regexErrorMessage;

        public StringCustomValidation(string name, bool required, int minLength, int maxLength, string regex, string regexErrorMessage)
        {
            _name = name;
            _required = required;
            _minLength = minLength;
            _maxLength = maxLength;
            _regex = regex;
            _regexErrorMessage = regexErrorMessage;
        }
        protected override ValidationResult IsValid(object prop, ValidationContext validationContext)
        {
            var value = prop as string;

            if (_required == true)
            {
                if (string.IsNullOrEmpty(value))
                {
                    return new ValidationResult($"{_name} is required");
                }
            }

            if (_minLength > 0 && !string.IsNullOrEmpty(value))
            {
                if (value.Length < _minLength)
                {
                    return new ValidationResult($"{_name} length must be more than {_minLength} characters");
                }
            }

            if (_maxLength > 0 && !string.IsNullOrEmpty(value))
            {
                if (value.Length > _maxLength)
                {
                    return new ValidationResult($"{_name} length must be less than {_maxLength} characters");
                }
            }

            if (!string.IsNullOrEmpty(_regex) && !string.IsNullOrEmpty(value))
            {
                Regex regex = new Regex(_regex);
                var result = regex.IsMatch(value);

                if (!result)
                {
                    var errorMessage = _regexErrorMessage;
                    if (errorMessage != null)
                    {
                        return new ValidationResult(errorMessage);
                    }
                    else
                    {
                        return new ValidationResult($"{_name} does not match the following regex pattern of {_regex}");
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}
