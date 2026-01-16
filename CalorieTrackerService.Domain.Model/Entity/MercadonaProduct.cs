using System.Text.Json.Serialization;

public class MercadonaProduct
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("brand")]
    public string Brand { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("price")]
    public string Price { get; set; } = string.Empty;

    [JsonPropertyName("unit_price")]
    public double? UnitPrice { get; set; } = 0.0;

    [JsonPropertyName("category_id")]
    public string CategoryId { get; set; } = string.Empty;

    [JsonPropertyName("category_name")]
    public string CategoryName { get; set; } = string.Empty;

    [JsonPropertyName("image_url")]
    public string ImageUrl { get; set; } = string.Empty;

    [JsonPropertyName("published")]
    public bool Published { get; set; }

    [JsonPropertyName("available_online")]
    public bool AvailableOnline { get; set; }

    [JsonPropertyName("share_url")]
    public string ShareUrl { get; set; } = string.Empty;

    [JsonPropertyName("scraped_at")]
    public string ScrapedAt { get; set; } = string.Empty;

    [JsonPropertyName("main_category_id")]
    public int MainCategoryId { get; set; }

    [JsonPropertyName("main_category_name")]
    public string MainCategoryName { get; set; } = string.Empty;

    [JsonPropertyName("subcategory_id")]
    public int SubcategoryId { get; set; }

    [JsonPropertyName("subcategory_name")]
    public string SubcategoryName { get; set; } = string.Empty;
}