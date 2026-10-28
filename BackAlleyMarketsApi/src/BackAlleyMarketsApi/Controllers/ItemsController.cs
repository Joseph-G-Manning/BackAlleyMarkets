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
                ItemTag itemTag = new ItemTag();
                itemTag.ItemTagId = readerTag.GetInt32(0);
                itemTag.ItemId = readerTag.GetInt32(1);
                itemTag.TagId = readerTag.GetInt32(2);
                itemTag.TagName = readerTag.GetString(3);
                tempTagList.Add(itemTag);
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
    }
}