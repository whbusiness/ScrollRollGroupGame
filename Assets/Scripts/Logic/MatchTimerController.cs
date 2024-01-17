using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MatchBiomePhase
{
    PreBiome,
    FullBiome,
    PostBiome
}

public class MatchTimerController : MonoBehaviour
{
    public MatchController m_matchController;
    [Header("Scroll to Control")]
    public ScrollController m_scrollController;

    public ParticleSystem m_particleSystem;
    public float m_emissionInc = 0.05f;
    public float m_minEmission = 15.0f;
    public float m_maxEmission = 120.0f;
    [SerializeField]
    private float m_currentEmission;
    public float m_emissionSpeedInc = 0.05f;
    public float m_minEmissionSpeed = 1.0f;
    public float m_maxEmissionSpeed = 7.5f;
    [SerializeField]
    private float m_currentEmissionSpeed;
    
    [Space(10)]
    [Header("Biome Info")]
    [SerializeField]
    private ScrollBiome m_nextBiome;
    public MatchBiomePhase m_currentPhase;

    [Space(10)]
    [Header("Indicator Configuration")]
    public Sprite m_biomeUnknownImage;
    public List<Image> m_biomeIndicator;
    [Space]
    [SerializeField]
    private MatchBiomePhase m_indicatorPhase;
    [SerializeField]
    private Vector2 m_indicatorStartPos;
    [SerializeField]
    private Vector2 m_indicatorEndPos;
    [SerializeField]
    private Vector2 m_indicatorSpeed;
    [SerializeField]
    private bool m_isIndicatorMovingUp;
    [SerializeField]
    private bool m_isIndicatorMovingDown;
    [SerializeField]
    private bool m_hasIndicatorMovedUp;
    [SerializeField]
    private bool m_hasIndicatorMovedDown;

    [Space(10)]
    [Header("Timer Configuration")]
    public Image m_minutes;
    public Image m_secondsTens;
    public Image m_secondsDigits;
    public List<Sprite> m_images;
    public Color m_pulseColor = Color.yellow;
    [Space()]
    [Header("Timer Info")]
    public float m_matchTimeLeft;
    public float m_matchTimeStarted;
    [SerializeField]
    private float m_lastTimeTimerUpdated;

    [Space(10)]
    [Header("Timings")]
    public float m_matchDuration = 180.0f;
    public int m_biomeCount = 4;
    public float m_matchTimePadding = 10.0f;
    private float m_lastBiomeEndTime;
    private int m_biomesHad = 1;
    private bool m_isPlaying;
    private float m_biomeTime;

    private void Start()
    {
        m_biomeTime = m_matchDuration / m_biomeCount;
        m_matchTimeStarted = Time.time;
        m_matchTimeLeft = m_matchDuration;
        m_lastTimeTimerUpdated = Time.time;
        UpdateMatchTimer();

        FetchNextBiome();

        m_particleSystem.Stop();
    }

    public void FetchNextBiome()
    {
        if(m_nextBiome.m_preset != null)
        {
            m_lastBiomeEndTime = m_nextBiome.m_preset.m_biomeEnd;
        } else
        {
            m_lastBiomeEndTime = Time.time;
        }
        m_nextBiome = m_scrollController.SetNextBiome();
        m_nextBiome.m_preset.m_biomeStart = m_lastBiomeEndTime;
        m_nextBiome.m_preset.m_biomeEnd = m_lastBiomeEndTime + m_biomeTime;
        m_nextBiome.m_preset.m_biomePreTimeStart = m_lastBiomeEndTime;
        m_nextBiome.m_preset.m_biomeTimeStart = m_lastBiomeEndTime + m_matchTimePadding;
        m_nextBiome.m_preset.m_biomePostTimeStart = m_nextBiome.m_preset.m_biomeEnd - m_matchTimePadding;

        //m_biomeIndicator.sprite = m_nextBiome.m_preset.m_sprite;
    }

    private void ResetImageColors()
    {
        m_isPlaying = false;

        foreach (var l_image in m_biomeIndicator)
        {
            l_image.color = Color.white;
        }

        m_minutes.color = Color.white;
        m_secondsTens.color = Color.white;
        m_secondsDigits.color = Color.white;
    }

