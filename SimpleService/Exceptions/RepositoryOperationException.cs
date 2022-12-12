namespace SimpleService.Exceptions;

public class RepositoryOperationException : Exception
{
    public RepositoryOperationException()
    {
    }

    public RepositoryOperationException(string message) : base(message)
    {
    }

    public RepositoryOperationException(string message, Exception exception) : base(message, exception) 
    {
    }
}