// ===== VALIDATION =====
// Validators/JobValidators.cs
using FluentValidation;
using FindTradie.Services.JobManagement.DTOs;
using System;

namespace FindTradie.Services.JobManagement.Validators;

public class CreateJobRequestValidator : AbstractValidator<CreateJobRequest>
{
    public CreateJobRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Job title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Job description is required")
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters");

        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required");

        RuleFor(x => x.CustomerName)
            .NotEmpty().WithMessage("Customer name is required")
            .MaximumLength(100).WithMessage("Customer name cannot exceed 100 characters");

        RuleFor(x => x.CustomerEmail)
            .NotEmpty().WithMessage("Customer email is required")
            .EmailAddress().WithMessage("Invalid email address")
            .MaximumLength(255).WithMessage("Email cannot exceed 255 characters");

        RuleFor(x => x.CustomerPhone)
            .NotEmpty().WithMessage("Customer phone is required")
            .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required")
            .MaximumLength(200).WithMessage("Address cannot exceed 200 characters");

        RuleFor(x => x.Suburb)
            .NotEmpty().WithMessage("Suburb is required")
            .MaximumLength(100).WithMessage("Suburb cannot exceed 100 characters");

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("State is required")
            .MaximumLength(50).WithMessage("State cannot exceed 50 characters");

        RuleFor(x => x.PostCode)
            .NotEmpty().WithMessage("Post code is required")
            .Matches(@"^\d{4}$").WithMessage("Post code must be 4 digits");

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180");

        When(x => x.BudgetMin.HasValue && x.BudgetMax.HasValue, () =>
        {
            RuleFor(x => x.BudgetMin)
                .LessThanOrEqualTo(x => x.BudgetMax)
                .WithMessage("Minimum budget cannot be greater than maximum budget");
        });

        RuleFor(x => x.PreferredStartDate)
            .GreaterThanOrEqualTo(DateTime.Today)
            .WithMessage("Preferred start date cannot be in the past")
            .When(x => x.PreferredStartDate.HasValue);

        RuleFor(x => x.PreferredEndDate)
            .GreaterThan(x => x.PreferredStartDate)
            .WithMessage("Preferred end date must be after start date")
            .When(x => x.PreferredStartDate.HasValue && x.PreferredEndDate.HasValue);

        RuleFor(x => x.ImageUrls)
            .Must(x => x == null || x.Count <= 10)
            .WithMessage("Cannot exceed 10 images per job");
    }
}

public class CreateQuoteRequestValidator : AbstractValidator<CreateQuoteRequest>
{
    public CreateQuoteRequestValidator()
    {
        RuleFor(x => x.JobId)
            .NotEmpty().WithMessage("Job ID is required");

        RuleFor(x => x.TradieId)
            .NotEmpty().WithMessage("Tradie ID is required");

        RuleFor(x => x.MaterialsCost)
            .GreaterThanOrEqualTo(0).WithMessage("Materials cost cannot be negative");

        RuleFor(x => x.LabourCost)
            .GreaterThan(0).WithMessage("Labour cost must be greater than 0");

        RuleFor(x => x.EstimatedDurationHours)
            .GreaterThan(0).WithMessage("Estimated duration must be greater than 0 hours")
            .LessThanOrEqualTo(8760).WithMessage("Estimated duration cannot exceed 1 year");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Quote description is required")
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters");

        RuleFor(x => x.ProposedStartDate)
            .GreaterThanOrEqualTo(DateTime.Today)
            .WithMessage("Proposed start date cannot be in the past")
            .When(x => x.ProposedStartDate.HasValue);

        RuleFor(x => x.ProposedEndDate)
            .GreaterThan(x => x.ProposedStartDate)
            .WithMessage("Proposed end date must be after start date")
            .When(x => x.ProposedStartDate.HasValue && x.ProposedEndDate.HasValue);

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("At least one quote item is required")
            .Must(x => x.Count <= 50).WithMessage("Cannot exceed 50 quote items");

        RuleForEach(x => x.Items).SetValidator(new CreateQuoteItemRequestValidator());
    }
}

public class CreateQuoteItemRequestValidator : AbstractValidator<CreateQuoteItemRequest>
{
    public CreateQuoteItemRequestValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Item description is required")
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0");

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0).WithMessage("Unit price must be greater than 0");
    }
}