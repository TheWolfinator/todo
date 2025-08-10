using MediatR;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using AutoMapper.Configuration.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Todo_App.Application.Common.Exceptions;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Domain.Entities;
using Todo_App.Domain.Enums;
using Todo_App.Domain.ValueObjects;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Todo_App.Application.TodoItems.Commands.UpdateTodoItemDetail;

public record UpdateTodoItemDetailCommand : IRequest
{
    public int Id { get; init; }

    public int ListId { get; init; }

    public PriorityLevel Priority { get; init; }

    public string? Note { get; init; }
    public string? Colour { get; init; }

    public List<Tag> Tags { get; init; }
}

public class UpdateTodoItemDetailCommandHandler : IRequestHandler<UpdateTodoItemDetailCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<UpdateTodoItemDetailCommandHandler> _logger;

    public UpdateTodoItemDetailCommandHandler(IApplicationDbContext context, ILogger<UpdateTodoItemDetailCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Unit> Handle(UpdateTodoItemDetailCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TodoItems
            .FindAsync(new object[] { request.Id }, cancellationToken);
        if (entity == null)
        {
            throw new NotFoundException(nameof(TodoItem), request.Id);
        }

        entity.ListId = request.ListId;
        entity.Priority = request.Priority;
        entity.Note = request.Note;
        entity.Colour = request.Colour is null ? Colour.White : Colour.From(request.Colour);
        UpdateTags(request, entity);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error updating TodoItem with ID {TodoItemId}", request.Id);
            throw; 
        }
        

        return Unit.Value;
    }

    private void UpdateTags(UpdateTodoItemDetailCommand request, TodoItem entity)
    {

        var existingTags = _context.Tags
            .Where(t => t.TodoItem.Id == request.ListId)
            .Select(t => t.Name.ToLower().Trim());

        var requestTags = (request.Tags ?? Enumerable.Empty<Tag>())
            .Select(t => t.Name?.ToLower().Trim())
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Distinct()
            .ToList();

        // tags that are not in db
        var newTags = requestTags.Where(rt => !existingTags.Contains(rt));
        // tags in db that is not in the request
        var tagsToRemove = _context.Tags.Where(t => !requestTags.Contains(t.Name) && t.TodoItem.Id == request.ListId);

        foreach (var tag in tagsToRemove)
        {
            entity.Tags.Remove(tag);
        }

        foreach (var newTag in newTags)
        {
            entity.Tags.Add(new Tag { Name = newTag, TodoItemId = entity.Id });
        }
    }
}
