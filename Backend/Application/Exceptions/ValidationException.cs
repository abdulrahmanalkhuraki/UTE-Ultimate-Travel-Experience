using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Exceptions
{
    public class ValidationException : Exception
    {
        public Dictionary<string, string[]> Errors { get; }
        
        public ValidationException() 
            : base("Validation failed")
        {
            Errors = new Dictionary<string, string[]>();
        }
        
        public ValidationException(string message) 
            : base(message)
        {
            Errors = new Dictionary<string, string[]>();
        }
        
        public ValidationException(string message, Exception innerException) 
            : base(message, innerException)
        {
            Errors = new Dictionary<string, string[]>();
        }
        
        public ValidationException(IEnumerable<ValidationFailure> failures)
            : base("Validation failed")
        {
            Errors = failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(failureGroup => failureGroup.Key, 
                              failureGroup => failureGroup.ToArray());
        }
        
        // Additional constructor for string list
        public ValidationException(List<string> errors)
            : base("Validation failed")
        {
            Errors = new Dictionary<string, string[]>();
            if (errors != null && errors.Any())
            {
                Errors["General"] = errors.ToArray();
            }
        }
    }
}