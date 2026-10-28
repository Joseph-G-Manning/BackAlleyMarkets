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

public class CreateItemRequest
{
    public string ItemName { get; set; } = string.Empty;
    public string ItemDescription { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public List<string> ItemTags { get; set; } = new();
}

public class TagResponse
{
    public int TagId { get; set; }
    public string TagName { get; set; } = string.Empty;
}

public class ItemResponse
{
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public List<TagResponse> Tags { get; set; } = new();
}