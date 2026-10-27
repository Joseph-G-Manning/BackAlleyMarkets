using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Any;
using Microsoft.AspNetCore.Http.HttpResults;
using BackAlleyMarketsApi.Models;
using System.Text.Json;

namespace BackAlleyMarketsApi.Controllers;

// public record ItemDto(string ItemName, string ItemDescription, string CategoryName);
// public class CreateItemRequest
// {
//     public string ItemName { get; set; } = string.Empty;
//     public string ItemDescription { get; set; } = string.Empty;
//     public string CategoryName { get; set; } = string.Empty;
//     public List<string> ItemTags { get; set; } = new();
// }

// public class TagResponse
// {
//     public int TagId { get; set; }
//     public string TagName { get; set; } = string.Empty;
// }

[Route("api/[controller]")]
[ApiController]
public class ItemsController : ControllerBase
{
    private readonly string _connectionString;
    private IConfiguration _configuration;
    private readonly IItemRepository _items;

    public ItemsController(IConfiguration configuration, IItemRepository items)
    {
        _configuration = configuration;
        _items = items;
        _connectionString = _configuration.GetConnectionString("BackAlleyMarkets")
            ?? throw new InvalidOperationException("Missing connection string.");
    }

    [HttpGet]
    // public JsonResult GetItems()
    // {
    //     string query = "SELECT ItemId, ItemName, Description, Item.CategoryId, CategoryName FROM Item LEFT JOIN Category ON Item.CategoryId = Category.CategoryId;";
    //     DataTable table = new DataTable();
    //     string? SqlDatasource = _configuration.GetConnectionString("BackAlleyMarkets");
    //     SqlDataReader myReader;
    //     using (SqlConnection myCon = new SqlConnection(SqlDatasource))
    //     {
    //         myCon.Open();
    //         using (SqlCommand myCommand = new SqlCommand(query, myCon))
    //         {
    //             myReader = myCommand.ExecuteReader();
    //             table.Load(myReader);
    //         }
    //     }

    //     return new JsonResult(table);
    // }

    public List<Item> GetItems()
    {
        string? connectionString = _configuration.GetConnectionString("BackAlleyMarkets");
        List<Item> items = new List<Item>();

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            SqlCommand cmdItem = new SqlCommand("SELECT ItemId, ItemName, Description, Item.CategoryId, CategoryName FROM Item LEFT JOIN Category ON Item.CategoryId = Category.CategoryId;", conn);
            SqlCommand cmdTag = new SqlCommand("SELECT Item_Tag.ItemTagId, Item_Tag.ItemId, Item_Tag.TagId, Tag.TagName FROM Item_Tag FULL OUTER JOIN Item ON Item_Tag.ItemId = Item.ItemId FULL OUTER JOIN Tag ON Item_Tag.TagId = Tag.TagId;", conn);
            conn.Open();

            SqlDataReader readerTag = cmdTag.ExecuteReader();
            List<ItemTag> tempTagList = new List<ItemTag>();
            while (readerTag.Read())
            {
                // if (readerTag.GetInt32(1) == item.ItemId)
                // {
                ItemTag itemTag = new ItemTag();
                itemTag.ItemTagId = readerTag.GetInt32(0);
                itemTag.ItemId = readerTag.GetInt32(1);
                itemTag.TagId = readerTag.GetInt32(2);
                itemTag.TagName = readerTag.GetString(3);
                tempTagList.Add(itemTag);
                // }
            }
            readerTag.Close();

            SqlDataReader readerItem = cmdItem.ExecuteReader();
            while (readerItem.Read())
            {
                Item item = new Item();
                item.ItemId = readerItem.GetInt32(0); // Assuming Id is the first column
                item.ItemName = readerItem.GetString(1);
                item.Description = readerItem.GetString(2);
                item.CategoryId = readerItem.GetInt32(3);
                item.CategoryName = readerItem.GetString(4);
                foreach (var tag in tempTagList)
                {
                    if (tag.ItemId == item.ItemId)
                    {
                        item.Tags.Add(tag);
                    }
                }
                // while (readerTag.Read())
                // {
                //     if (readerTag.GetInt32(1) == item.ItemId)
                //     {
                //         ItemTag itemTag = new ItemTag();
                //         itemTag.ItemTagId = readerTag.GetInt32(0);
                //         itemTag.ItemId = readerTag.GetInt32(1);
                //         itemTag.TagId = readerTag.GetInt32(2);
                //         itemTag.TagName = readerTag.GetString(3);
                //         item.Tags.Add(itemTag);
                //     }
                // }
                items.Add(item);
            }

            readerItem.Close();

        }

