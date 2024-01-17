using UnityEngine;

[CreateAssetMenu(fileName = "New Color Fade SoftwareAnim", menuName = "ScrollRoll/SoftwareAnim/Color Fade")]
public class UISoftwareColorAnimFade : UISoftwareAnimData<Vector4>
{
    protected override Vector4 UpdateCurrentValue()
    {
        var l_calcValue = (Time.time - m_startTime) / (m_endTime - m_startTime);
        return CheckIfAnimStatusRun(m_status) || !CheckIfAnimStatusReverse(m_status) ?
            new Vector4()
            {
                x = Mathf.Lerp(m_sourceValue.x, m_targetValue.x, l_calcValue),
                y = Mathf.Lerp(m_sourceValue.y, m_targetValue.y, l_calcValue),
                z = Mathf.Lerp(m_sourceValue.z, m_targetValue.z, l_calcValue),
                w = Mathf.Lerp(m_sourceValue.w, m_targetValue.w, l_calcValue)
            }
            :
            new Vector4()
            {   
                x = Mathf.Lerp(m_targetValue.x, m_sourceValue.x, l_calcValue),
                y = Mathf.Lerp(m_targetValue.y, m_sourceValue.y, l_calcValue),
                z = Mathf.Lerp(m_targetValue.z, m_sourceValue.z, l_calcValue),
                w = Mathf.Lerp(m_targetValue.w, m_sourceValue.w, l_calcValue)
            };
    }
}