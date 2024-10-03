using System;
using System.Collections.Generic;

namespace Digi_Pets_Battle.Models;

public partial class Pet
{
    public int Id { get; set; }

    public float? AccountId { get; set; }

    public string? Name { get; set; }

    public float? Health { get; set; }

    public float? Strength { get; set; }

    public float? Experience { get; set; }
}
