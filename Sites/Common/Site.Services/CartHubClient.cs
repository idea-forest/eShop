﻿using Microsoft.AspNetCore.SignalR.Client;

namespace Site.Services;

public class CartHubClient
{
    HubConnection hubConnection = null!;
    private readonly IAccessTokenProvider accessTokenProvider;

    public CartHubClient(IAccessTokenProvider accessTokenProvider)
    {
        this.accessTokenProvider = accessTokenProvider;
    }

    public async Task StartAsync(string baseUri, string clientId)
    {
        try
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl($"{baseUri}hubs/cart?clientId={clientId}", config =>
                {
                    config.AccessTokenProvider = async () => await accessTokenProvider.GetAccessToken();
                })
                .WithAutomaticReconnect()
                .Build();

            hubConnection.On("CartUpdated", OnCartUpdated);

            hubConnection.Closed += async (exc) =>
            {
                if (ConnectionClosed is null) return;

                await ConnectionClosed.Invoke(exc);
            };

            hubConnection.Reconnected += async (error) =>
            {
                if (Reconnected is null) return;

                await Reconnected.Invoke(error);
            };

            hubConnection.Reconnecting += async (exc) =>
            {
                if (Reconnecting is null) return;

                await Reconnecting.Invoke(exc);
            };

            await hubConnection.StartAsync();

            //Snackbar.Add("Connected");
        }
        catch (Exception exc)
        {
            //Snackbar.Add(exc.Message.ToString(), Severity.Error);
        }
    }

    public event Func<Exception?, Task>? ConnectionClosed;

    public event Func<string?, Task>? Reconnected;

    public event Func<Exception?, Task>? Reconnecting;

    public event EventHandler CartUpdated = null!;

    private void OnCartUpdated() => CartUpdated?.Invoke(this, EventArgs.Empty);

    public bool IsConnected => hubConnection?.State == HubConnectionState.Connected;

    public async Task StopAsync()
    {
        await hubConnection.StopAsync();
    }

    public async Task RestartAsync()
    {
        await hubConnection.StopAsync();
        await hubConnection.StartAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (hubConnection is null)
            return;

        await hubConnection.DisposeAsync();
    }
}

