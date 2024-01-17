

using UnityEngine;

public class ScrollSegment: MonoBehaviour
{
    public GameObject m_spawnPoint;
    public GameObject m_platform;

    [SerializeField]
    [Range(0f, 20f)]
    private float m_platformHeightMod = 1.0f;

    [SerializeField]
    private bool m_doRangomHeightMod = true;
    [SerializeField]
    [Range(-10, 10)]
    private int m_doRangomMaxHeight;
    [SerializeField]
    private bool m_doConstantModifier = true;
    [SerializeField]
    private float m_constantModifier = 12.0f;

    private Vector3 m_calcPos;
    private float m_minBounceMod = -10.0f;
    private float m_maxBounceMod = 10.0f;
    private bool m_isInc = false;
    private float m_lastChange;

    private void Start()
    {
        if(m_platform != null)
        {
            var l_heightMod = Vector3.up * m_platformHeightMod;
            
            if(m_doRangomHeightMod)
            {
                l_heightMod *= Random.Range(-10, m_doRangomMaxHeight);
            }

            if (m_doConstantModifier)
            {
                l_heightMod += Vector3.up * m_constantModifier;
            }

            m_platform.transform.localPosition += l_heightMod;
            m_calcPos = m_platform.transform.localPosition; 
        }
        
        if(Random.Range(0,1) == 0)
        {
            m_isInc = !m_isInc;
            m_lastChange = Time.time;
        }
    }

    private void FixedUpdate()
    {
        var l_platPos = m_platform.transform.localPosition;
        if(m_isInc)
        {
            if (l_platPos.y <= m_maxBounceMod)
            {
                m_platform.transform.localPosition += Vector3.up * Time.fixedDeltaTime;
            } else
            {
                m_isInc = false;
            }
        } else
        {
            if (l_platPos.y >= m_minBounceMod)
            {
                m_platform.transform.localPosition -= Vector3.up * Time.fixedDeltaTime;
            }
            else
            {
                m_isInc = true;
            }
        }
    }
}