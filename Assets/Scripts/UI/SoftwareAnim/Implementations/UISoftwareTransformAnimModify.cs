using UnityEngine;

[CreateAssetMenu(fileName = "New Transform Modify SoftwareAnim", menuName = "ScrollRoll/SoftwareAnim/Transform Modify")]
public class UISoftwareTransformAnimModify : UISoftwareAnimData<Vector2>
{
    protected override Vector2 UpdateCurrentValue()
    {
        m_finishValue.x = CheckIfAnimStatusRun(m_status) || !CheckIfAnimStatusReverse(m_status) ? Mathf.Lerp(m_sourceValue.x, m_targetValue.x, (Time.time - m_startTime) / m_duration) : Mathf.Lerp(m_targetValue.x, m_sourceValue.x, (Time.time - m_startTime) / m_duration);
        m_finishValue.y = CheckIfAnimStatusRun(m_status) || !CheckIfAnimStatusReverse(m_status) ? Mathf.Lerp(m_sourceValue.y, m_targetValue.y, (Time.time - m_startTime) / m_duration) : Mathf.Lerp(m_targetValue.y, m_sourceValue.y, (Time.time - m_startTime) / m_duration);
        return m_finishValue;
    }
}