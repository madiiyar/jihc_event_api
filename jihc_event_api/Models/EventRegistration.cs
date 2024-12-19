using System;
using System.Collections.Generic;

namespace jihc_event_api.Models;

public partial class EventRegistration
{
    public int Id { get; set; }

    public int EventId { get; set; }

    public int UserId { get; set; }

    public DateTime? RegistrationDate { get; set; } = DateTime.Now;

    // Make navigation properties nullable
    public virtual Event? Event { get; set; }
    public virtual User? User { get; set; }
}
