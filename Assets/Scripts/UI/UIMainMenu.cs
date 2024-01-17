using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    // Menu Panels
    [Header("Main Panel")]
    public GameObject m_mainMenuPanel;
    [Space]
    [Header("All Panels")]
    public List<GameObject> m_panels;

    [Space(10)]
    [Header("Animation Organisation")]
    [SerializeField]
    private float m_allAnimTime;
    private bool m_hasFirstAnimStart;
    private bool m_hasSecondAnimStart;
    [SerializeField]
    private GameObject m_objToSet;
    private GameObject m_objSetting;

    public UISoftwareColorAnimFade m_colorFade;
    public UISoftwareColorAnimFade m_textFade;

    void Awake()
    {
        //Always assign main menu panel animations
        m_mainMenuPanel.GetComponent<UIAnimationList>().FindAnims(m_textFade, m_colorFade);

        //Check if main menu panel has been added to panels, if not, add it1
        if (!m_panels.Contains(m_mainMenuPanel)){
            m_panels.Add(m_mainMenuPanel);
        }

        //Error handling, make sure UI Menu is set up properly
        if (m_objToSet == null && m_mainMenuPanel != null)
        {
            m_objToSet = m_mainMenuPanel;
        }

        //Setup default animations for all panels found in the scene 
        foreach (var l_panel in m_panels)
        {
            l_panel.GetComponent<UIAnimationList>().FindAnims(m_textFade, m_colorFade);
        }

        // Set the main menu panel active by default
        SetActivePanel(m_mainMenuPanel);
    }

    public void FadeToPanel(GameObject p_panel)
    {
        m_objToSet.GetComponent<UIAnimationList>().ReverseAll();
        m_objSetting = p_panel;
        m_hasFirstAnimStart = true;
        m_allAnimTime = Time.time;
    }

    /// <summary>
    /// Toggle all <see cref="UIAnimImageColorFade"/>s and <see cref="UIAnimTextColorFade"/>s in this scene.
    /// </summary>
    public void ToggleAllAnims()
    {
        m_objToSet.GetComponent<UIAnimationList>().ToggleAll();
    }

    public void LateUpdate()
    {
        if(m_hasFirstAnimStart && Time.time > m_allAnimTime + 1.0f)
        {
            //SetActivePanel(m_objToSet);
            m_objToSet.SetActive(false);
            m_objToSet = m_objSetting;
            m_objToSet.SetActive(true);
            m_objToSet.GetComponent<UIAnimationList>().RunAll();
            m_hasFirstAnimStart = false;
            m_hasSecondAnimStart = true;
            m_allAnimTime = Time.time;
        }else if (m_hasSecondAnimStart && Time.time > m_allAnimTime + 1.0f)
        {
            m_hasSecondAnimStart = false;
        }
    }

    // Sets all panel gameobjects to false, then sets the panel gameobject in the parameter to true
    public void SetActivePanel(GameObject p_panel, bool p_setActive = true)
    {
        foreach(var l_panel in m_panels)
        {
            l_panel.SetActive(false);
        }

        m_objToSet = p_panel;
        if (p_setActive)
        {
            p_panel.SetActive(true);
        }
    }

    /// <summary>
    /// Select the chosen selectable UI component. Used when changing active panels
    /// </summary>
    /// <param name="p_selectable"></param>
    public void SelectNextUIComponent(Selectable p_selectable)
    {
        p_selectable.Select();
    }
}
