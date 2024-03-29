﻿using Bunit;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using NSubstitute;
using YourBrand.Portal;
using YourBrand.Portal.Pages;

namespace YourBrand.Portal.Tests;

public class TodosPageTest
{
    [Fact]
    public void ItemsShouldLoadOnInitializedSuccessful()
    {
        // Arrange
        using var ctx = new TestContext();
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;

        ctx.Services.AddMudServices();

        ctx.Services.AddLocalization();

        var fakeAccessTokenProvider = Substitute.For<YourBrand.Portal.Services.IAccessTokenProvider>();

        ctx.Services.AddSingleton(fakeAccessTokenProvider);

        var fakeTodosClient = Substitute.For<ITodosClient>();
        fakeTodosClient.GetTodosAsync(Arg.Any<TodoStatusDto>(), null, null, null, null, default)
            .ReturnsForAnyArgs(t => new ItemsResultOfTodoDto()
            {
                Items = new[]
                {
                    new TodoDto
                    {
                        Id = 1,
                        Title = "Item 1",
                        Description = "Description",
                        Status = TodoStatusDto.InProgress,
                        Created = DateTimeOffset.Now.AddMinutes(-3)
                    },
                    new TodoDto
                    {
                        Id = 2,
                        Title = "Item 2",
                        Description = "Description",
                        Status = TodoStatusDto.InProgress,
                        Created = DateTimeOffset.Now.AddMinutes(-1)
                    },
                    new TodoDto
                    {
                        Id = 3,
                        Title = "Item 2",
                        Description = "Description",
                        Status = TodoStatusDto.InProgress,
                        Created = DateTimeOffset.Now
                    }
                },
                TotalItems = 3
            });

        ctx.Services.AddSingleton<ITodosClient>(fakeTodosClient);


        /*
        var cut = ctx.RenderComponent<TodosPage>();

        // Act
        //cut.Find("button").Click();

        // Assert
        cut.WaitForState(() => cut.Find("tr") != null);

        int expectedNoOfTr = 4; // incl <td> in <thead>

        cut.FindAll("tr").Count.Should().Be(expectedNoOfTr);

        */
    }
}
