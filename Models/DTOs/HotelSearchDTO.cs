using System.ComponentModel.DataAnnotations;

namespace Midyaf.Models.DTOs;

public class PropertySearchDTO
{
    public string? Name { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    [Range(1, 5)]
    public int? MinRating { get; set; }
    public DateTime? CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }
    [Range(0, double.MaxValue)]
    public decimal? MinPrice { get; set; }
    [Range(0, double.MaxValue)]
    public decimal? MaxPrice { get; set; }
    [Range(1, 50)]
    public int? Guests { get; set; }
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;
    [Range(1, 100)]
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public string? SortOrder { get; set; } = "asc";
}
