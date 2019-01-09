namespace Vue2API.ViewModels.Validations
{
    using FluentValidation;

    public class AuthenticatorViewModelValidator : AbstractValidator<AuthenticatorViewModel>
    {
        public AuthenticatorViewModelValidator()
        {
            RuleFor(vm => vm.Code).NotEmpty().WithMessage("Verification Code cannot be empty");
            RuleFor(vm => vm.Code).Length(6, 7).WithMessage("Verification Code must be between 6 and 7 characters");
        }
    }
}
