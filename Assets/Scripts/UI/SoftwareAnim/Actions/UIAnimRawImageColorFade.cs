using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class UIAnimRawImageColorFade : MonoBehaviour
{
    public RawImage m_targetImage;
    public float m_targetAlpha = 1.0f;
    public bool m_useTargetAlpha;
    public UISoftwareColorAnimFade m_presetAnim;
    public UISoftwareColorAnimFade m_localAnim;
    public UnityEvent m_onRunFinished;
    public UnityEvent m_onReverseFinished;
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
        m_localAnim.Init(m_presetAnim.m_sourceValue, (m_useTargetAlpha ? m_presetAnim.m_targetValue : new Vector4(l_presetTargetValue.x, l_presetTargetValue.y, l_presetTargetValue.z, m_targetAlpha)));
    }

    public void Update()
    {
        m_targetImage.color = m_targetImage.color = new Color(m_targetImage.color.r, m_targetImage.color.g, m_targetImage.color.b, m_localAnim.Update().w);
    }

    public void LateUpdate()
    {
        if (m_onRunFinished != null && m_localAnim.m_status == UISoftwareAnimStatus.CompleteRun)
        {
            m_onRunFinished.Invoke();
        }
        else if (m_onReverseFinished != null && m_localAnim.m_status == UISoftwareAnimStatus.CompleteReverse)
        {
            m_onReverseFinished.Invoke();
        }
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