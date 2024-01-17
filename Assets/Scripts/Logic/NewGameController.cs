using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// The <b>New</b> GameController (as opposed to <see cref="GameController"/>) handles connecting players into a <see cref="UILobbyScreen"/>,
/// then pushing them forward into an actual match. This is done in the following workflow:
/// <list type="number">
///     <item>Load <see cref="UILobbyScreen"/></item>
///     <item>Await players to connect via the PlayerInputManager</item>
///     <item>Assign players to their respective characters</item>
///     <item>Await <b>4</b> players to connect, or allow the host to fill seats in with AI.</item>
///     <item>Don't Destroy created Players & load into Match Screen</item>
/// </list>
/// </summary>
/// 
public class NewGameController : MonoBehaviour
{
    /// <summary>
    /// The amount of time before the match starts after all players are ready
    /// </summary>
    public float m_readyPreMatchTime = 5.0f;
    /// <summary>
    /// Time when all players became ready
    /// </summary>
    private float m_allPlayerReadyTime;

    public TextMeshProUGUI m_text;
    public UIAnimTextColorModify m_textModifyAnim;
    public UIAnimImageColorFade m_imageColorFade;
    public bool m_matchStart = false;
    /// <summary>
    /// Spawn height of joining player (above platform)
    /// </summary>
    public float m_playerSpawnHeight = 25.0f;
    /// <summary>
    /// Current InputManager for the Game
    /// </summary>
    public PlayerInputManager m_inputManager;
    /// <summary>
    /// List of connected players
    /// </summary>
    public GameObject[] m_players = new GameObject[4];
    /// <summary>
    /// List of PER-PLAYER UI objects to be used by each player
    /// </summary>
    public GameObject[] m_playerUIModules = new GameObject[4];
    /// <summary>
    /// Respective player spawnpoints
    /// </summary>
    public Transform[] m_playerSpawnPoints = new Transform[4];
    /// <summary>
    /// Respective player render textures - these are used in the Match Screen for HUD purposes
    /// </summary>
    public RenderTexture[] m_playerPreviewTextures = new RenderTexture[4];
    /// <summary>
    /// The InputSystem UI modules used by each respective player
    /// </summary>
    //public InputSystemUIInputModule[] m_inputModules = new InputSystemUIInputModule[4];
    
    /// <summary>
    /// Keeps track of which player has connected
    /// </summary>
    private int m_playersCount = 0;

    /// <summary>
    /// Keeps track of which players are ready to start the game
    /// </summary>
    public List<bool> m_playersReady;

    public MatchController m_matchController;
    public bool aiIsPlaying = false;
    [SerializeField]
    private GameObject aiPrefab;
    public List<GameObject> aiPlayers = new();
    public List<Vector3> aiPlayersSpawnPos = new(new Vector3[3]);

    public void AttachMatchController(MatchController p_matchControllerToRegister)
    {
        m_matchController = p_matchControllerToRegister;
    }

    private void Awake()
    {
        //Setup input manager callbacks
        m_inputManager.onPlayerJoined += ConnectNewPlayer;
        m_inputManager.onPlayerLeft += DisconnectPlayer;
        DontDestroyOnLoad(this);
    }
    private void Start()
    {
        //tartCoroutine(RunAfterSecond(1f));
    }

    /// <summary>
    /// Player Input new player connection callback method - spawns new player and assigns relevant properties.
    /// </summary>
    /// <param name="p_player">The Player that connected</param>
    public void ConnectNewPlayer(PlayerInput p_player)
    {
        //Assign player GO
        m_players[m_playersCount] = p_player.gameObject;
        m_playersReady.Add(false);
        //Assign player UI components
        //p_player.uiInputModule = m_inputModules[m_playersCount];
        //p_player.GetComponent<MultiplayerEventSystem>().playerRoot = m_playerUIModules[m_playersCount];
        //p_player.GetComponent<MultiplayerEventSystem>().firstSelectedGameObject = m_playerUIModules[m_playersCount].transform.GetChild(0).gameObject;
        //Assign player Render Texture
        p_player.gameObject.GetComponentInChildren<Camera>().targetTexture = m_playerPreviewTextures[m_playersCount];

        //Set player spawn position
        var l_platformPosition = m_playerSpawnPoints[m_playersCount].position;
        p_player.transform.position = new Vector3(l_platformPosition.x, l_platformPosition.y + m_playerSpawnHeight, l_platformPosition.z);
        //ClientNetworkTransform (NetworkRigidbody) breaks without the below line, needs investigation
        //p_player.gameObject.GetComponent<Rigidbody>().isKinematic = false;

        m_playerUIModules[m_playersCount].transform.GetChild(0).GetComponent<UIAnimImageColorFade>().StartRun();

        //Assign random colours to local players
        var l_skinController = p_player.GetComponent<PlayerSkinController>();
        l_skinController.m_charData = PlayerCharacterData.RandomCharacterData();
        l_skinController.SetPlayerID(m_playersCount.ToString());
        p_player.transform.localScale *= 1.5f;

        //m_playerUIModules[m_playersCount].transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "";
        //m_playerUIModules[m_playersCount].transform.GetChild(2).gameObject.SetActive(true);

        p_player.GetComponent<PlayerController>().SetPlayerID(m_playersCount);
        m_playersCount++;
    }


