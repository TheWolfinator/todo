using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Todo_App.Application.Common.Mappings;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.TodoLists.Queries.GetTodos;
public class TagDto : IMapFrom<Tag>
{
    public int Id { get; set; }

    public string Name { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Tag, TagDto>();
    }
}
