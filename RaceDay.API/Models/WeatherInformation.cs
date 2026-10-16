using System;
using System.Collections.Generic;

namespace RaceDay.API.Models;

public partial class WeatherInformation
{
    public int WeatherId { get; set; }

    public int EventId { get; set; }

    public decimal? Temperature { get; set; }

    public string? WeatherDescription { get; set; }

    public decimal? WindSpeed { get; set; }

    public DateTime LastUpdated { get; set; }

    public virtual Event Event { get; set; } = null!;
}
