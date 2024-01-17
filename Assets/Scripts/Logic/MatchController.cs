using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public enum MatchPhase
{
    FullMatch,
    SuddenDeath,
    End,
    Podium
}

public class MatchController : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    private NewGameController m_gameController;
    [SerializeField]
    private MatchTimerController m_timerController;
    [SerializeField]
    private CameraZoomer m_cameraController;
    [SerializeField]
    private Camera m_cameraToControl;
    [SerializeField]
    private Transform m_cameraTransform;
    public List<PlayerController> m_players;
    public PlayerController m_playerWon;
    public AIHandler m_aiWon;
    public GameObject m_aiOrPlayWon;
    public float m_winScaleSpeed = 1.005f;
    public float m_winTime = 5.0f;
    private float m_timeWon;
    private Vector3 m_startScale;
    public float m_winScale;
    public GameObject m_crownPrefab;
    public Transform m_crown;
    public float m_crownSpawnAdj;
    public int m_playersAlive;
    public static float MatchTime { get { return Time.time; } set { } }
    public const float m_minute = 60.0f;
    public float m_matchLength = m_minute * 3;
    public MatchPhase m_phase;
    [SerializeField]
    private ScrollController m_scroll;

    public float m_minimumSpawnX;
    public float m_maximumSpawnX;

    public List<Transform> m_spawnLocations;

    [SerializeField]
    private bool m_isSuddenDeath;

    [Header("Sudden Death Indicators")]
    [SerializeField]
    private GameObject m_suddenDeathPanel;
    [SerializeField]
    private Volume m_suddenDeathPostProVolume;
    [SerializeField]
    private float m_suddenDeathVolumeLerpDuration;
    private List<AIHandler> aiPlayers = new();

    /// <summary>
    /// 
    /// </summary>
    public void Start()
    {
        m_gameController = FindFirstObjectByType<NewGameController>();
        m_gameController.AttachMatchController(this);

        m_matchLength += Time.time;

        foreach(var l_player in FindObjectsOfType<PlayerController>())
        {
            m_players.Add(l_player);

            int l_randomSpawn = Random.Range(0, m_spawnLocations.Count);
            m_players[^1].transform.position = m_spawnLocations[l_randomSpawn].position;
            m_players[^1].m_rb.velocity = Vector3.zero;
            print(m_spawnLocations[l_randomSpawn].position);
            m_spawnLocations.Remove(m_spawnLocations[l_randomSpawn]);
            m_playersAlive++;
        }
        foreach(var l_aiPlayer in FindObjectsOfType<AIHandler>())
        {
            aiPlayers.Add(l_aiPlayer);

            int l_randomSpawn = Random.Range(0, m_spawnLocations.Count);
            aiPlayers[^1].transform.position = m_spawnLocations[l_randomSpawn].position;
            aiPlayers[^1].GetComponent<Rigidbody>().velocity = Vector3.zero;
            print(m_spawnLocations[l_randomSpawn].position);
            m_spawnLocations.Remove(m_spawnLocations[l_randomSpawn]);
            m_playersAlive++;
        }

        if (m_cameraController.cm == null) m_cameraController.cm = m_cameraToControl;
        //m_cameraController.FetchCameraPositioners();

        HideSuddenDeathIndicators();
    }

    public void FixedUpdate()
    {
        switch (m_phase)
        {
            case MatchPhase.FullMatch:
                    if (m_playersAlive <= 1)
                    {
                        OnMatchWin();
                    }
                    if (MatchTime > m_matchLength)
                    {
                        if (m_playersAlive == 2)
                        {
                            OnMatchTie();
                        }
                    }
                break;
            case MatchPhase.SuddenDeath:
                foreach(var p_indicatorImage in m_timerController.m_biomeIndicator)
                {
                    p_indicatorImage.color = Color.Lerp(Color.red, Color.yellow, Mathf.PingPong(Time.time, 1.0f));
                }
                m_timerController.m_pulseColor = Color.red;
                PlaySuddenDeathTextAnim();

                if (m_playersAlive <= 1)
                {
                    OnMatchWin();
                }
                break;
            case MatchPhase.End:

                if (m_aiOrPlayWon == null)
                {
                    foreach(var l_player in m_players)
                    {
                        if(l_player.m_hudController.m_data.m_currentLives != 0)
                        {
                            m_aiOrPlayWon = l_player.gameObject;
                        }
                    }
                    foreach(var l_aiPlayer in aiPlayers)
                    {
                        if (l_aiPlayer.m_hudController.m_data.m_currentLives != 0)
                        {
                            m_aiOrPlayWon = l_aiPlayer.gameObject;
                        }
                    }


                    foreach (var l_player in m_players)
                    {
                        if (l_player.gameObject == m_aiOrPlayWon)
                        {
                            m_timeWon = Time.time;
                            m_playerWon = l_player;
                            m_startScale = m_playerWon.transform.localScale;
                            m_playerWon.m_matchData.m_isInMatch = false;

                            if (!m_playerWon.m_crown.activeSelf) m_playerWon.m_crown.SetActive(true);
                            break;
                        }
                    }
                    foreach (var l_aiPlayer in aiPlayers)
                    {
                        if (l_aiPlayer.gameObject == m_aiOrPlayWon)
                        {
                            m_timeWon = Time.time;
                            m_aiWon = l_aiPlayer;
                            m_startScale = m_aiWon.transform.localScale;
                            m_aiWon.m_matchData.m_isInMatch = false;

                            if (!m_aiWon.m_crown.activeSelf) m_aiWon.m_crown.SetActive(true);
                            break;
                        }
                    }
                }
                if (m_playerWon != null)
                {
                    if (m_playerWon.transform.localScale != m_startScale * m_winScale)
                    {
                        m_playerWon.transform.localScale *= m_winScaleSpeed;
                        m_playerWon.m_crown.transform.localScale *= m_winScaleSpeed;

                        if (m_playerWon.transform.localScale.y >= m_startScale.y * m_winScale)
                        {
                            m_phase = MatchPhase.Podium;
                        }
                    }
                }
                    if(m_aiWon != null)
                    {
                        if (m_aiWon.transform.localScale != m_startScale * m_winScale)
                        {
                            m_aiWon.transform.localScale *= m_winScaleSpeed;
                            m_aiWon.m_crown.transform.localScale *= m_winScaleSpeed;

                            if (m_aiWon.transform.localScale.y >= m_startScale.y * m_winScale)
                            {
                                m_phase = MatchPhase.Podium;
                            }
                        }

                    }

                break;
            case MatchPhase.Podium:
                if (Time.time > m_timeWon + m_winTime)
                {
                    if(m_playerWon == null)
                    {
                        m_aiWon.transform.localScale = m_startScale;
                        m_aiWon.stunned = true;
                        if (m_aiWon.m_crown.activeSelf) m_aiWon.m_crown.SetActive(false);
                    }
                    else
                    {
                        m_playerWon.transform.localScale = m_startScale;
                        m_playerWon.stunned = true;
                        if (m_playerWon.m_crown.activeSelf) m_playerWon.m_crown.SetActive(false);
                    }
                    SceneManager.LoadScene(5);
                }
                break;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public int OnPlayerKnockout()
    {
        print("Player knocked out!: Players alive: " + m_playersAlive);
        m_playersAlive--;
        print("Player placed: " + (GameMatchPlace)m_playersAlive);

        if(m_playersAlive == 0)
        {
            return OnMatchWin();
        }
        return m_playersAlive;
    }

    /// <summary>
    /// 
    /// </summary>
    public void OnMatchTie()
    {
        m_phase = MatchPhase.SuddenDeath;
    }

    /// <summary>
    /// 
    /// </summary>
    public int OnMatchWin()
    {
        m_phase = MatchPhase.End;
        return 0;
    }

    /// <summary>
    /// 
    /// </summary>
    public void OnSuddenDeathFinish()
    {
        HideSuddenDeathIndicators();
        m_phase = MatchPhase.End;
    }

    /// <summary>
    /// // Sets Sudden Death Panel to active and plays UIAnimTextColorFade animation on child TMP
    /// </summary>
    public void PlaySuddenDeathTextAnim()
    {
        m_suddenDeathPanel.SetActive(true);
        m_suddenDeathPanel.GetComponentInChildren<UIAnimTextColorFade>().StartRun();
    }

    /// <summary>
    /// Controls the effects for the Sudden Death Post-Pro Volume
    /// </summary>
    public void SuddenDeathVolumeEffects()
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public void HideSuddenDeathIndicators()
    {
        m_suddenDeathPanel.SetActive(false);
        m_suddenDeathPostProVolume.weight = 0f;
    }
}