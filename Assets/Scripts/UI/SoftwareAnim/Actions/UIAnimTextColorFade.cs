using UnityEngine;

public class UIAnimTextColorFade : MonoBehaviour
{
    public TMPro.TextMeshProUGUI m_targetText;
    public float m_targetAlpha = 1.0f;
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

        var l_presetTargetValue = m_presetAnim.m_targetValue;
        //Initalise local animation, either use this Component's m_targetAlpha, or the preset animation's target alpha
        m_localAnim.Init(m_presetAnim.m_sourceValue, new Vector4(l_presetTargetValue.x, l_presetTargetValue.y, l_presetTargetValue.z, m_targetAlpha));
    }

    public void Update()
    {
        m_targetText.color = new Color(m_targetText.color.r, m_targetText.color.g, m_targetText.color.b, m_localAnim.Update().w);
    }

    public void StartRun()
    {
        m_localAnim.StartRun();
    }

    public void StartReverse()
    {
        m_localAnim.StartReverse();
    }
}