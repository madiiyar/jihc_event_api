using System;
using System.Collections.Generic;

namespace jihc_event_api.Models;

public partial class Event
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Type { get; set; } = null!;

    public DateOnly Date { get; set; }

    public string Location { get; set; } = null!;

    public string Price { get; set; } = null!;

    public DateOnly Deadline { get; set; }

    public string Audience { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string? Image { get; set; }

    public int? Attendees { get; set; }

    public int MaxAttendees { get; set; }

    public virtual ICollection<EventRegistration> EventRegistrations { get; set; } = new List<EventRegistration>();
}
