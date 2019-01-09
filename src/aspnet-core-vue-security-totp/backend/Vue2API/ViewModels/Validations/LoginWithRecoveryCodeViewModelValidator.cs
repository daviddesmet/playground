namespace Vue2API.ViewModels.Validations
{
    using FluentValidation;

    public class LoginWithRecoveryCodeViewModelValidator : AbstractValidator<LoginWithRecoveryCodeViewModel>
    {
        public LoginWithRecoveryCodeViewModelValidator()
        {
            RuleFor(vm => vm.RecoveryCode).NotEmpty().WithMessage("Recovery Code cannot be empty");
        }
    }
}
