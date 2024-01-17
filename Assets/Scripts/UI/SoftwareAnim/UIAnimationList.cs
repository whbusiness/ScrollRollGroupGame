using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimationList : MonoBehaviour
{
    [Header("Animation List Configuration")]
    [Tooltip("Should this Animation List find its' animations on Start? (DO NOT set to TRUE if using a UIMainMenu.")]
    public bool m_findAnimsOnStart = false;
    public bool m_runOnStart = false;
    public UISoftwareColorAnimFade m_defaultColourFade;
    public UISoftwareColorAnimFade m_defaultTextFade;

    [Space]
    public List<UIAnimImageColorFade> m_colors;
    public List<UIAnimTextColorFade> m_textColors;

    public void Start()
    {
        if (m_findAnimsOnStart)
        {
            FindAnims(m_defaultTextFade, m_defaultColourFade);
        }

        if (m_runOnStart)
        {
            RunAll();
        }
    }

    public void FindAnims(UISoftwareColorAnimFade l_defaultTextFade, UISoftwareColorAnimFade l_defaultColorFade)
    {
        foreach(var l_colorFade in gameObject.GetComponentsInChildren<Image>())
        {
            if (l_colorFade == null) continue;
            else
            {
                if (!l_colorFade.gameObject.TryGetComponent<UIAnimListIgnore>(out var l_ignore))
                {
                    if (!l_colorFade.gameObject.TryGetComponent<UIAnimImageColorFade>(out var l_tempColourFade))
                    {
                        l_tempColourFade = l_colorFade.gameObject.AddComponent<UIAnimImageColorFade>();
                        l_tempColourFade.enabled = true;
                        l_tempColourFade.m_targetImage = l_colorFade;
                        l_tempColourFade.m_presetAnim = l_defaultColorFade;
                        l_tempColourFade.m_localAnim = Instantiate(l_defaultColorFade);
                    }
                    m_colors.Add(l_tempColourFade);
                }
            }
        }

        foreach (var l_text in gameObject.GetComponentsInChildren<TMPro.TextMeshProUGUI>())
        {
            if (l_text == null) continue;
            else
            {
                if (!l_text.gameObject.TryGetComponent<UIAnimListIgnore>(out var l_ignore))
                {
                    if (!l_text.gameObject.TryGetComponent<UIAnimTextColorFade>(out var l_tempTextFade))
                    {
                        l_tempTextFade = l_text.gameObject.AddComponent<UIAnimTextColorFade>();
                        l_tempTextFade.enabled = true;
                        l_tempTextFade.m_targetText = l_text;
                        l_tempTextFade.m_presetAnim = l_defaultTextFade;
                        l_tempTextFade.m_localAnim = Instantiate(l_defaultTextFade);
                    }
                    m_textColors.Add(l_tempTextFade);
                }
            }
        }
    }

    public void ToggleAll()
    {
        ToggleColours();
        ToggleTexts();
    }

    public void RunAll()
    {
        foreach(var l_colorFade in m_colors)
        {
            l_colorFade.StartRun();
        }

        foreach(var l_text in m_textColors)
        {
            l_text.StartRun();
        }
    }

    public void ReverseAll()
    {
        foreach (var l_colorFade in m_colors)
        {
            l_colorFade.StartReverse();
        }

        foreach (var l_text in m_textColors)
        {
            l_text.StartReverse();
        }
    }

    public void ToggleColours()
    {
        foreach(var l_colorFade in m_colors)
        {
            l_colorFade.m_localAnim.Toggle();
        }
    }

    public void ToggleTexts() 
    { 
        foreach(var l_text in m_textColors)
        {
            l_text.m_localAnim.Toggle();
        }
    }
}