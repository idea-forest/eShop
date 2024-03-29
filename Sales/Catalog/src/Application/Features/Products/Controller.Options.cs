using Microsoft.AspNetCore.Mvc;

using YourBrand.Catalog.Features.Options;
using YourBrand.Catalog.Features.Products.Options;
using YourBrand.Catalog.Features.Products.Options.Groups;

namespace YourBrand.Catalog.Features.Products;

partial class ProductsController : Controller
{
    [HttpGet("{productId}/Options")]
    public async Task<ActionResult<IEnumerable<ProductOptionDto>>> GetProductOptions(long productId, string? variantId, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new GetProductOptions(productId, variantId), cancellationToken));
    }

    [HttpPost("{productId}/Options")]
    public async Task<ActionResult<OptionDto>> CreateProductOption(long productId, ApiCreateProductOption data, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new CreateProductOption(productId, data), cancellationToken));
    }

    [HttpPut("{productId}/Options/{optionId}")]
    public async Task<ActionResult<OptionDto>> UpdateProductOption(long productId, string optionId, ApiUpdateProductOption data, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new UpdateProductOption(productId, optionId, data), cancellationToken));
    }

    [HttpDelete("{productId}/Options/{optionId}")]
    public async Task<ActionResult> DeleteProductOption(long productId, string optionId)
    {
        await _mediator.Send(new DeleteProductOption(productId, optionId));
        return Ok();
    }

    [HttpPost("{productId}/Options/{optionId}/Values")]
    public async Task<ActionResult<OptionValueDto>> CreateProductOptionValue(long productId, string optionId, ApiCreateProductOptionValue data, CancellationToken cancellationToken)
    {

        return Ok(await _mediator.Send(new CreateProductOptionValue(productId, optionId, data), cancellationToken));
    }

    [HttpPost("{productId}/Options/{optionId}/Values/{valueId}")]
    public async Task<ActionResult> DeleteProductOptionValue(long productId, string optionId, string valueId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteProductOptionValue(productId, optionId, valueId), cancellationToken);
        return Ok();
    }

    [HttpGet("{productId}/Options/{optionId}/Values")]
    public async Task<ActionResult<IEnumerable<OptionValueDto>>> GetProductOptionValues(long productId, string optionId)
    {
        return Ok(await _mediator.Send(new GetOptionValues(optionId)));
    }

    [HttpGet("{productId}/Options/Groups")]
    public async Task<ActionResult<IEnumerable<OptionGroupDto>>> GetOptionGroups(long productId)
    {
        return Ok(await _mediator.Send(new GetProductOptionGroups(productId)));
    }

    [HttpPost("{productId}/Options/Groups")]
    public async Task<ActionResult<OptionGroupDto>> CreateOptionGroup(long productId, ApiCreateProductOptionGroup data)
    {
        return Ok(await _mediator.Send(new CreateProductOptionGroup(productId, data)));
    }

    [HttpPut("{productId}/Options/Groups/{optionGroupId}")]
    public async Task<ActionResult<OptionGroupDto>> UpdateOptionGroup(long productId, string optionGroupId, ApiUpdateProductOptionGroup data)
    {
        return Ok(await _mediator.Send(new UpdateProductOptionGroup(productId, optionGroupId, data)));
    }

    [HttpDelete("{productId}/Options/Groups/{optionGroupId}")]
    public async Task<ActionResult> DeleteOptionGroup(long productId, string optionGroupId)
    {
        await _mediator.Send(new DeleteProductOptionGroup(productId, optionGroupId));
        return Ok();
    }
}