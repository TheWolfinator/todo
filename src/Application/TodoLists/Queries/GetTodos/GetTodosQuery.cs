using System.Reflection;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Domain.Enums;
using Todo_App.Domain.ValueObjects;

namespace Todo_App.Application.TodoLists.Queries.GetTodos;

public record GetTodosQuery : IRequest<TodosVm>
{
    public int? ListId { get; init; } = null;
    public int[] TagIds { get; init; } = Array.Empty<int>();
    public string Title { get; set; }
}

public class GetTodosQueryHandler : IRequestHandler<GetTodosQuery, TodosVm>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTodosQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<TodosVm> Handle(GetTodosQuery request, CancellationToken cancellationToken)
    {

        return new TodosVm
        {
            PriorityLevels = Enum.GetValues(typeof(PriorityLevel))
                .Cast<PriorityLevel>()
                .Select(p => new PriorityLevelDto { Value = (int)p, Name = p.ToString() })
                .ToList(),

            Lists = await _context.TodoLists
                .AsNoTracking()
                .ProjectTo<TodoListDto>(_mapper.ConfigurationProvider)
                .OrderBy(t => t.Title)
                .ToListAsync(cancellationToken),

            SupportedColours = GetSupportedColours(),
        };
    }

    private List<SupportedColourDto> GetSupportedColours()
    {
        var colour = Colour.Instance;
        List<PropertyInfo> list = new List<PropertyInfo>();
        foreach (var property in typeof(Colour).GetProperties()) list.Add(property);
        var supportedColours = list
            .Select(prop => new SupportedColourDto()
            {
                Hex = prop.GetValue(colour)?.ToString() ?? string.Empty,
                Name = prop.Name
            })
            .ToList();
        return supportedColours;
    }
}
