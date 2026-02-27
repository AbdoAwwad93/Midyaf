using Microsoft.EntityFrameworkCore;
using Midyaf.Data;
using Midyaf.Models;
using Midyaf.Models.DTOs;

namespace Midyaf.Repository;

public class PropertyRepository : GenericRepository<Property>, IPropertyRepository
{
    public PropertyRepository(AppDbContext context)
        : base(context)
    {
    }

    public async Task<PaginatedResult<Property>> SearchAsync(PropertySearchDTO searchDto)
    {
        IQueryable<Property> query = _dbset.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchDto.Name))
        {
            var name = searchDto.Name.ToLower();
            query = query.Where(h => h.Name.ToLower().Contains(name));
        }

        if (!string.IsNullOrWhiteSpace(searchDto.City))
        {
            var city = searchDto.City.ToLower();
            query = query.Where(h => h.City.ToLower() == city);
        }

        if (!string.IsNullOrWhiteSpace(searchDto.Country))
        {
            var country = searchDto.Country.ToLower();
            query = query.Where(h => h.Country.ToLower() == country);
        }

        if (searchDto.MinRating.HasValue)
        {
            query = query.Where(h => h.Reviews != null && h.Reviews.Count > 0 &&
                                     h.Reviews.Average(r => r.Rate) >= searchDto.MinRating.Value);
        }

        if (searchDto.MinPrice.HasValue)
        {
            query = query.Where(h => h.Rooms.Any(r => r.Price >= searchDto.MinPrice.Value));
        }

        if (searchDto.MaxPrice.HasValue)
        {
            query = query.Where(h => h.Rooms.Any(r => r.Price <= searchDto.MaxPrice.Value));
        }

        if (searchDto.Guests.HasValue)
        {
            query = query.Where(h => h.Rooms.Any(r => r.Capacity >= searchDto.Guests.Value));
        }

        if (searchDto.CheckIn.HasValue && searchDto.CheckOut.HasValue)
        {
            var checkIn = searchDto.CheckIn.Value;
            var checkOut = searchDto.CheckOut.Value;

            query = query.Where(h => h.Rooms.Any(room =>
                room.IsAvailable &&
                (room.Reservations.All(res => res.CheckIn.Date == default ||
                                              res.CheckOut <= checkIn || res.CheckIn >= checkOut))
            ));
        }

        query = searchDto.SortBy?.ToLower() switch
        {
            "name" => searchDto.SortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(h => h.Name)
                : query.OrderBy(h => h.Name),
            "price" => searchDto.SortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(h => h.Rooms.Min(r => r.Price))
                : query.OrderBy(h => h.Rooms.Min(r => r.Price)),
            "rating" => searchDto.SortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(h => h.Reviews != null && h.Reviews.Count > 0 ? h.Reviews.Average(r => r.Rate) : 0)
                : query.OrderBy(h => h.Reviews != null && h.Reviews.Count > 0 ? h.Reviews.Average(r => r.Rate) : 0),
            _ => query.OrderBy(h => h.Name)
        };

        var totalCount = await query.CountAsync();
        var properties = await query
            .Skip((searchDto.Page - 1) * searchDto.PageSize)
            .Take(searchDto.PageSize)
            .ToListAsync();

        return new PaginatedResult<Property>
        {
            Items = properties,
            TotalCount = totalCount,
            Page = searchDto.Page,
            PageSize = searchDto.PageSize
        };
    }
}
