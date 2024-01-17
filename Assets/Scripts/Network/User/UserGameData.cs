using System;
using UnityEngine;

public enum GameMatchPlace
{
    First,
    Second,
    Third,
    Fourth
}

public enum GameMatchResult
{
    Win,
    Lose,
    Draw
}

public enum GameType
{
    Online,
    Local
}

[Serializable]
public class UserGameData
{
    /// <summary>
    /// Total number of games played for this game type 
    /// </summary>
    public int m_gamesPlayed = 0;

    /// <summary>
    /// Amount of times player has tried to block  (held block button > set amount of time)
    /// </summary>
    public int m_blocksAttempted = 0;
    /// <summary>
    /// Times player successfulled blocked hits
    /// </summary>
    public int m_blockedHits = 0;

    /// <summary>
    /// Times player has taken damage & been hit
    /// </summary>
    public int m_timesHit = 0;
    /// <summary>
    /// Times player has hit an enemy
    /// </summary>
    public int m_totalHits = 0;

    /// <summary>
    /// Integer array that holds player final placing after each game, with
    /// array position representing literal position (index 0 = 1st place etc.)
    /// </summary>
    public int[] m_gamesPlayedPlaced = { 0, 0, 0, 0 };

    /// <summary>
    /// 
    /// Integer index position (FALL, PUSH, PULL, SPEC):
    /// <list type="bullet">
    ///     0 - times fallen off (self)
    ///     1 - times pushed off (by other)
    ///     2 - times pulled off (by other)
    ///     3 - times special attacked off (by otber)
    /// </list>
    /// </summary>
    public int[] m_timesFallen = { 0, 0, 0, 0 };

    public string GetJSON()
    {
        return JsonUtility.ToJson(this);
    }

    public static UserGameData FromJSON(string p_json)
    {
        return JsonUtility.FromJson<UserGameData>(p_json);
    }
}