    public void DisconnectPlayer(PlayerInput p_player)
    {
        
    }

    public void ResetPlayers()
    {
        for (int i = 0; i < m_players.Length; i++)
        {
            Destroy(m_players[i]);
        }
    }

    public void SetPlayerReady(int m_playerID)
    {
        m_playersReady[m_playerID] = true;
        m_players[m_playerID].GetComponent<Rigidbody>().isKinematic = true;
        m_players[m_playerID].GetComponent<PlayerController>().enabled = false;
        m_playerUIModules[m_playerID].transform.GetChild(0).GetComponent<UIAnimImageColorFade>().StartReverse();
        m_playerUIModules[m_playerID].transform.GetChild(1).GetComponent<UIAnimRawImageColorFade>().StartRun();
        m_playerUIModules[m_playerID].transform.GetChild(2).GetComponent<UIAnimTextColorFade>().StartRun();
    }

    public void FixedUpdate()
    {
        if (m_playersCount > 0)
        {
            if (!m_matchStart)
            {
                if (m_allPlayerReadyTime == 0)
                {
                    bool l_allReady = true;
                    for (int i = 0; i < m_playersReady.Count; i++)
                    {
                        if (!m_playersReady[i])
                        {
                            l_allReady = false;
                            break;
                        }
                        else
                        {
                            m_allPlayerReadyTime = Time.time;
                        }
                    }

                    if (l_allReady && m_allPlayerReadyTime > 0)
                    {
                        m_textModifyAnim.gameObject.SetActive(true);
                        m_textModifyAnim.StartRun();
                        m_imageColorFade.gameObject.SetActive(true);
                        m_imageColorFade.StartRun();
                    } else
                    {
                        m_allPlayerReadyTime = 0;
                    }
                }
                else
                {
                    if (Time.time < m_allPlayerReadyTime + m_readyPreMatchTime)
                    {
                        m_text.text = Mathf.CeilToInt(m_readyPreMatchTime - (Time.time - m_allPlayerReadyTime)).ToString() + "s";
                    }
                    else
                    {
                        m_text.text = "0s\nMATCH START!";
                        //Start match!


                        for (int i = 0; i < m_players.Length; i++)
                        {
                            if (m_players[i] != null)
                            {
                                aiIsPlaying = false;
                                m_players[i].GetComponent<Rigidbody>().isKinematic = false;
                                m_players[i].GetComponent<PlayerController>().StartMatchMechanics(); 
                                m_players[i].GetComponent<PlayerController>().enabled = true;
                                m_players[i].GetComponent<PlayerController>().m_renderTextureCamera.gameObject.SetActive(false);
                                DontDestroyOnLoad(m_players[i]);
                            }
                            else
                            {
                                //Assign AI here!
                                aiIsPlaying = true;
                                GameObject aiChar = Instantiate(aiPrefab, aiPlayersSpawnPos[i-1], Quaternion.identity);
                                aiChar.name = "AIChar" + i;
                                aiChar.GetComponent<AIHandler>().StartMatchMechanics();
                                aiPlayers.Add(aiChar);
                                aiChar.GetComponent<AIHandler>().SetPlayerID(m_playersCount);
                                m_playersCount++;
                                DontDestroyOnLoad(aiChar);
                            }
                        }

                        SceneManager.LoadScene(3);
                        m_matchStart = true;
                    }
                }
            }
        }
    }
}