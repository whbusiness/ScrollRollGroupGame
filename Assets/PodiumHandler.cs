using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodiumHandler : MonoBehaviour
{
    public GameMatchPlace m_podiumPlace;
    public PlayerController m_player;

    public ParticleSystem m_confettiL;
    public ParticleSystem m_confettiR;

    private bool m_playerPlaced;
    private bool m_confettiLaunched;

    public bool m_confettiFinished;

    private void Start()
    {
        if(m_confettiL.isPlaying) m_confettiL.Stop();
        if(m_confettiR.isPlaying) m_confettiR.Stop();
    }

    private void FixedUpdate()
    {
        if(m_confettiLaunched && !m_confettiFinished)
        {
            if(!m_confettiL.isPlaying && !m_confettiR.isPlaying) {
                m_confettiFinished = true;
            }
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent<PlayerController>(out var l_player) && l_player != null)
        {
            
            if (!m_playerPlaced && !m_confettiLaunched)
            {
                m_playerPlaced = true;
                m_player = l_player;

                if (!m_confettiLaunched)
                {
                    m_confettiLaunched = true;
                    m_confettiL.Play();
                    m_confettiR.Play();
                }
            }
        }
    }
}
