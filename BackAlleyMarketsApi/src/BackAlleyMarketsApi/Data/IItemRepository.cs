using BackAlleyMarketsApi.Models;

public interface IItemRepository
{
    /// <exception cref="DuplicateItemException">An item with this name already exists.</exception>
    Task<Item?> CreateItemAsync(CreateItemRequest request);
}
