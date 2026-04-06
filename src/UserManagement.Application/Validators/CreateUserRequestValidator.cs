using FluentValidation;
using UserManagement.Application.DTOs;
using UserManagement.Domain.ValueObjects;

namespace UserManagement.Application.Validators;

public sealed class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(Name.MinLength).WithMessage($"Name must have at least {Name.MinLength} characters.")
            .MaximumLength(Name.MaxLength).WithMessage($"Name cannot exceed {Name.MaxLength} characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(Email.MaxLength).WithMessage($"Email cannot exceed {Email.MaxLength} characters.");
    }
}

