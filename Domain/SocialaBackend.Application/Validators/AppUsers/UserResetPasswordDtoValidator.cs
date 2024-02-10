using FluentValidation;
using SocialaBackend.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Validators
{
    public class UserResetPasswordDtoValidator: AbstractValidator<AppUserResetPasswordDto>
    {
        private const string REQUIRED_MESS = "{PropertyName} is required!";
        private const string REGEX_MESS = "{PropertyName} may only contain letters!";
        private const string MINLENGTH_MESS = "{PropertyName} must be at least {MinLength} characters long!";
        private const string MAXLENGTH_MESS = "{PropertyName} must be less {MaxLength} characters!";
        public UserResetPasswordDtoValidator()
        {
            RuleFor(a => a.Email)
            .NotEmpty().WithMessage(REQUIRED_MESS)
            .MaximumLength(256).WithMessage(MAXLENGTH_MESS)
            .MinimumLength(4).WithMessage(MINLENGTH_MESS)
            .Matches("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$").WithMessage("Incorrect email!");
            RuleFor(a => a.Password)
                .NotEmpty().WithMessage(REQUIRED_MESS)
                .MaximumLength(100).WithMessage(MAXLENGTH_MESS)
                .MinimumLength(8).WithMessage(MINLENGTH_MESS)
                .Matches("^(?=.*[A-Za-z])(?=.*\\d)(?=.*[@$!%*#?&])[A-Za-z\\d@$!%*#. ,?&]{8,}$").WithMessage("The password must contain at least one letter, one digit, and one uppercase letter!");
        }
    }
}
