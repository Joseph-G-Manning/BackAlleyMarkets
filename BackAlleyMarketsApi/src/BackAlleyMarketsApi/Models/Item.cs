using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;
using Microsoft.Identity.Client;

namespace BackAlleyMarketsApi.Models;

public class Item
{
    public int ItemId { get; set; }
    public string? ItemName { get; set; }
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public List<ItemTag> Tags { get; init; } = new();
}