using Todo_App.Application.Common.Mappings;
using Todo_App.Domain.Entities;
using Todo_App.Domain.Enums;
using Todo_App.Domain.ValueObjects;

namespace Todo_App.Application.TodoItems.Queries.GetTodoItemsWithFilterAndPagination;

public class TodoItemBriefDto : IMapFrom<TodoItem>
{
    public int Id { get; set; }

    public int ListId { get; set; }

    public string? Title { get; set; }

    public Colour Colour { get; set; } = Colour.White;
    public string? Note { get; set; }

    public PriorityLevel Priority { get; set; }

    public bool Done { get; set; }

    public List<Tag> Tags { get; set; } = new List<Tag>();
}
