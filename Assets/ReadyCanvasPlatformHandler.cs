using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyCanvasPlatformHandler : MonoBehaviour
{
    [SerializeField]
    private float[] m_playerPlatformPos = new float[4];

    [SerializeField]
    private float m_readyZoneWidthPx = 0f;

    [SerializeField]
    private Transform[] m_playerSpawnPoints = new Transform[4];

    [SerializeField]
    private GameObject m_barrierPrefab;
    [SerializeField]
    private Transform[] m_barrierObjects = new Transform[5];

    [SerializeField]
    private NewGameController m_gameController;

    [SerializeField]
    private Camera m_mainCam;

    public void Start()
    {
        m_playerSpawnPoints = m_gameController.m_playerSpawnPoints;

        m_readyZoneWidthPx = Screen.width / m_playerSpawnPoints.Length;

        for (int i = 0; i < m_playerPlatformPos.Length; i++)
        {
            m_playerSpawnPoints[i].position = new Vector3
            {
                x = m_mainCam.ScreenToWorldPoint(new Vector3(((i + 1) * m_readyZoneWidthPx) - (m_readyZoneWidthPx / 2), 0, 0)).x,
                y = m_playerSpawnPoints[i].position.y,
                z = m_playerSpawnPoints[i].position.z
            };
        }

        for(int i = 0; i < m_barrierObjects.Length; i++)
        {
            m_barrierObjects[i] = Instantiate(m_barrierPrefab).transform;

            m_barrierObjects[i].position = new Vector3
            {
                x = m_mainCam.ScreenToWorldPoint(new Vector3(i * m_readyZoneWidthPx, 0, 0)).x,
                y = m_barrierObjects[i].position.y,
                z = 5
            };
        }
    }
}
