using System;

namespace Domain.Entities;

/// <summary>
/// A Firebase Cloud Messaging (FCM) registration token for one of a user's devices.
/// A single user may have several (phone, tablet, web).
/// </summary>
public partial class DeviceToken : BaseEntity
{
    public int UserId { get; set; }

    /// <summary>The FCM registration token reported by the client device.</summary>
    public string Token { get; set; } = null!;

    /// <summary>Optional device platform hint: "android", "ios", "web".</summary>
    public string? Platform { get; set; }

    public virtual User User { get; set; } = null!;
}
