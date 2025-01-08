/// <summary>
/// Wyliczenie reprezentujące typy lobby.
/// </summary>
public enum LobbyTypeEnum
{
    /// <summary>
    /// Typ lobby dla gospodarza (hosta)
    /// </summary>
    Host,
    /// <summary>
    /// Typ lobby dla osoby dołączającej do istniejącego lobby.
    /// </summary>
    Join,
    /// <summary>
    /// Typ lobby, w którym nie wybrano jeszcze żadnego rodzaju (domyślny stan).
    /// </summary>
    NotSelected
}
