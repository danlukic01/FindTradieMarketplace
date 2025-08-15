// ===== VALIDATION =====
// Validators/CreateTradieProfileRequestValidator.cs
using FluentValidation;
using FindTradie.Services.TradieManagement.DTOs;

namespace FindTradie.Services.TradieManagement.Validators;

public class CreateTradieProfileRequestValidator : AbstractValidator<CreateTradieProfileRequest>
{
    public CreateTradieProfileRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.BusinessName)
            .NotEmpty().WithMessage("Business name is required")
            .MaximumLength(200).WithMessage("Business name cannot exceed 200 characters");

        RuleFor(x => x.ABN)
            .NotEmpty().WithMessage("ABN is required")
            .Matches(@"^\d{11}$").WithMessage("ABN must be 11 digits");

        RuleFor(x => x.ACN)
            .Matches(@"^\d{9}$").WithMessage("ACN must be 9 digits")
            .When(x => !string.IsNullOrEmpty(x.ACN));

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters");

        RuleFor(x => x.HourlyRate)
            .GreaterThan(0).WithMessage("Hourly rate must be greater than 0")
            .LessThanOrEqualTo(1000).WithMessage("Hourly rate cannot exceed $1000/hour");

        RuleFor(x => x.ServiceRadius)
            .GreaterThan(0).WithMessage("Service radius must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Service radius cannot exceed 100km");

        RuleFor(x => x.ServiceCategories)
            .NotEmpty().WithMessage("At least one service category is required")
            .Must(x => x.Count <= 5).WithMessage("Cannot exceed 5 service categories");

        RuleFor(x => x.InsuranceExpiryDate)
            .GreaterThan(DateTime.Today).WithMessage("Insurance must not be expired")
            .When(x => x.InsuranceExpiryDate.HasValue);
    }
}

public class UpdateTradieProfileRequestValidator : AbstractValidator<UpdateTradieProfileRequest>
{
    public UpdateTradieProfileRequestValidator()
    {
        RuleFor(x => x.BusinessName)
            .NotEmpty().WithMessage("Business name is required")
            .MaximumLength(200).WithMessage("Business name cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters");

        RuleFor(x => x.HourlyRate)
            .GreaterThan(0).WithMessage("Hourly rate must be greater than 0")
            .LessThanOrEqualTo(1000).WithMessage("Hourly rate cannot exceed $1000/hour");

        RuleFor(x => x.ServiceRadius)
            .GreaterThan(0).WithMessage("Service radius must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Service radius cannot exceed 100km");
    }
}

public class SearchTradiesRequestValidator : AbstractValidator<SearchTradiesRequest>
{
    public SearchTradiesRequestValidator()
    {
        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180");

        RuleFor(x => x.RadiusKm)
            .GreaterThan(0).WithMessage("Radius must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Radius cannot exceed 100km");

        RuleFor(x => x.MinRating)
            .InclusiveBetween(0, 5).WithMessage("Rating must be between 0 and 5")
            .When(x => x.MinRating.HasValue);

        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50).WithMessage("Page size must be between 1 and 50");
    }
}
