using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUDController : MonoBehaviour
{
    public int m_whichPlayer = 0;
    public PlayerHUDData m_data;
    public NewGameController m_gameController;
    public Image m_hatIndicator;
    public Image m_bodyIndicator;
    public Image m_wandIndicator;
    public Image[] m_lives;
    public Image m_powerupIcon;
    public PowerupType m_powerUpType = PowerupType.None;
    public MagicalOverloadHandler m_MOhandler;
    [SerializeField]
    private List<PowerupPreset> m_powerUps;
    private Color32 l_baseHeartColour;
    private Color l_translatedColour;
    public void Start()
    {
        m_gameController = FindFirstObjectByType<NewGameController>();
        if (m_gameController.m_players[m_whichPlayer] != null)
        {
            m_gameController.m_players[m_whichPlayer].GetComponent<PlayerController>().AttachHUDController(this);
            m_data = ScriptableObject.CreateInstance<PlayerHUDData>();

            l_baseHeartColour = m_gameController.m_players[m_whichPlayer].GetComponent<PlayerSkinController>().m_charData.m_hatColour;
            l_translatedColour = new(l_baseHeartColour.r / 255.0f, l_baseHeartColour.g / 255.0f, l_baseHeartColour.b / 255.0f, l_baseHeartColour.a / 255.0f);

            m_hatIndicator.color = l_translatedColour;
            m_bodyIndicator.color = m_gameController.m_players[m_whichPlayer].GetComponent<PlayerSkinController>().m_charData.m_bodyColour;

            for (int i = 0; i < m_lives.Length; i++)
            {
                m_lives[i].color = l_translatedColour;
            }

            UpdateMagicalOverload();
        } else
        {
            print(m_gameController.aiPlayers.Count);
            switch (m_gameController.aiPlayers.Count)
            {
                case 1:
                    m_whichPlayer -= 3;
                    break;
                case 2:
                    m_whichPlayer -= 2;
                    break;
                case 3:
                    m_whichPlayer -= 1;
                    break;

            }
            m_gameController.aiPlayers[m_whichPlayer].GetComponent<AIHandler>().AttachHUDController(this);
            m_data = ScriptableObject.CreateInstance<PlayerHUDData>();

            l_baseHeartColour = m_gameController.aiPlayers[m_whichPlayer].GetComponent<PlayerSkinController>().m_charData.m_hatColour;
            l_translatedColour = new(l_baseHeartColour.r / 255.0f, l_baseHeartColour.g / 255.0f, l_baseHeartColour.b / 255.0f, l_baseHeartColour.a / 255.0f);

            m_hatIndicator.color = l_translatedColour;
            m_bodyIndicator.color = m_gameController.aiPlayers[m_whichPlayer].GetComponent<PlayerSkinController>().m_charData.m_bodyColour;

            for (int i = 0; i < m_lives.Length; i++)
            {
                m_lives[i].color = l_translatedColour;
            }

            UpdateMagicalOverload();
        }

        SetPowerup(PowerupType.None);
    }

    public void SetPowerup(PowerupType p_elementToSet)
    {
        if (m_powerupIcon != null)
        {
            if (p_elementToSet == PowerupType.None)
            {
                m_powerupIcon.gameObject.SetActive(false);
            }
            else
            {
                m_powerupIcon.gameObject.SetActive(true);
                m_powerupIcon.sprite = m_powerUps[((int)p_elementToSet) - 1].m_elementSprite;
            }
            m_powerUpType = p_elementToSet;
        }
    }

    public PowerupType UsePowerup()
    {
        PowerupType l_usedPowerup = m_powerUpType;
        SetPowerup(PowerupType.None);
        return l_usedPowerup;
    }

    public void UpdateMagicalOverload()
    {
        m_MOhandler.SetMagicalOverload((int)m_data.m_currentMagicalOverload);    
    }

    public bool LoseLife()
    {
        m_lives[m_data.m_currentLives-1].gameObject.SetActive(false);
        m_data.m_currentLives--;

        if(m_data.m_currentLives <= 0)
        {
            return false;
        } else
        {
            return true;
        }
    }
}
