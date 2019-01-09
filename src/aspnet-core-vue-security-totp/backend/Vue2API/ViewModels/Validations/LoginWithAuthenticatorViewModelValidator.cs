namespace Vue2API.ViewModels.Validations
{
    using FluentValidation;

    public class LoginWithAuthenticatorViewModelValidator : AbstractValidator<LoginWithAuthenticatorViewModel>
    {
        public LoginWithAuthenticatorViewModelValidator()
        {
            RuleFor(vm => vm.Code).NotEmpty().WithMessage("Verification code cannot be empty");
        }
    }
}