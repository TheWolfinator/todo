using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Application.Common.Mappings;
using Todo_App.Application.Common.Models;

namespace Todo_App.Application.TodoItems.Queries.GetTodoItemsWithFilterAndPagination;

public record GetTodoItemsWithFilterAndPaginationQuery : IRequest<PaginatedList<TodoItemBriefDto>>
{
    public int ListId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public List<string> Tags { get; init; } = new List<string>();
    public string Title { get; init; }

}

public class GetTodoItemsWithFilterAndPaginationQueryHandler : IRequestHandler<GetTodoItemsWithFilterAndPaginationQuery, PaginatedList<TodoItemBriefDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTodoItemsWithFilterAndPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<TodoItemBriefDto>> Handle(GetTodoItemsWithFilterAndPaginationQuery request, CancellationToken cancellationToken)
    {

        var query = _context.TodoItems
            .Include(td => td.Tags)
            .AsQueryable();

        // Filter by tags if provided

        if (request.Tags.Any())
        {
            var normalizedTags = request.Tags
                .Select(t => t.Trim().ToLower())
                .ToList();

            query = query.Where(q => q.Tags.Any(x => normalizedTags.Contains(x.Name.Trim().ToLower())));
        }

        // Filter by title if provided
        if (!string.IsNullOrWhiteSpace(request.Title))
        {
            query = query.Where(q => EF.Functions.Like(q.Title, $"%{request.Title}%"));
        }

        return await query
            .OrderBy(x => x.Title)
            .ProjectTo<TodoItemBriefDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}
