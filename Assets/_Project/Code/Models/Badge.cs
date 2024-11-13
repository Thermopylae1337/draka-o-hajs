public class Badge
{
    [JsonProperty("name")]
    public string Name { get; private set; }

    [JsonProperty("unlocked")]
    public bool Unlocked { get; private set; }

    [JsonProperty("unlockCondition")]
    public string UnlockCondition { get; private set; }

    [JsonProperty("unlockCounter")]
    public int UnlockCounter { get; private set; }

    public Badge(string name, string unlockCondition)
    {
        Name = name;
        UnlockCondition = unlockCondition;
        Unlocked = false;
    }

    public void Unlock()
    {
        Unlocked = true;
    }

    public bool IsUnlocked()
    {
        return Unlocked;
    }
}

