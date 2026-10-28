using System.Data;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using BackAlleyMarketsApi.Models;

public class ItemRepository : IItemRepository
{
    // Loaded once when the class is first used, then reused
    private static readonly string PostItemSql = SqlLoader.Load("PostItem.sql");

    private readonly string _connectionString;

    public ItemRepository(IConfiguration config)
    {
        _connectionString =
            config.GetConnectionString("BackAlleyMarkets")
            ?? throw new InvalidOperationException("Missing connection string.");
    }

    public async Task<Item?> CreateItemAsync(CreateItemRequest request)
    {
        var tagsJson = JsonSerializer.Serialize(request.ItemTags ?? new List<string>());

        try
        {
            await using var conn = new SqlConnection(_connectionString);
            await using var cmd = new SqlCommand(PostItemSql, conn);

            cmd.Parameters.AddWithValue("@ItemName", request.ItemName.Trim());
            cmd.Parameters.AddWithValue("@Description", request.ItemDescription ?? string.Empty);
            cmd.Parameters.AddWithValue("@CategoryName", request.CategoryName.Trim());
            cmd.Parameters.Add("@Tags", SqlDbType.NVarChar, -1).Value = tagsJson;

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return null;
            }

            var item = new Item
            {
                ItemId = reader.GetInt32(reader.GetOrdinal("ItemId")),
                ItemName = reader.GetString(reader.GetOrdinal("ItemName")),
                Description = reader.GetString(reader.GetOrdinal("Description")),
                CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId")),
            };

            // Second result set: the tags
            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    item.Tags.Add(
                        new ItemTag
                        {
                            TagId = reader.GetInt32(reader.GetOrdinal("TagId")),
                            TagName = reader.GetString(reader.GetOrdinal("TagName")),
                        }
                    );
                }
            }

            return item;
        }
        catch (SqlException ex) when (ex.Number == 50001)
        {
            throw new DuplicateItemException(request.ItemName);
        }
    }
}
