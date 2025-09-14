namespace PollsAppBlazor.Shared.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
    AllowMultiple = false)]
public class DateTimeOffsetRangeAttribute : ValidationAttribute
{
    public TimeSpan Min { get; private set; }
    public TimeSpan Max { get; private set; }

    public DateTimeOffsetRangeAttribute(long minTicks, long maxTicks, string? errorMessage = null) :
        base(errorMessage ?? "Date is invalid")
    {
        Min = TimeSpan.FromTicks(minTicks);
        Max = TimeSpan.FromTicks(maxTicks);
    }

    public override bool IsValid(object? value)
    {
        if (value is not DateTimeOffset date) return true;

        TimeSpan time = date - DateTimeOffset.Now;
        return time >= Min && time <= Max;
    }
}
