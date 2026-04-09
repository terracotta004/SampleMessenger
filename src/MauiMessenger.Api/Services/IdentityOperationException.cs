namespace MauiMessenger.Api.Services;

public sealed class IdentityOperationException : Exception
{
    public IdentityOperationException(IReadOnlyList<string> errors)
        : base(errors.Count > 0 ? string.Join(Environment.NewLine, errors) : "Identity operation failed.")
    {
        Errors = errors;
    }

    public IReadOnlyList<string> Errors { get; }
}
