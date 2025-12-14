using FluentValidation;
using API.DebugDojo.Application.Contracts;

namespace API.DebugDojo.Application.Validators;

public sealed class WorkItemCreateRequestValidator : AbstractValidator<WorkItemCreateRequest>
{
    public WorkItemCreateRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000);
    }
}

public sealed class WorkItemUpdateRequestValidator : AbstractValidator<WorkItemUpdateRequest>
{
    public WorkItemUpdateRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000);
    }
}

public sealed class WorkItemStatusUpdateRequestValidator : AbstractValidator<WorkItemStatusUpdateRequest>
{
    public WorkItemStatusUpdateRequestValidator()
    {
        RuleFor(x => x.RowVersion).NotNull().WithMessage("RowVersion es obligatoria.");
    }
}

public sealed class WorkItemQueryValidator : AbstractValidator<WorkItemQuery>
{
    public WorkItemQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.Sort).NotEmpty();
    }
}
