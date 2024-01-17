using System.Collections;
using UnityEngine;

public class PlayerSkinController : MonoBehaviour
{
    public bool m_generateRandomColour = true;

    public PlayerCharacterData m_charData = new PlayerCharacterData();
    public SkinnedMeshRenderer m_hatRenderer;
    public SkinnedMeshRenderer m_bodyRenderer;
    public MeshRenderer m_wandRenderer;

    public void Awake()
    {
        if(m_generateRandomColour)
        {
            m_charData = PlayerCharacterData.RandomCharacterData();
        } else
        {
            m_charData = PlayerCharacterData.LoadCharacterData();
        }
        ApplyCharacterColours();
    }

    public void ApplyCharacterColours()
    {
        m_hatRenderer.material.SetColor("_BASE_COLOR", GetColorFromColor32(m_charData.m_hatColour));
        //m_hatTopRenderer.material.color = GetColorFromColor32(m_charData.m_hatColour);
        m_bodyRenderer.material.SetColor("_BASE_COLOR", GetColorFromColor32(m_charData.m_bodyColour));
        m_wandRenderer.material.SetColor("_BASE_COLOR", GetColorFromColor32(m_charData.m_wandColour));
    }

    public IEnumerator DamageRed(float dur)
    {
        StopCoroutine(nameof(DamageRed));
        while(true)
        {
            print("CHANGING Colour");
            m_hatRenderer.material.SetColor("_BASE_COLOR", Color.red);
            m_bodyRenderer.material.SetColor("_BASE_COLOR", Color.red);
            m_wandRenderer.material.SetColor("_BASE_COLOR", Color.red);
            yield return new WaitForSeconds(dur);
            print("STOP CHANGING COLOUR");
            m_hatRenderer.material.SetColor("_BASE_COLOR", GetColorFromColor32(m_charData.m_hatColour));
            m_bodyRenderer.material.SetColor("_BASE_COLOR", GetColorFromColor32(m_charData.m_bodyColour));
            m_wandRenderer.material.SetColor("_BASE_COLOR", GetColorFromColor32(m_charData.m_wandColour));
            yield break;
        }
    }

    public void SetPlayerID(string p_id)
    {
        m_charData.SetPlayerID(p_id);
    }

    public static Color GetColorFromColor32(Color32 p_inputColor)
    {
        return new Color()
        {
            r = p_inputColor.r / 255.0f,
            g = p_inputColor.g / 255.0f,
            b = p_inputColor.b / 255.0f,
            a = p_inputColor.a / 255.0f
        };
    }
}