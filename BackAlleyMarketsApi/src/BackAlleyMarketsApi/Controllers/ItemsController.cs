using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Any;
using Microsoft.AspNetCore.Http.HttpResults;
using BackAlleyMarketsApi.Models;

namespace BackAlleyMarketsApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ItemsController : ControllerBase
{

    private IConfiguration _configuration;
    public ItemsController(IConfiguration configuration)
    {
        _configuration = configuration;
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
        string? SqlDatasource = _configuration.GetConnectionString("BackAlleyMarkets");
        SqlDataReader myReader;
        using (SqlConnection myCon = new SqlConnection(SqlDatasource))
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

    // [HttpPost("add_pizza/{pizzaName}/{toppings}")]
    // public JsonResult add_pizza(string pizzaName, string toppings)
    // {
    //     string query = "INSERT INTO Pizzas VALUES ('" + toppings + "', '" + pizzaName + "')";
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

    //     return new JsonResult("Added Successfully");
    // }

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