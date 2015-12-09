using FluentValidation;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using FluentValidation.Validators;
using FluentValidation.Resources;

namespace Web.Models
{
    [Validator(typeof(CustomerMessageValidator))]
    public class CustomerMessage
    {
        public CustomerMessage()
        {
            MinLength = 3;
        }

        public int MinLength { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
    }

    public class CustomerMessageValidator : AbstractValidator<CustomerMessage>
    {
        public CustomerMessageValidator()
        {
            RuleFor(m => m.Name)
                .NotEmpty()
                .Must(n => n == null || n.StartsWith("A")).WithMessage("Must start with an 'A'")
                ;

            RuleFor(m => m.Email)
                .NotEmpty()
                .EmailAddress()
                ;

            RuleFor(m => m.Message)
                .NotEmpty()
                ;
        }
    }
}