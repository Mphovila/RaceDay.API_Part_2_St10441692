using System;
using System.Collections.Generic;

namespace RaceDay.API.Models;

public partial class Route
{
    public int RouteId { get; set; }

    public int EventId { get; set; }

    public string RouteName { get; set; } = null!;

    public string StartLocation { get; set; } = null!;

    public string EndLocation { get; set; } = null!;

    public decimal Distance { get; set; }

    public string? RouteDescription { get; set; }

    public virtual Event Event { get; set; } = null!;
}
