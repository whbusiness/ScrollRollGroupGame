using System;

/// <summary>
/// A simple store of all match-related data for a player.
/// </summary>
[Serializable]
public struct PlayerMatchData
{
    public bool m_isInMatch;
    /// <summary>
    /// Times player has pushed, even if pushes did not hit any other player
    /// </summary>
    public int m_timesPushed;
    /// <summary>
    /// Times player has pulled, even if pulls did not hit any other player
    /// </summary>
    public int m_timesPulled;
    /// <summary>
    /// Times player has engaged a block move
    /// </summary>
    public int m_timesBlocked;
    /// <summary>
    /// Times player has successfully pushed & hit another player
    /// </summary>
    public int m_timesPushedHit;
    /// <summary>
    /// Times player has successfully pulled & hit another player
    /// </summary>
    public int m_timesPulledHit;
    /// <summary>
    /// Times player has successfully blocked other player pulls or pushes
    /// </summary>
    public int m_timesBlockedHit;
    /// <summary>
    /// Times player has been pushed by other players
    /// </summary>
    public int m_timesPushedByOthers;
    /// <summary>
    /// Times player has been pulled by other players
    /// </summary>
    public int m_timesPulledByOthers;
    /// <summary>
    /// Times player attacks have been blocked by others
    /// </summary>
    public int m_timesBlockedByOthers;
    /// <summary>
    /// Times player has fallen off (either by self or by environment)
    /// </summary>
    public int m_timesFallenOff;
    /// <summary>
    /// Times player has been pushed off by other players
    /// </summary>
    public int m_timesPushedOff;
    /// <summary>
    /// Times player has been pulled off by other players
    /// </summary>
    public int m_timesPulledOff;
    /// <summary>
    /// Times player has pushed other players off
    /// </summary>
    public int m_timesPushedOthersOff;
    /// <summary>
    /// Times player has pulled other players off
    /// </summary>
    public int m_timesPulledOthersOff;
    /// <summary>
    /// Total magical overload damage this player has taken
    /// </summary>
    public int m_totalDamageTaken;
    /// <summary>
    /// Maximum magical overload this player reached during the match
    /// </summary>
    public int m_maxMagicalOverload;
    /// <summary>
    /// Total damage this player has blocked
    /// </summary>
    public int m_totalDamageBlocked;
    /// <summary>
    /// Total time this player has spent with the block engaged
    /// </summary>
    public float m_timeSpentBlocking;
    /// <summary>
    /// Time alive during player life 1
    /// </summary>
    public float m_timeAliveLife1;
    /// <summary>
    /// Time alive during player life 2
    /// </summary>
    public float m_timeAliveLife2;
    /// <summary>
    /// Time alive during player life 3
    /// </summary>
    public float m_timeAliveLife3;
    public float m_totalTimeAlive;
    public GameMatchPlace m_matchPlace;
    /// <summary>
    /// Total time alive for this player - this should be a maximum of the match time
    /// </summary>
    public readonly float m_timeAlive { get { return m_timeAliveLife1 + m_timeAliveLife2 + m_timeAliveLife3; }}
}