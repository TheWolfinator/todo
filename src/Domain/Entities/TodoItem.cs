namespace Todo_App.Domain.Entities;

public class TodoItem : BaseAuditableEntity
{
    public TodoItem()
    {
        Tags = new HashSet<Tag>();
    }   

    public int ListId { get; set; }

    public string? Title { get; set; }

    public string? Note { get; set; }

    public Colour Colour { get; set; } = Colour.White;

    public PriorityLevel Priority { get; set; }

    public DateTime? Reminder { get; set; }

    private bool _done;
    public bool Done
    {
        get => _done;
        set
        {
            if (value == true && _done == false)
            {
                AddDomainEvent(new TodoItemCompletedEvent(this));
            }

            _done = value;
        }
    }

    public TodoList List { get; set; } = null!;
    public virtual ICollection<Tag> Tags { get; set; }

}
