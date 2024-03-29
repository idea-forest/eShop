using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Common.Models;

namespace YourBrand.Catalog.Features.Attributes;

public record GetAttributes(string[]? Ids = null, int Page = 10, int PageSize = 10, string? SearchString = null, string? SortBy = null, Common.Models.SortDirection? SortDirection = null) : IRequest<ItemsResult<AttributeDto>>
{
    public class Handler : IRequestHandler<GetAttributes, ItemsResult<AttributeDto>>
    {
        private readonly IApplicationDbContext _context;

        public Handler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ItemsResult<AttributeDto>> Handle(GetAttributes request, CancellationToken cancellationToken)
        {
            var query = _context.Attributes
                .AsSplitQuery()
                .AsNoTracking()
                .Include(o => o.Group)
                .Include(o => o.Values)
                .AsQueryable();

            if (request.Ids?.Any() ?? false)
            {
                var ids = request.Ids;
                query = query.Where(o => ids.Any(x => x == o.Id));
            }

            if (request.SearchString is not null)
            {
                query = query.Where(o => o.Name.ToLower().Contains(request.SearchString.ToLower()));
            }

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection == YourBrand.Catalog.Common.Models.SortDirection.Desc ? YourBrand.Catalog.SortDirection.Descending : YourBrand.Catalog.SortDirection.Ascending);
            }
            else
            {
                query = query.OrderBy(x => x.Name);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                            .OrderBy(x => x.Name)
                            .Include(x => x.Values)
                            .Skip(request.Page * request.PageSize)
                            .Take(request.PageSize).AsQueryable()
                            .ToArrayAsync();

            return new ItemsResult<AttributeDto>(items.Select(item => item.ToDto()), totalCount);
        }
    }
}