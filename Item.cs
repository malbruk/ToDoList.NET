using System;
using System.Collections.Generic;

namespace ToDoAPI;

public partial class Item
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public bool? IsComplete { get; set; }
}
