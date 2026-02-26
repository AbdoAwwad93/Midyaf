using Midyaf.Models;
using Midyaf.Models.DTOs;

namespace Midyaf.Repository;

public interface IPropertyRepository : IGenericRepository<Property>
{
    Task<PaginatedResult<Property>> SearchAsync(PropertySearchDTO searchDto);
}
