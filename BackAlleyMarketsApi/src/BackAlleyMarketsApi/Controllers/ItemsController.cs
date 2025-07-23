using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Any;
using Microsoft.AspNetCore.Http.HttpResults;

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
    public JsonResult GetItems()
    {
        string query = "SELECT ItemId, ItemName, Description, Item.CategoryId, CategoryName FROM Item LEFT JOIN Category ON Item.CategoryId = Category.CategoryId;";
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