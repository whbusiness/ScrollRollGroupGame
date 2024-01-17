using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPodiumPanelHandler : MonoBehaviour
{
    public TMPro.TextMeshProUGUI m_playerName;
    public TMPro.TextMeshProUGUI m_playerStats;
    public RawImage m_rawImage;

    public void SetPlayerStats(PlayerMatchData p_matchData)
    {
        m_playerStats.text = "Survived " + p_matchData.m_timeAlive.ToShortString(1) + "s";
        int l_totalAttacks = p_matchData.m_timesPushed + p_matchData.m_timesPulled;
        int l_totalHits = p_matchData.m_timesPushedHit + p_matchData.m_timesPulledHit;

        if (l_totalAttacks != 0)
        {
            m_playerStats.text += "\nAttack Accuracy: " + (l_totalHits / l_totalAttacks).ToShortString(2) + "%";
        } else
        {
            m_playerStats.text += "\nNo attacks! Did you try?";
        }
        m_playerStats.text += "\nHighest Overload: " + p_matchData.m_maxMagicalOverload + "%"; 
    }
}
