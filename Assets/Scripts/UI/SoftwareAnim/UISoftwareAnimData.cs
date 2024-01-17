using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public abstract class UISoftwareAnimData<T> : ScriptableObject
{
    [Header("Software Animation Configuration")]
    [SerializeField]
    public T m_sourceValue;
    [SerializeField]
    public T m_targetValue;
    private T m_refTargetValue;
    [SerializeField]
    public T m_finishValue;

    [Space(10)]
    [Header("Playback Options")]
    [SerializeField]
    public float m_duration;
    [SerializeField]
    protected float m_fpsRate;
    [SerializeField]
    public bool m_toggleOnStart = false;

    [Space(10)]
    [Header("Internal Use Only")]
    public UISoftwareAnimStatus m_status = UISoftwareAnimStatus.None;
    [ReadOnly(true)]
    public float m_startTime;
    [ReadOnly(true)]
    public float m_endTime;

    public void Init(T p_sourceValue, T p_targetValue, float p_duration, float p_fpsRate)
    {
        m_sourceValue = p_sourceValue;
        m_targetValue = p_targetValue;
        m_refTargetValue = p_targetValue;
        m_duration = p_duration;
        m_fpsRate = m_duration / p_fpsRate;
        m_finishValue = m_status == UISoftwareAnimStatus.Running || m_status == UISoftwareAnimStatus.None ? m_sourceValue : m_finishValue;
        UpdateTimes();

        if (m_toggleOnStart)
        {
            StartRun();
        }
    }

    public bool CheckIfAnimStatusRun(UISoftwareAnimStatus p_status)
    {
        return p_status == UISoftwareAnimStatus.Running || p_status == UISoftwareAnimStatus.CompleteRun ? true : false;
    }

    public bool CheckIfAnimStatusReverse(UISoftwareAnimStatus p_status)
    {
        return p_status == UISoftwareAnimStatus.Reversing || p_status == UISoftwareAnimStatus.CompleteReverse ? true : false;
    }

    public void Init(T p_sourceValue, T p_targetValue)
    {
        Init(p_sourceValue, p_targetValue, m_duration, Time.fixedDeltaTime);
    }

    protected abstract T UpdateCurrentValue();

    protected void UpdateTimes()
    {
        m_startTime = Time.time;
        m_endTime = m_startTime + m_duration;
    }

    public void StartReverse()
    {
        m_status = UISoftwareAnimStatus.Reversing;
        UpdateTimes();
    }

    public void StartRun()
    {
        m_status = UISoftwareAnimStatus.Running;
        UpdateTimes();
    }

    public void Toggle()
    {
        if (m_status == UISoftwareAnimStatus.None)
        {
            if (!m_toggleOnStart)
            {
                StartRun();
            }
        }
        else
        {
            if (CheckIfAnimStatusRun(m_status))
            {
                StartReverse();
            }
            else if (CheckIfAnimStatusReverse(m_status))
            {
                StartRun();
            }
        }
    }

    public T Update()
    {
        if (m_status == UISoftwareAnimStatus.Running)
        {
            m_finishValue = UpdateCurrentValue();
            if (Time.time >= m_endTime)
            {
                m_status = UISoftwareAnimStatus.CompleteRun;
            }
        }
        else if (m_status == UISoftwareAnimStatus.Reversing)
        {
            m_finishValue = UpdateCurrentValue();
            if (Time.time >= m_endTime)
            {
                m_status = UISoftwareAnimStatus.CompleteReverse;
            }
        }
        return m_finishValue;
    }
}