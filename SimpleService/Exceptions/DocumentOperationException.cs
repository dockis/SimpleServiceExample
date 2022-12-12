namespace SimpleService.Exceptions;

public class DocumentOperationException : Exception
{
    public DocumentOperationException()
    {
    }

    public DocumentOperationException(string message) : base(message)
    {
    }

    public DocumentOperationException(string message, Exception exception) : base(message, exception) 
    {
    }
}