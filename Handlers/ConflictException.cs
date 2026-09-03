namespace MyWebApp.Handlers;

public class ConflictException : Exception
{
    public ConflictException(string message)
        : base(message)
    {
    }
}