    private void FixedUpdate()
    {
        if (m_matchController.m_phase == MatchPhase.FullMatch || m_matchController.m_phase == MatchPhase.SuddenDeath)
        {
            if (Time.time >= m_lastTimeTimerUpdated + 1.0f)
            {
                m_lastTimeTimerUpdated = Time.time;
                m_matchTimeLeft--;
                UpdateMatchTimer();
            }

            //If this biome has started
            if (Time.time >= m_nextBiome.m_preset.m_biomeStart)
            {
                if (Time.time >= m_nextBiome.m_preset.m_biomePreTimeStart && Time.time < m_nextBiome.m_preset.m_biomeTimeStart)
                {
                    m_currentPhase = MatchBiomePhase.PreBiome;
                    if (m_indicatorPhase != MatchBiomePhase.PreBiome)
                    {
                        m_indicatorPhase = MatchBiomePhase.PreBiome;
                        m_biomeIndicator[0].sprite = m_biomeUnknownImage;

                        ResetImageColors();
                    }
                    else
                    {
                        if (Time.time >= m_nextBiome.m_preset.m_biomePreTimeStart + (m_matchTimePadding / 2))
                        {
                            m_isPlaying = true;
                        }
                    }
                }
                else if (Time.time >= m_nextBiome.m_preset.m_biomeTimeStart && Time.time < m_nextBiome.m_preset.m_biomePostTimeStart)
                {
                    m_currentPhase = MatchBiomePhase.FullBiome;
                    if (m_indicatorPhase != MatchBiomePhase.FullBiome)
                    {
                        if (m_indicatorPhase == MatchBiomePhase.PreBiome)
                        {
                            m_indicatorPhase = MatchBiomePhase.FullBiome;
                            m_biomeIndicator[0].sprite = m_nextBiome.m_preset.m_sprite;
                            m_scrollController.GoToNextBiome();

                            ResetImageColors();
                        }
                    }
                }
                else if (Time.time >= m_nextBiome.m_preset.m_biomePostTimeStart && Time.time < m_nextBiome.m_preset.m_biomeEnd)
                {
                    m_currentPhase = MatchBiomePhase.PostBiome;

                    if (m_indicatorPhase != MatchBiomePhase.PostBiome)
                    {
                        if (m_indicatorPhase == MatchBiomePhase.FullBiome)
                        {
                            m_indicatorPhase = MatchBiomePhase.PostBiome;

                            ResetImageColors();
                        }
                    }
                    else
                    {
                        if (Time.time >= m_nextBiome.m_preset.m_biomePostTimeStart + (m_matchTimePadding / 2))
                        {
                            m_biomesHad++;
                            FetchNextBiome();
                            m_isPlaying = true;
                        }
                    }
                }
            }

            if (m_isPlaying)
            {
                foreach (var l_image in m_biomeIndicator)
                {
                    l_image.color = Color.Lerp(Color.white, Color.clear, Mathf.PingPong(Time.time, 1));
                }

                m_minutes.color = Color.Lerp(Color.white, Color.yellow, Mathf.PingPong(Time.time, 1));
                m_secondsTens.color = Color.Lerp(Color.white, Color.yellow, Mathf.PingPong(Time.time, 1));
                m_secondsDigits.color = Color.Lerp(Color.white, Color.yellow, Mathf.PingPong(Time.time, 1));

                m_scrollController.RollPower();

                if (m_indicatorPhase == MatchBiomePhase.PreBiome)
                {
                    if (Time.time >= m_nextBiome.m_preset.m_biomeTimeStart - (m_matchTimePadding / 4))
                    {

                        if (!m_particleSystem.isPlaying)
                        {
                            m_particleSystem.Play();
                        }

                        var l_emission = m_particleSystem.emission;
                        l_emission.rateOverTime = m_currentEmission < m_maxEmission ? m_currentEmission += m_emissionInc : m_currentEmission;

                        var l_speed = m_particleSystem.main;
                        l_speed.startSpeed = m_currentEmissionSpeed < m_maxEmissionSpeed ? m_currentEmissionSpeed += m_emissionSpeedInc : m_currentEmissionSpeed;
                    }
                }
            }
            else
            {
                m_scrollController.RollIdle();
                if (m_particleSystem.isPlaying)
                {
                    m_particleSystem.Stop();

                    m_currentEmission = m_minEmission;
                    m_currentEmissionSpeed = m_minEmissionSpeed;

                    var l_emission = m_particleSystem.emission;
                    l_emission.rateOverTime = m_minEmission;
                    var l_speed = m_particleSystem.main;
                    l_speed.startSpeed = m_minEmissionSpeed;
                }
            }
        }
    }

    public void UpdateMatchTimer()
    {
        var l_minutes = 0;
        var l_tens = 0;
        var l_seconds = 0;

        //Count whole minutes
        for(int i=0; i < (m_matchDuration / 60.0f); i++)
        {
            if((m_matchTimeLeft - (l_minutes * 60.0f)) - 60.0f > 0)
            {
                l_minutes++;
            } else
            {
                break;
            }
        }

        //Assign minutes sprite
        m_minutes.sprite = m_images[l_minutes];

        //Calculate amount of tens left
        l_seconds = (int)(m_matchTimeLeft - (l_minutes * 60));

        if (l_seconds == 60)
        {
            m_minutes.sprite = m_images[l_minutes + 1];
            m_secondsTens.sprite = m_images[0];
            m_secondsDigits.sprite = m_images[0];
        }
        else
        {
            l_tens = l_seconds / 10;
            m_secondsTens.sprite = m_images[l_tens];


            l_seconds -= l_tens * 10;
            if (l_seconds >= 0)
            {
                m_secondsDigits.sprite = m_images[l_seconds];
            }
            else
            {
                //Match finished!
            }
        }
    }

}
