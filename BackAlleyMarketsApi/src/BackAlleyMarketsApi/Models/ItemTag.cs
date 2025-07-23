using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BackAlleyMarketsApi.Models;

public class ItemTag {
    public int ItemTagId { get; set; }
    public int TagId { get; set; }
    public string? TagName { get; set; }
    public int ItemId { get; set; }
    public string? ItemName { get; set; }
}