        return items;
    }

    [HttpGet("{id}")]
    public JsonResult GetItems(int id)
    {
        string query = "select ItemId, ItemName, Description, Item.CategoryId, CategoryName from Item LEFT JOIN Category ON Item.CategoryId = Category.CategoryId WHERE ItemId=" + id;
        DataTable table = new DataTable();
        string? connectionString = _configuration.GetConnectionString("BackAlleyMarkets");
        SqlDataReader myReader;
        using (SqlConnection myCon = new SqlConnection(connectionString))
        {
            myCon.Open();
            using (SqlCommand myCommand = new SqlCommand(query, myCon))
            {
                myReader = myCommand.ExecuteReader();
                table.Load(myReader);
            }
        }

        return new JsonResult(table);
    }

    // [HttpGet("get_items")]
    // public JsonResult get_items()
    // {
    //     string query = "select * from Item";
    //     DataTable table = new DataTable();
    //     string? SqlDatasource = _configuration.GetConnectionString("BackAlleyMarkets");
    //     SqlDataReader myReader;
    //     using (SqlConnection myCon = new SqlConnection(SqlDatasource))
    //     {
    //         myCon.Open();
    //         using (SqlCommand myCommand = new SqlCommand(query, myCon))
    //         {
    //             myReader = myCommand.ExecuteReader();
    //             table.Load(myReader);
    //         }
    //     }

    //     return new JsonResult(table);
    // }

    [HttpPost("add_item")]
    public async Task<IActionResult> AddItem([FromBody] CreateItemRequest newItem)
    {
        if (string.IsNullOrWhiteSpace(newItem.ItemName) ||
            string.IsNullOrWhiteSpace(newItem.CategoryName))
        {
            return BadRequest("ItemName and CategoryName are required.");
        }

        try
        {
            var item = await _items.CreateItemAsync(newItem);

            return item is null
                ? StatusCode(500, "Item was not created.")
                : Created($"/api/items/{item.ItemId}", item);
        }
        catch (DuplicateItemException ex)
        {
            return Conflict(ex.Message);
        }
        // string sqlQuery = """
        //                 SET XACT_ABORT ON;
        //                 BEGIN TRANSACTION;

        //                 -- 1. Reject duplicate item names
        //                 IF EXISTS (
        //                     SELECT 1 FROM dbo.Item WITH (UPDLOCK, HOLDLOCK)
        //                     WHERE ItemName = @ItemName
        //                 )
        //                 BEGIN
        //                     ROLLBACK TRANSACTION;
        //                     THROW 50001, 'An item with this name already exists.', 1;
        //                 END

        //                 -- 2. Find or create the category
        //                 DECLARE @CategoryId INT;

        //                 SELECT @CategoryId = CategoryId
        //                 FROM dbo.Category WITH (UPDLOCK, HOLDLOCK)
        //                 WHERE CategoryName = @CategoryName;

        //                 IF @CategoryId IS NULL
        //                 BEGIN
        //                     INSERT INTO dbo.Category (CategoryName) VALUES (@CategoryName);
        //                     SET @CategoryId = SCOPE_IDENTITY();
        //                 END

        //                 -- 3. Insert the item
        //                 INSERT INTO dbo.Item (ItemName, Description, CategoryId)
        //                 VALUES (@ItemName, @Description, @CategoryId);

        //                 DECLARE @ItemId INT = SCOPE_IDENTITY();

        //                 -- 4. Unpack the tag array (trimmed, blanks removed, duplicates removed)
        //                 DECLARE @TagNames TABLE (
        //                     TagName NVARCHAR(100) COLLATE DATABASE_DEFAULT PRIMARY KEY
        //                 );

        //                 INSERT INTO @TagNames (TagName)
        //                 SELECT DISTINCT LTRIM(RTRIM([value]))
        //                 FROM OPENJSON(@Tags)
        //                 WHERE LTRIM(RTRIM([value])) <> '';

        //                 -- 5. Create only the tags that don't exist yet
        //                 INSERT INTO dbo.Tag (TagName)
        //                 SELECT t.TagName
        //                 FROM @TagNames t
        //                 WHERE NOT EXISTS (
        //                     SELECT 1 FROM dbo.Tag WITH (UPDLOCK, HOLDLOCK)
        //                     WHERE TagName = t.TagName
        //                 );

        //                 -- 6. Link every tag (new or existing) to the new item
        //                 INSERT INTO dbo.Item_Tag (ItemId, TagId)
        //                 SELECT @ItemId, tg.TagId
        //                 FROM dbo.Tag tg
        //                 JOIN @TagNames t ON t.TagName = tg.TagName;

        //                 COMMIT TRANSACTION;

        //                 -- Result set 1: the item
        //                 SELECT @ItemId AS ItemId,
        //                     @ItemName AS ItemName,
        //                     @Description AS Description,
        //                     @CategoryId AS CategoryId;

        //                 -- Result set 2: its tags
        //                 SELECT tg.TagId, tg.TagName
        //                 FROM dbo.Item_Tag it
        //                 JOIN dbo.Tag tg ON tg.TagId = it.TagId
        //                 WHERE it.ItemId = @ItemId;
        //                 """;
        // try
        // {
        //     var tagsJson = JsonSerializer.Serialize(newItem.ItemTags ?? new List<string>());

        //     await using var conn = new SqlConnection(_connectionString);
        //     await using var cmd = new SqlCommand(sqlQuery, conn);

        //     cmd.Parameters.AddWithValue("@ItemName", newItem.ItemName.Trim());
        //     cmd.Parameters.AddWithValue("@Description", newItem.ItemDescription ?? string.Empty);
        //     cmd.Parameters.AddWithValue("@CategoryName", newItem.CategoryName.Trim());
        //     cmd.Parameters.Add("@Tags", SqlDbType.NVarChar, -1).Value = tagsJson;

        //     await conn.OpenAsync();
        //     await using var reader = await cmd.ExecuteReaderAsync();

        //     if (!await reader.ReadAsync())
        //     {
        //         return StatusCode(500, "Item was not created.");
        //     }

        //     var item = new Item
        //     {
        //         ItemId = reader.GetInt32(reader.GetOrdinal("ItemId")),
        //         ItemName = reader.GetString(reader.GetOrdinal("ItemName")),
        //         Description = reader.GetString(reader.GetOrdinal("Description")),
        //         CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId"))
        //     };

        //     // Move to the second result set (the tags)
        //     if (await reader.NextResultAsync())
        //     {
        //         while (await reader.ReadAsync())
        //         {
        //             item.Tags.Add(new ItemTag
        //             {
        //                 TagId = reader.GetInt32(reader.GetOrdinal("TagId")),
        //                 TagName = reader.GetString(reader.GetOrdinal("TagName"))
        //             });
        //         }
        //     }

        //     return Created($"/api/items/{item.ItemId}", item);
        // }
        // catch (SqlException ex) when (ex.Number == 50001)
        // {
        //     return Conflict("An item with this name already exists.");
        // }
        // catch (Exception ex)
        // {
        //     // Log the exception (ex) here as needed
        //     return StatusCode(500, "An error occurred while creating the item.");  
        // }
    }
    // [HttpPost("update_pizza/{pizzaName}/{toppings}/{pizzaId}")]
    // public JsonResult update_pizza(string pizzaName, string toppings, int pizzaId)
    // {
    //     string query = "UPDATE Pizzas SET PizzaName = '" + pizzaName + "', Toppings = '" + toppings + "' WHERE PizzaID = " + pizzaId;
    //     DataTable table = new DataTable();
    //     string SqlDatasource = _configuration.GetConnectionString("Pizza_Shop");
    //     SqlDataReader myReader;
    //     using (SqlConnection myCon = new SqlConnection(SqlDatasource))
    //     {
    //         myCon.Open();
    //         using (SqlCommand myCommand = new SqlCommand(query, myCon))
    //         {
    //             myReader = myCommand.ExecuteReader();
    //             table.Load(myReader);
    //         }
    //     }

    //     return new JsonResult("Updated Successfully");
    // }

    // [HttpDelete("delete_pizza/{pizzaId}")]
    // public JsonResult delete_pizza(int pizzaId)
    // {
    //     string query = "DELETE FROM Pizzas WHERE PizzaId = '" + pizzaId + "'";
    //     DataTable table = new DataTable();
    //     string SqlDatasource = _configuration.GetConnectionString("Pizza_Shop");
    //     SqlDataReader myReader;
    //     using (SqlConnection myCon = new SqlConnection(SqlDatasource))
    //     {
    //         myCon.Open();
    //         using (SqlCommand myCommand = new SqlCommand(query, myCon))
    //         {
    //             myReader = myCommand.ExecuteReader();
    //             table.Load(myReader);
    //         }
    //     }

    //     return new JsonResult("Deleted Successfully");
    // }


}