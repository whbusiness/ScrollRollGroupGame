using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UICharacterEditor : MonoBehaviour
{
    public PlayerCharacterData m_charData;
    public PlayerCharCustomisable m_part;

    public UIAnimationList m_colourPickerAnim;
    private  UIColorSelector m_colorSelector;
    public List<GameObject> m_configPanels;
    public bool m_showColourSelect = false;
    public float m_lastMenuTime; 

    public GameObject m_hat;
    public GameObject m_hatTop;
    public GameObject m_body;
    public GameObject m_wand;

    public Image m_hatPreview;
    public Image m_bodyPreview;
    public Image m_wandPreview;

    private EventSystem m_eventSystem;
    private GameObject m_firstSelected;

    void Awake()
    {
        m_eventSystem = (EventSystem)FindFirstObjectByType(typeof(EventSystem));
        m_firstSelected = m_eventSystem.firstSelectedGameObject;
        m_colorSelector = m_colourPickerAnim.GetComponent<UIColorSelector>();
        m_colorSelector.gameObject.SetActive(false);
        if(File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + PlayerCharacterData.PLAYER_CHAR_DATA_FILE_NAME))
        {
            m_charData = PlayerCharacterData.LoadCharacterData();
            UpdateColours();
        } else
        {
            m_charData = new PlayerCharacterData();
            m_charData.m_hatColour = Color.red;
            m_charData.m_bodyColour = Color.grey;
            m_charData.m_wandColour = Color.green;
        }
    }

    public void LateUpdate()
    {
        if (m_lastMenuTime > 0)
        {
            if(Time.time > m_lastMenuTime)
            {
                if (m_showColourSelect)
                {
                    foreach(GameObject p_panel in m_configPanels)
                    {
                        p_panel.SetActive(false);
                    }

                    m_colourPickerAnim.gameObject.SetActive(true);
                    m_colourPickerAnim.RunAll();
                    m_lastMenuTime = 0;
                } else
                {
                    m_colourPickerAnim.gameObject.SetActive(false);

                    foreach(GameObject p_panel in m_configPanels)
                    {
                        p_panel.SetActive(true);
                        p_panel.GetComponent<UIAnimationList>().RunAll();
                    }

                    m_eventSystem.SetSelectedGameObject(m_firstSelected);
                    m_lastMenuTime = 0;
                }
            }
        }
    }

    public void SelectHat()
    {
        m_colorSelector.m_targetObject = m_hat;
        m_colorSelector.m_targetObjectFallback = m_hatTop;
        m_colorSelector.SetColour(m_charData.m_hatColour.r, m_charData.m_hatColour.g, m_charData.m_hatColour.b, m_charData.m_hatColour.a);
        print(m_charData.m_hatColour.ToString());
        print(m_colorSelector.m_outputColor.ToString());
        SelectPart(PlayerCharCustomisable.Hat);
    }

    public void SelectBody()
    {
        m_colorSelector.m_targetObject = m_body;
        m_colorSelector.m_targetObjectFallback = null;
        m_colorSelector.SetColour(m_charData.m_bodyColour.r, m_charData.m_bodyColour.g, m_charData.m_bodyColour.b, m_charData.m_bodyColour.a);
        SelectPart(PlayerCharCustomisable.Body);
    }

    public void SelectWand()
    {
        m_colorSelector.m_targetObject = m_wand;
        m_colorSelector.m_targetObjectFallback = null;
        m_colorSelector.SetColour(m_charData.m_wandColour.r, m_charData.m_wandColour.g, m_charData.m_wandColour.b, m_charData.m_wandColour.a);
        SelectPart(PlayerCharCustomisable.Wand);
    }

    private void SelectPart(PlayerCharCustomisable p_part)
    {
        m_part = p_part;
        foreach(GameObject p_panel in m_configPanels)
        {
            p_panel.GetComponent<UIAnimationList>().ReverseAll();
            m_showColourSelect = true;
            m_lastMenuTime = Time.time + 1.0f;
        }

        m_eventSystem.SetSelectedGameObject(m_colorSelector.m_redSlider.gameObject);
    }

    public void SavePart()
    {
        switch (m_part)
        {
            case PlayerCharCustomisable.Hat:
                m_charData.m_hatColour = m_colorSelector.m_outputColor;
                break;
            case PlayerCharCustomisable.Wand:
                m_charData.m_wandColour = m_colorSelector.m_outputColor;
                break;
            case PlayerCharCustomisable.Body:
                m_charData.m_bodyColour = m_colorSelector.m_outputColor;
                break;
        }

        UpdateColours();
        m_charData.SaveCharacterData();
        CancelPart();
    }

    public void UpdateColours()
    {
        m_hat.GetComponent<MeshRenderer>().sharedMaterial.SetColor("_BaseColor", m_charData.m_hatColour);
        m_hatTop.GetComponent<MeshRenderer>().sharedMaterial.SetColor("_BaseColor", m_charData.m_hatColour);
        m_body.GetComponent<MeshRenderer>().sharedMaterial.SetColor("_BaseColor", m_charData.m_bodyColour);
        m_wand.GetComponent<MeshRenderer>().sharedMaterial.SetColor("_BaseColor", m_charData.m_wandColour);

        m_hatPreview.color = m_charData.m_hatColour;
        m_bodyPreview.color = m_charData.m_bodyColour;
        m_wandPreview.color = m_charData.m_wandColour;
    }

    public void CancelPart()
    {
        UpdateColours();
        m_colourPickerAnim.ReverseAll();
        m_showColourSelect = false;
        m_lastMenuTime = Time.time + 1.0f;
    }
}
