using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todo_App.Domain.Entities;
public class Tag : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;

    public int TodoItemId { get; set; }

    public TodoItem TodoItem { get; set; } = null!;

}
