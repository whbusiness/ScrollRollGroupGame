using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct PlayerMatchFinishData
{
    public GameMatchPlace m_place;
    public PlayerMatchData m_data;
    public PlayerHUDData m_hudData;
    public PlayerController m_player;
    public AIHandler m_ai;
}

public class PodiumController : MonoBehaviour
{
    public NewGameController m_gameController;
    public List<PlayerMatchFinishData> m_players;
    public List<Transform> m_places;
    public float m_playerScaleSpeed = 0.98f;

    public List<PlayerPodiumPanelHandler> m_panels;
    public List<RenderTexture> m_renderTextures;

    // Start is called before the first frame update
    void Start()
    {
        m_gameController = FindFirstObjectByType<NewGameController>();

        int l_playerCount = 0;

        m_players = new List<PlayerMatchFinishData>();

        foreach (var l_player in m_gameController.m_players)
        {
            if (l_player != null)
            {
                var l_playerController = l_player.GetComponent<PlayerController>();
                l_playerController.m_matchData.m_isInMatch = false;

                switch (l_playerController.m_matchData.m_matchPlace)
                {
                    case GameMatchPlace.First:
                        l_playerController.transform.position = m_places[0].position;
                        break;
                    case GameMatchPlace.Second:
                        l_playerController.transform.position = m_places[1].position;
                        break;
                    case GameMatchPlace.Third:
                        l_playerController.transform.position = m_places[2].position;
                        break;
                    case GameMatchPlace.Fourth:
                        l_playerController.transform.position = m_places[3].position;
                        break;
                    default:
                        Debug.LogError("Invalid GameMatchPlace!! + " + l_playerController.m_matchData.m_matchPlace);
                        break;
                }

                m_players.Add(new PlayerMatchFinishData
                {
                    m_place = l_playerController.m_matchData.m_matchPlace,
                    m_data = l_playerController.m_matchData,
                    m_hudData = l_playerController.m_hudController.m_data,
                    m_player = l_playerController
                });

                l_playerController.m_rb.isKinematic = false;
                l_playerCount++;
            }
        }
        foreach (var l_aiPlayer in m_gameController.aiPlayers)
        {
            if (l_aiPlayer != null)
            {
                var l_playerController = l_aiPlayer.GetComponent<AIHandler>();
                l_playerController.m_matchData.m_isInMatch = false;

                switch (l_playerController.m_matchData.m_matchPlace)
                {
                    case GameMatchPlace.First:
                        l_playerController.transform.position = m_places[0].position;
                        break;
                    case GameMatchPlace.Second:
                        l_playerController.transform.position = m_places[1].position;
                        break;
                    case GameMatchPlace.Third:
                        l_playerController.transform.position = m_places[2].position;
                        break;
                    case GameMatchPlace.Fourth:
                        l_playerController.transform.position = m_places[3].position;
                        break;
                    default:
                        Debug.LogError("Invalid GameMatchPlace!! + " + l_playerController.m_matchData.m_matchPlace);
                        break;
                }

                m_players.Add(new PlayerMatchFinishData
                {
                    m_place = l_playerController.m_matchData.m_matchPlace,
                    m_data = l_playerController.m_matchData,
                    m_hudData = l_playerController.m_hudController.m_data,
                    m_ai = l_playerController
                });

                l_playerController.GetComponent<Rigidbody>().isKinematic = false;
                l_playerCount++;
            }
        }

        for (int i=0; i<m_panels.Count; i++)
        {
            m_panels[i].gameObject.SetActive(false);
        }

        for(int i=0; i<m_players.Count; i++)
        {
            if(i < m_panels.Count)
            {
                m_panels[(int)m_players[i].m_place].gameObject.SetActive(true);
                if (m_players[i].m_player != null)
                {
                    m_panels[(int)m_players[i].m_place].m_playerName.text = "Player " + (m_players[i].m_player.m_playerID + 1).ToString();
                }
                else
                {
                    m_panels[(int)m_players[i].m_place].m_playerName.text = "Player " + (m_players[i].m_ai.m_playerID + 1).ToString();
                }

                m_panels[(int)m_players[i].m_place].SetPlayerStats(m_players[i].m_data);

                m_panels[(int)m_players[i].m_place].m_rawImage.texture = m_renderTextures[i];
            }
        }

    }

    public void FixedUpdate()
    {
        int l_playerCount = 0;
        foreach(var l_player in m_players)
        {
            if (l_player.m_player == null)
            {
                l_playerCount++;
            }
        }

        if(l_playerCount == m_players.Count)
        {
            Destroy(m_gameController.gameObject);
            Cursor.lockState = CursorLockMode.None; // Unlock the cursor
            Cursor.visible = true; // Make the cursor visible
            SceneManager.LoadScene(0);
        }
    }

    private float Compare(float p_a, float p_b)
    {
        return p_a > p_b ? p_a : p_b;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
