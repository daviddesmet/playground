namespace Vue2API.ViewModels.Validations
{
    using FluentValidation;

    public class ChangePasswordViewModelValidator : AbstractValidator<ChangePasswordViewModel>
    {
        public ChangePasswordViewModelValidator()
        {
            RuleFor(vm => vm.OldPassword).NotEmpty().WithMessage("Current password cannot be empty");
            RuleFor(vm => vm.NewPassword).NotEmpty().WithMessage("New password cannot be empty");
            RuleFor(vm => vm.NewPassword).NotEqual(_ => _.OldPassword).WithMessage("New password must be different");
            RuleFor(vm => vm.NewPassword).Length(6, 100).WithMessage("Password must be at least 6 characters long.");
            //RuleFor(vm => vm.ConfirmPassword).NotEmpty().WithMessage("Confirm new password cannot be empty");
            RuleFor(vm => vm.ConfirmPassword).Equal(_ => _.NewPassword).WithMessage("New password do not match");
        }
    }
}