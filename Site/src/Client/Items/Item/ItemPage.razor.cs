﻿using System;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;
using System.Xml.Linq;
using Site.Client.Items.Item;
using Blazored.Toast.Services;

namespace Site.Client.Items.Item;

partial class ItemPage
{
    ProductViewModel? productViewModel;

    int quantity = 1;
    bool hasAddedToCart = false;

    private PersistingComponentStateSubscription persistingSubscription;

    [Parameter]
    public string Id { get; set; }

    [Parameter]
    public string? VariantId { get; set; }

    [Parameter]
    [SupplyParameterFromQuery(Name = "cartItemId")]
    public string? CartItemId { get; set; }

    [Parameter]
    [SupplyParameterFromQuery(Name = "d")]
    public string? Data { get; set; }

    [Inject]
    public IToastService ToastService { get; set; } = null!;

    [Inject]
    public RenderingContext RenderingContext { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        persistingSubscription =
            ApplicationState.RegisterOnPersisting(PersistItems);

        if (!ApplicationState.TryTakeFromJson<ProductViewModel>(
            "productViewModel", out var restored))
        {
            var pwm = new ProductViewModel(ItemsClient);
            await pwm.Initialize(Id, VariantId);
            productViewModel = pwm;
        }
        else
        {
            productViewModel = restored!;
            productViewModel.SetClient(ItemsClient);
        }

        if(!RenderingContext.IsPrerendering) 
        {
            if(CartItemId is not null) 
            {
                var cart = await CartClient.GetCartAsync();
                var item = cart.Items.First(x => x.Id == CartItemId);
                Data = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(item.Data));

                //Console.WriteLine(Data);
            }
        }

        if(Data is not null) 
        {
            var str = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(Data));
            Deserialize(str);
        }
    }

    private Task PersistItems()
    {
        ApplicationState.PersistAsJson("productViewModel", productViewModel);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        persistingSubscription.Dispose();
    }

    async Task UpdateUrl()
    {
        string data = Serialize();
        data = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(data));

        await JS.InvokeVoidAsync("skipScroll");

        System.Text.StringBuilder sb = new ();
        sb.Append($"/items/{Id}");

        if(productViewModel!.VariantId is not null) 
        {
            sb.Append($"/{productViewModel.VariantId}");
        }

        if(data is not null) 
        {
            sb.Append($"?d={data}");
        }

        if(CartItemId is not null) 
        {
            sb.Append($"&cartItemId={CartItemId}");
        }
        
        NavigationManager.NavigateTo(sb.ToString(), replace: true);
    }

    async Task AddItemToCart()
    {
        await CartClient.AddItemToCartAsync(new AddCartItemDto()
        {
            ItemId = productViewModel?.Variant?.Id ?? productViewModel?.Item?.Id,
            Quantity = quantity,
            Data = Serialize()
        });

        hasAddedToCart = true;

        //ToastService.ShowInfo($"{productViewModel.Name} was added to your basket,", "Item added");
    }

    async Task UpdateCartItem()
    {
        await CartClient.UpdateCartItemAsync(CartItemId, new UpdateCartItemDto() {
            Quantity = quantity,
            Data = Serialize()
        });

        hasAddedToCart = true;

        ToastService.ShowInfo($"{productViewModel.Name} was updated in your basket,", "Item updated");
    }
    
    string Serialize() 
    {
        return JsonSerializer.Serialize(productViewModel!.GetData(), new JsonSerializerOptions
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });
    }

    void Deserialize(string str)
    {
        var options = JsonSerializer.Deserialize<IEnumerable<ProductViewModel.Option>>(str, new JsonSerializerOptions
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });

        productViewModel!.LoadData(options!);
    }

    /*
    async Task UpdateUrl()
    {
        string data = Serialize();
        data = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(data));

        await JS.InvokeVoidAsync("skipScroll");

        NavigationManager.NavigateTo($"/items/{Id}?d={data}", forceLoad: false, replace: true);
    }
    */
}

