namespace Assignment.Counters.Application.Exceptions;

public class EntryAlreadyExistsException<T> : Exception
{
    public EntryAlreadyExistsException() : base("entry_exists")
    {
    }

    public Type EntryType => typeof(T);   
}