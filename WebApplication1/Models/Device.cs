using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Device
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Token { get; set; } = null!;

    public string Platform { get; set; } = null!;

    public DateTime RegisteredAt { get; set; }

    public DateTime LastUsedAt { get; set; }

    public bool IsActive { get; set; }
}
