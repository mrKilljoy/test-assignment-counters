namespace Assignment.Counters.Application.Exceptions;

[Serializable]
public class EntryNotFoundException<T> : Exception
{
    public EntryNotFoundException(object id) : base("entry_not_found")
    {
        Id = id;
    }

    public Type EntryType => typeof(T);
    
    public object Id { get; private set; }   
}