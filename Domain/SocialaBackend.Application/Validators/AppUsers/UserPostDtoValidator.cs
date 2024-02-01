using FluentValidation;
using SocialaBackend.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Validators.Users
{
    public class UserPostDtoValidator:AbstractValidator<AppUserRegisterDto>
    {
        private const string REQUIRED_MESS = "{PropertyName} is required!";
        private const string REGEX_MESS = "{PropertyName} may only contain letters!";
        private const string MINLENGTH_MESS = "{PropertyName} must be at least {MinLength} characters long!";
        private const string MAXLENGTH_MESS = "{PropertyName} must be less {MaxLength} characters!";
        public UserPostDtoValidator()
        {
            RuleFor(a => a.Name)
                .NotEmpty().WithMessage(REQUIRED_MESS)
                .MaximumLength(50).WithMessage(MAXLENGTH_MESS)
                .MinimumLength(2).WithMessage(MINLENGTH_MESS)
                .Matches("^[a-zA-Z]+$").WithMessage(REGEX_MESS);
            RuleFor(a => a.Surname)
                .NotEmpty().WithMessage(REQUIRED_MESS)
                .MaximumLength(100).WithMessage(MAXLENGTH_MESS)
                .MinimumLength(2).WithMessage(MINLENGTH_MESS)
                .Matches("^[a-zA-Z]+$").WithMessage(REGEX_MESS);
            RuleFor(a => a.Username)
                .NotEmpty().WithMessage(REQUIRED_MESS)
                .MaximumLength(100).WithMessage(MAXLENGTH_MESS)
                .MinimumLength(4).WithMessage(MINLENGTH_MESS)
                .Matches("^[a-zA-Z0-9]+$").WithMessage(REGEX_MESS + " or digits!");
            RuleFor(a => a.Password)
                .NotEmpty().WithMessage(REQUIRED_MESS)
                .MaximumLength(100).WithMessage(MAXLENGTH_MESS)
                .MinimumLength(8).WithMessage(MINLENGTH_MESS)
                .Matches("^(?=.*[A-Za-z])(?=.*\\d)(?=.*[@$!%*#?&])[A-Za-z\\d@$!%*#. ,?&]{8,}$").WithMessage("The password must contain at least one letter, one digit, and one uppercase letter!");
        }
    }
}
