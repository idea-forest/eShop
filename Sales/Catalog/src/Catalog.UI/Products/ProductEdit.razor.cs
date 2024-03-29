﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using YourBrand.Catalog.Attributes;

namespace YourBrand.Catalog.Products;

partial class ProductEdit : ComponentBase
{
    ProductDto? product;

    [Parameter]
    public long? ProductId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        NavigationManager.LocationChanged += NavigationManager_LocationChanged;

        await LoadAsync();
    }

    private async void NavigationManager_LocationChanged(object? sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e)
    {
        await LoadAsync();

        StateHasChanged();
    }

    private async Task LoadAsync()
    {
        product = await ProductsClient.GetProductAsync(ProductId.ToString()!);
    }

    public void Dispose()
    {
        NavigationManager.LocationChanged -= NavigationManager_LocationChanged;
    }

    private async void UploadFiles(InputFileChangeEventArgs e)
    {
        var file = e.GetMultipleFiles().First();
        product!.Image = await ProductsClient.UploadProductImageAsync(product.Id, new FileParameter(file.OpenReadStream(3 *
        1000000), file.Name));

        StateHasChanged();
    }

    async Task UpdateVisibility(ProductVisibility? value)
    {
        try
        {
            await ProductsClient.UpdateProductVisibilityAsync(ProductId.GetValueOrDefault(), value.GetValueOrDefault());

            product!.Visibility = value;

            Snackbar.Add($"The item \"{product.Name}\" is now {product.Visibility.ToString()!.ToLower()}.", Severity.Success);
        }
        catch (Exception exc)
        {
            Snackbar.Add(exc.Message, Severity.Error);
        }
    }

    async Task UpdateGroup() 
    {
        var parameters = new DialogParameters();
        parameters.Add(nameof(Groups.ProductGroupSelectorModal.GroupId), product!.Group!.Id);

        var dref = DialogService.Show<Groups.ProductGroupSelectorModal>("", parameters);
        var r = await dref.Result;

        if(r.Canceled) return;

        var data = (ProductGroupTreeNodeDto)r.Data;

        product!.Group = await ProductsClient.UpdateProductGroupAsync(product!.Id, data.Id);
    }

    async Task UpdatePrice() 
    {
        var parameters = new DialogParameters();
        parameters.Add(nameof(UpdatePriceModal.Price), product!.Price);

        var dref = DialogService.Show<UpdatePriceModal>("Update price", parameters);
        var r = await dref.Result;

        if(r.Canceled) return;

        var data = (decimal)r.Data;

        await ProductsClient.UpdateProductPriceAsync(product!.Id, new UpdateProductPriceRequest {
            Price = data
        });

        product!.Price = data;
    }
}

