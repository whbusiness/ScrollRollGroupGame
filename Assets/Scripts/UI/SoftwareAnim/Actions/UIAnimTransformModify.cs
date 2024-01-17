using System.Security.Cryptography;
using UnityEngine;

public class UIAnimTransformModify: MonoBehaviour
{
    public RectTransform m_target;
    public bool m_useTargetX;
    public bool m_useTargetY;
    public Vector2 m_startPosition;
    public Vector2 m_targetPosition;
    [SerializeField]
    protected UISoftwareTransformAnimModify m_presetAnim;
    [SerializeField]
    public UISoftwareTransformAnimModify m_localAnim;
    public bool m_overrideToggleOnStart = false;
    public bool m_overrideToggle = false;

    public void Start()
    {
        Vector2 m_realTarget = m_presetAnim.m_targetValue;

        if (m_useTargetX)
        {
           m_realTarget.x = m_targetPosition.x;
        }

        if (m_useTargetY)
        {
            m_realTarget.y = m_targetPosition.y;
        }

        m_localAnim = Instantiate(m_presetAnim);

        if (m_overrideToggleOnStart)
        {
            m_localAnim.m_toggleOnStart = m_overrideToggle;
        }

        m_localAnim.Init(m_presetAnim.m_sourceValue, m_realTarget);
    }

    public void Update()
    {
        var l_vec2 = m_localAnim.Update();
        m_target.localPosition = new Vector3(l_vec2.x, l_vec2.y, m_target.localRotation.z);
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