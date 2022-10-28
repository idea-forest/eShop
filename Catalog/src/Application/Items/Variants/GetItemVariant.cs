using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Application.Items.Variants;
using YourBrand.Catalog.Domain;
using YourBrand.Catalog.Application.Attributes;
using YourBrand.Catalog.Application.Options;

namespace YourBrand.Catalog.Application.Items.Variants;

public record GetItemVariant(string ItemId, string ItemVariantId) : IRequest<ItemDto?>
{
    public class Handler : IRequestHandler<GetItemVariant, ItemDto?>
    {
        private readonly IApplicationDbContext _context;

        public Handler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ItemDto?> Handle(GetItemVariant request, CancellationToken cancellationToken)
        {
            var itemVariant = await _context.Items
                .AsSplitQuery()
                .AsNoTracking()
                .Include(pv => pv.Variants)
                .Include(pv => pv.AttributeValues)
                .ThenInclude(pv => pv.Attribute)
                //.ThenInclude(o => o.DefaultValue)
                .Include(pv => pv.AttributeValues)
                .ThenInclude(pv => pv.Value)
                .FirstOrDefaultAsync(pv => pv.ParentItem!.Id == request.ItemId && pv.Id == request.ItemVariantId);

            if(itemVariant is null) return null;

            return itemVariant.ToDto();
        }

        private static string? GetImageUrl(string? name)
        {
            return name is null ? null : $"http://127.0.0.1:10000/devstoreaccount1/images/{name}";
        }
    }
}
