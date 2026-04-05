namespace MacroMission.Domain.Common;

/// <summary>Aggregates multiple field-level validation errors into a single Error.</summary>
public sealed record ValidationError(Error[] Errors)
    : Error("Validation.Failed", "One or more validation errors occurred.", ErrorType.Validation);
