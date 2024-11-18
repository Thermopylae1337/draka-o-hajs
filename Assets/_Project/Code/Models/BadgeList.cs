using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class BadgeList
{
    [JsonProperty("badges")]
    private List<Badge> badges;

    public BadgeList()
    {
        badges = new List<Badge>();
    }

    public void AddBadge(Badge badge)
    {
        badges.Add(badge);
    }

    public Badge FindBadge(string name)
    {
        if (badges.Count > 0)
        {
            return badges.Find(badge => badge.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
        return null;
    }

    public bool IsBadgeUnlocked(string name)
    {
        var badge = FindBadge(name);
        return badge != null && badge.IsUnlocked();
    }
}
