using FluentAssertions;
using NUnit.Framework;
using Todo_App.Application.TodoItems.Queries.GetTodoItemsWithFilterAndPagination;
using Todo_App.Application.TodoItems.Commands.CreateTodoItem;
using Todo_App.Application.TodoLists.Commands.CreateTodoList;
using Todo_App.Domain.Entities;
using Todo_App.Application.TodoItems.Commands.UpdateTodoItemDetail;
using Todo_App.Domain.Enums;
using System.Linq.Expressions;

namespace Todo_App.Application.IntegrationTests.TodoItems.Queries;

using static Testing;

public class GetTodoItemsWithFilterAndPaginationTests : BaseTestFixture
{
    [Test]
    public async Task ShouldReturnAllItems_WhenNoFilterProvided()
    {
        var userId = await RunAsDefaultUserAsync();

        var listId = await SendAsync(new CreateTodoListCommand
        {
            Title = "New List"
        });

        var itemId1 = await SendAsync(new CreateTodoItemCommand
        {
            ListId = listId,
            Title = "New Item 1"
        });

        var itemId2 = await SendAsync(new CreateTodoItemCommand
        {
            ListId = listId,
            Title = "New Item 2"
        });

        var query = new GetTodoItemsWithFilterAndPaginationQuery
        {
            ListId = listId
        };

        var result = await SendAsync(query);

        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.Items.Select(i => i.Title).Should().Contain(new[] { "New Item 1", "New Item 2" });
    }

    [Test]
    public async Task ShouldFilterByTitle()
    {
        var userId = await RunAsDefaultUserAsync();

        var listId = await SendAsync(new CreateTodoListCommand
        {
            Title = "New List"
        });

        var itemId1 = await SendAsync(new CreateTodoItemCommand
        {
            ListId = listId,
            Title = "Alpha"
        });

        var itemId2 = await SendAsync(new CreateTodoItemCommand
        {
            ListId = listId,
            Title = "Beta"
        });

        var query = new GetTodoItemsWithFilterAndPaginationQuery
        {
            ListId = listId,
            Title = "Al"
        };

        var result = await SendAsync(query);

        result.Items.Should().HaveCount(1);
        result.Items.First().Title.Should().Be("Alpha");
    }

    [Test]
    public async Task ShouldFilterByTags()
    {
        var userId = await RunAsDefaultUserAsync();

        var listId = await SendAsync(new CreateTodoListCommand
        {
            Title = "New List"
        });

        var itemIdTagged = await SendAsync(new CreateTodoItemCommand
        {
            ListId = listId,
            Title = "Tagged",
        });

        var itemIdNotTagged = await SendAsync(new CreateTodoItemCommand
        {
            ListId = listId,
            Title = "Not Tagged"
        });

        var command = new UpdateTodoItemDetailCommand
        {
            Id = itemIdTagged,
            ListId = listId,
            Note = "A1",
            Priority = PriorityLevel.High,
            Tags = new List<Tag> { new() { Name = "tag1" } }
        };

        await SendAsync(command);

        var query = new GetTodoItemsWithFilterAndPaginationQuery
        {
            ListId = listId,
            Tags = new List<string> { "tag1" }
        };

        var result = await SendAsync(query);

        result.Items.Should().HaveCount(1);
        result.Items.First().Title.Should().Be("Tagged");
    }

    [Test]
    public async Task ShouldFilterByTagsAndTitle()
    {
        var userId = await RunAsDefaultUserAsync();

        var listId = await SendAsync(new CreateTodoListCommand
        {
            Title = "New List"
        });

        var itemIdTagged = await SendAsync(new CreateTodoItemCommand
        {
            ListId = listId,
            Title = "Tagged",
        });

        var itemIdNotTagged = await SendAsync(new CreateTodoItemCommand
        {
            ListId = listId,
            Title = "Not Tagged"
        });

        var command = new UpdateTodoItemDetailCommand
        {
            Id = itemIdTagged,
            ListId = listId,
            Note = "A1",
            Priority = PriorityLevel.High,
            Tags = new List<Tag> { new() { Name = "tag1" } }
        };

        await SendAsync(command);

        var query = new GetTodoItemsWithFilterAndPaginationQuery
        {
            ListId = listId,
            Title = "Tagged",
            Tags = new List<string> { "tag1" }
        };

        var result = await SendAsync(query);

        result.Items.Should().HaveCount(1);
        result.Items.First().Title.Should().Be("Tagged");
    }

    [Test]
    public async Task ShouldPaginateResults()
    {
        var userId = await RunAsDefaultUserAsync();

        var listId = await SendAsync(new CreateTodoListCommand
        {
            Title = "New List"
        });

        for (int i = 1; i <= 15; i++)
        {
            await SendAsync(new CreateTodoItemCommand
            {
                ListId = listId,
                Title = $"New Item {i:D2}"
            });
        }

        var query = new GetTodoItemsWithFilterAndPaginationQuery
        {
            ListId = listId,
            PageNumber = 2,
            PageSize = 5
        };

        var result = await SendAsync(query);

        result.Items.Should().HaveCount(5);
        result.PageNumber.Should().Be(2);
        result.TotalCount.Should().Be(15);
    }

}
