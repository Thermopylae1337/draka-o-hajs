using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public class BadgeList
{
    [JsonProperty("badges")]
    private readonly List<Badge> badges;

    public BadgeList() => badges = new List<Badge>();

    public void AddBadge(Badge badge) => badges.Add(badge);

    public Badge FindBadge(string name) => badges.Count > 0 ? ( badges?.Find(badge => badge.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) ) : null;

    public bool IsBadgeUnlocked(string name)
    {
        Badge badge = FindBadge(name);
        return badge != null;
    }
}
