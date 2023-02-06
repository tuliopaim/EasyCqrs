namespace EasyCqrs.Results;

public class Error : IEquatable<Error>
{
    public static Error None = new(string.Empty);

    public Error(string message)
    {
        ArgumentNullException.ThrowIfNull(message);

        Message = message;
    }

    public string Message { get; }

    public static implicit operator Error(string message) => new(message);
    public static implicit operator string(Error error) => new(error.Message);

    public bool Equals(Error? other)
    {
        return other?.Message == Message;
    }

    public static bool operator ==(Error? a, Error? b)
    {
        if (a is null && b is null) return true;
        if (a is null || b is null) return false;

        return a.Equals(b);
    }

    public static bool operator !=(Error? a, Error? b)
    {
        return !(a == b);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Error);
    }

    public override int GetHashCode()
    {
        return Message.GetHashCode();
    }
}
