using UnityEngine;

public class UIAnimTextColorModify : MonoBehaviour
{
    public TMPro.TextMeshProUGUI m_targetText;
    public Color m_targetColor;
    [SerializeField]
    public UISoftwareColorAnimFade m_presetAnim;
    [SerializeField]
    public UISoftwareColorAnimFade m_localAnim;
    public bool m_overrideToggleOnStart = false;
    public bool m_overrideToggle = false;

    public void Start()
    {
        if (m_localAnim == null && m_presetAnim != null)
        {
            m_localAnim = Instantiate(m_presetAnim);
        }

        if (m_overrideToggleOnStart)
        {
            m_localAnim.m_toggleOnStart = m_overrideToggle;
        }

        //Initalise local animation, either use this Component's m_targetColor, or the preset animation's target alpha
        m_localAnim.Init(m_presetAnim.m_sourceValue, new Vector4(m_targetColor.r, m_targetColor.g, m_targetColor.b, m_targetColor.a));
    }

    public void Update()
    {
        m_targetText.color = m_localAnim.Update();
    }

    public void StartRun()
    {
        if (m_localAnim == null)
        {
            Start();
        }
        m_localAnim.StartRun();
    }

    public void StartReverse()
    {
        if (m_localAnim == null)
        {
            Start();
        }
        m_localAnim.StartReverse();
    }
}