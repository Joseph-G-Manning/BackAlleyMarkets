public class DuplicateItemException : Exception
{
    public DuplicateItemException(string itemName)
        : base($"An item named '{itemName}' already exists.") { }
}
