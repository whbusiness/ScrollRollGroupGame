using System.Collections;
using UnityEngine.Animations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public GameObject m_crown;

    private MasterInput m_masterInput;
    
    public Rigidbody m_rb;

    public Camera m_renderTextureCamera;

    public Transform m_playerPrefab;

    [Header("Physics Control")]
    //Ground check control
    [SerializeField]
    private bool m_isJumping = false;
    [SerializeField]
    private bool m_isGrounded = false;
    public bool isBlocking = false;
    //Player physics configuration
    [SerializeField]
    private EntityPhysicsData m_playerPhysicsData;
    [SerializeField]
    private PlayerOrientationConfig m_orientation;
    //Player movement direction
    [SerializeField]
    private Vector2 m_movement;
    [SerializeField]
    private GameObject cameraZoomer;
    //These should be removed once we have a proper character model
    [SerializeField]
    private Transform m_arm;
    private float m_maxArmRotation = 3.0f;
    [SerializeField]
    private float m_offPlatformTime;
    [SerializeField]
    private float m_lastOffPlatformTime;
    public bool m_isOffPlatform;
    public bool m_lastTouchedByPlayer;
    public PlayerController m_lastTouchedBy;

    public PlayerHUDController m_hudController;
    public PlayerMatchData m_matchData;
    public AttackType m_lastAttackCast;
    public AttackType m_lastAttackHit;
    private float m_lifeStart = 0;

    private GameObject playerHitByPoison;
    public bool stunned = false, stuckInIce;
    private float stunTimer, poisonTimer, flameThrowerTimer;

    private int iceHealth = 10;
    private int playerLayer;

    public static int powerUp;
    private bool enableFlameThrower;
    public static bool playerCanAttack = false;

    [Header("Attack Options")]
    [SerializeField]
    private float m_maxDistanceForPushPullCheck;
    [SerializeField]
    private float m_amountOfForceAppliedOnPush;
    [SerializeField]
    private float m_amountOfForceAppliedOnPull;
    [SerializeField]
    private float m_amountOfPoisonDamagePerSecond;
    [SerializeField]
    private float m_flameThrowerDamageRanConstantFor2Seconds;

    [Header("Audio/Sounds")]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip fireProjectileAudio;
    [SerializeField]
    private AudioClip blockAudio;
    [SerializeField]
    private AudioClip pushPullAudio;
    [SerializeField]
    private AudioClip jumpAudio;
    [SerializeField]
    private AudioClip pickupCollisionAudio;
    [SerializeField]
    private AudioClip scrollChangingEnvironmentAudio;
    public float audioSourceVolume;

    public bool m_isDestroyingSelf;
    private float m_selfDestroySpeed;
    private float coolDownTimePush, coolDownTimePull, blockDelay;
    private bool playerCanPush = true, playerCanPull = true, playerCanBlock = true;

    [Space]
    [Header("Player Indicator")]
    public int m_playerID;
    public TMPro.TextMeshProUGUI m_playerIndicatorText;


    [Header("Animations")]
    [SerializeField]
    private GameObject fireAnimationProjectile;
    [SerializeField]
    private GameObject stunAnimationProjectile;
    [SerializeField]
    private GameObject iceAnimationProjectile, iceHitAnim;
    [SerializeField]
    private GameObject poisonAnimationProjectile;
    [SerializeField]
    private GameObject stunAnimationHit;
    [SerializeField]
    private GameObject poisonAnimationHit;
    private NewGameController m_gameController;
    private GameObject aiHit;
    [SerializeField]
    private float pushForceAgainstAI, pullForceAgainstAI;
    [SerializeField]
    private RuntimeAnimatorController stunProjectileRight, stunProjectileLeft, iceProjectileRight, iceProjectileLeft;
    private Gamepad connectedController;
    public Animator _anim;
    public PlayerSkinController playerSkinController;
    [SerializeField]
    private GameObject blockingShield;
    [SerializeField]
    private Transform raycastGO;
    [SerializeField]
    private float timeStuckInPoisonPlatform;
    [SerializeField]
    private Stuck isStuck;
    public void AttachHUDController(PlayerHUDController p_hudController)
    {
        m_hudController = p_hudController;
    }

    public void StartMatchMechanics()
    {
        m_rb.isKinematic = false;
        transform.position = m_playerPhysicsData.m_spawnLocation;
        m_matchData = new PlayerMatchData();
        m_matchData.m_isInMatch = true;
        m_lifeStart = Time.time;
    }

    // Start is called before the first frame update
    void Awake()
    {
        if(m_rb == null) TryGetComponent(out m_rb);

        if (m_playerID < Gamepad.all.Count)
        {
            connectedController = Gamepad.all[m_playerID];
        }
        if (m_playerPhysicsData == null) Debug.LogError("Player Controller " + gameObject.name + " lacks EntityPhysicsData!");
        _anim = GetComponentInChildren<Animator>();
        m_masterInput = new MasterInput();
        m_masterInput.Enable();
        
        m_masterInput.Player.MoveLeft.canceled += ctx => { m_movement.x = 0; };
        m_masterInput.Player.MoveRight.canceled += ctx => { m_movement.x = 0; };
        audioSource = GetComponent<AudioSource>();
        playerLayer = 1 << 6;
    }

    public void SetPlayerID(int p_newID)
    {
        m_playerID = p_newID;
        m_playerIndicatorText.text = "PLAYER " + (m_playerID + 1);
    }

    public void OnMoveUp()
    {
        if (!m_isJumping && m_isGrounded){
            if (!enableFlameThrower)
            {
                audioSource.Stop();
                audioSource.clip = jumpAudio;
                audioSource.time = 0f;
                audioSource.volume = audioSourceVolume;
                audioSource.Play();
            }
            m_isJumping = true;
            m_isGrounded = false;
            m_movement.y = 1;
            _anim.SetBool("Jump", true);
            m_rb.AddForce(new Vector3(0, m_playerPhysicsData.m_jumpHeight, 0), ForceMode.Impulse);
            OnLookUp();
        }
    }
    public void OnPowerUp()
    {
        if (m_matchData.m_isInMatch)
        {
            switch (m_hudController.UsePowerup())
            {
                case PowerupType.Ice: //ice powerup
                    print("Use ice powerup");
                    StartCoroutine(IceProjectileAnimation(.5f));
                    if (Physics.Raycast(transform.position, -raycastGO.forward, out RaycastHit hite, m_maxDistanceForPushPullCheck, playerLayer, QueryTriggerInteraction.Ignore))
                    {
                        if (hite.collider.gameObject.TryGetComponent<PlayerController>(out var l_player))
                        {
                            StartCoroutine(l_player.playerSkinController.DamageRed(0.5f));
                            l_player.StartCoroutine(nameof(Rumble), .5f);
                            l_player.StartCoroutine(nameof(HitIcePowerup), .5f);
                            l_player.stuckInIce = true;
                            iceHealth = 10;
                        }
                    }
                    break;
                case PowerupType.Fire: //fire powerup
                    print("Use fire powerup");
                    audioSource.clip = fireProjectileAudio;
                    audioSource.Stop();
                    enableFlameThrower = true;
                    break;
                case PowerupType.Electric: //electric powerup
                    print("Use electric powerup");
                    StartCoroutine(StunProjectileAnimation(.5f));
                    if (Physics.Raycast(transform.position, -raycastGO.forward, out RaycastHit hit, m_maxDistanceForPushPullCheck, playerLayer, QueryTriggerInteraction.Ignore))
                    {
                        if (hit.collider.gameObject.TryGetComponent<PlayerController>(out var l_player))
                        {
                            StartCoroutine(l_player.playerSkinController.DamageRed(0.5f));
                            l_player.StartCoroutine(nameof(Rumble), .5f);
                            l_player.stunned = true;
                        }
                    }
                    break;
                case PowerupType.Poison: //poison powerup
                    print("Use poison powerup");
                    if (Physics.Raycast(transform.position, -raycastGO.forward, out RaycastHit hit2, m_maxDistanceForPushPullCheck, playerLayer, QueryTriggerInteraction.Ignore))
                    {
                        if (hit2.collider.gameObject.TryGetComponent<PlayerController>(out var l_player))
                        {
                            print("Hit OtherPlayer");
                            playerHitByPoison = hit2.collider.gameObject;
                            poisonTimer = 0;
                            l_player.StartCoroutine(PoisonDamageEverySecond(10f));
                        }
                    }
                    break;
                default:
                    print("Has no powerup");
                    break;
            }
        } else
        {
            if(m_lifeStart > 0)
            {
                if (!m_isDestroyingSelf)
                {
                    m_isDestroyingSelf = true;
                    m_selfDestroySpeed = FindFirstObjectByType<PodiumController>().m_playerScaleSpeed;
                }
            }
        }
    }
    IEnumerator HitIcePowerup(float dur)
    {
        iceHitAnim.SetActive(true);
        yield return new WaitForSeconds(dur);
        iceHitAnim.SetActive(false);
    }
    IEnumerator StunProjectileAnimation(float sec)
    {
        if (m_orientation == PlayerOrientationConfig.LEFT || m_orientation == PlayerOrientationConfig.UP_LEFT || m_orientation == PlayerOrientationConfig.DOWN_LEFT)
        {
            print("Looking Left");
            stunAnimationProjectile.transform.eulerAngles = new Vector3(0, 0, 0);
            stunAnimationProjectile.GetComponent<Animator>().runtimeAnimatorController = stunProjectileLeft;
        }
        if (m_orientation == PlayerOrientationConfig.RIGHT || m_orientation == PlayerOrientationConfig.UP_RIGHT || m_orientation == PlayerOrientationConfig.DOWN_RIGHT)
        {
            print("Looking Right");
            stunAnimationProjectile.transform.eulerAngles = new Vector3(0, -183.511f, 0);
            stunAnimationProjectile.GetComponent<Animator>().runtimeAnimatorController = stunProjectileRight;
        }
        stunAnimationProjectile.SetActive(true);
        yield return new WaitForSeconds(sec);
            stunAnimationProjectile.SetActive(false);
    }
    IEnumerator IceProjectileAnimation(float sec)
    {
        if (m_orientation == PlayerOrientationConfig.LEFT || m_orientation == PlayerOrientationConfig.UP_LEFT || m_orientation == PlayerOrientationConfig.DOWN_LEFT)
        {
            print("Looking Left");
            iceAnimationProjectile.transform.eulerAngles = new Vector3(0, 180, 0);
            iceAnimationProjectile.GetComponent<Animator>().runtimeAnimatorController = iceProjectileLeft;
        }
        if (m_orientation == PlayerOrientationConfig.RIGHT || m_orientation == PlayerOrientationConfig.UP_RIGHT || m_orientation == PlayerOrientationConfig.DOWN_RIGHT)
        {
            print("Looking Right");
            iceAnimationProjectile.transform.eulerAngles = new Vector3(0, 0, 0);
            iceAnimationProjectile.GetComponent<Animator>().runtimeAnimatorController = iceProjectileRight;
        }
        iceAnimationProjectile.SetActive(true);
        yield return new WaitForSeconds(sec);
            iceAnimationProjectile.SetActive(false);
    }

    IEnumerator PoisonDamageEverySecond(float sec)
    {
        while(true)
        {
            poisonTimer += 1;
            //print(poisonTimer);
            Gamepad.current.SetMotorSpeeds(0.1f, 0.2f);
            playerHitByPoison.TryGetComponent<PlayerController>(out var l_player);
            var l_hudController = l_player.m_hudController;
            l_hudController.m_data.TakeDamage(m_amountOfPoisonDamagePerSecond);
            l_hudController.UpdateMagicalOverload();
            print("Poisoning");
            l_player.poisonAnimationHit.SetActive(true);
            yield return new WaitForSeconds(1);
            Gamepad.current.ResetHaptics();
            if(poisonTimer >= sec)
            {
                l_player.poisonAnimationHit.SetActive(false);
                print("end poison");
                yield break;
            }
        }
    }

    public void OnLookLeft()
    {
        Rotate(PlayerOrientationConfig.LEFT);
    }

    public void OnLookRight()
    {
        Rotate(PlayerOrientationConfig.RIGHT);
    }

    public void OnLookUp()
    {
        if (m_orientation == PlayerOrientationConfig.LEFT || m_orientation == PlayerOrientationConfig.DOWN_LEFT ||
            m_orientation == PlayerOrientationConfig.UP_LEFT)
        {
            Rotate(PlayerOrientationConfig.UP_LEFT);
        }
        else if (m_orientation == PlayerOrientationConfig.RIGHT || m_orientation == PlayerOrientationConfig.DOWN_RIGHT ||
                 m_orientation == PlayerOrientationConfig.UP_RIGHT)
        {
            Rotate(PlayerOrientationConfig.UP_RIGHT);
        }
        else
        {
            Rotate(PlayerOrientationConfig.UP);
        }
    }

    public void OnLookDown()
    {
        OnMoveDown();
    }

    public void OnMoveLeft()
    {
        m_movement = new Vector2(-1, m_movement.y);
        Rotate(PlayerOrientationConfig.LEFT);
    }

    public void OnMoveRight()
    {
        m_movement = new Vector2(1, m_movement.y);
        Rotate(PlayerOrientationConfig.RIGHT);
    }

    public void OnMoveDown()
    {
        if(m_orientation == PlayerOrientationConfig.UP_LEFT || m_orientation == PlayerOrientationConfig.LEFT)
        {
            Rotate(PlayerOrientationConfig.DOWN_LEFT);
        } else if (m_orientation == PlayerOrientationConfig.UP_RIGHT || m_orientation == PlayerOrientationConfig.RIGHT)
        {
            Rotate(PlayerOrientationConfig.DOWN_RIGHT);
        } else
        {
            Rotate(PlayerOrientationConfig.DOWN);
        }
    }

    public void Rotate(PlayerOrientationConfig p_newOrientation = PlayerOrientationConfig.NONE)
    {
        if (p_newOrientation != PlayerOrientationConfig.NONE && p_newOrientation != m_orientation)
        {
            m_orientation = p_newOrientation;
        } 
        m_playerPrefab.localEulerAngles = new Vector3(0, PlayerOrientation.GetOrientationFromConfig(m_orientation).GetLookDir().x * 135.0f);
        m_arm.transform.localEulerAngles = new Vector3(0, -180.0f, PlayerOrientation.GetOrientationFromConfig(m_orientation).GetLookDir().y * m_maxArmRotation);
        raycastGO.localEulerAngles = new Vector3(0, PlayerOrientation.GetOrientationFromConfig(m_orientation).GetLookDir().x * -90.0f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
       // Debug.DrawLine(transform.position, transform.position -raycastGO.forward * m_maxDistanceForPushPullCheck, Color.yellow);
        if (!m_isDestroyingSelf)
        {
            
            if (m_matchData.m_isInMatch)
            {
                if(cameraZoomer == null)
                {
                    cameraZoomer = GameObject.Find("CameraZoom");
                }
                if (stunned)
                {
                    stunAnimationHit.SetActive(true);
                    print("Player stunned");
                    stunTimer += Time.deltaTime;
                    if (stunTimer > 2)
                    {
                        stunAnimationHit.SetActive(false);
                        print("unstun player");
                        stunned = false;
                    }
                    m_movement = Vector2.zero;
                }

                if (stuckInIce)
                {
                    m_movement = Vector2.zero;
                    //m_rb.isKinematic = true;
                }

                if (enableFlameThrower)
                {
                    if (m_orientation == PlayerOrientationConfig.LEFT || m_orientation == PlayerOrientationConfig.UP_LEFT || m_orientation == PlayerOrientationConfig.DOWN_LEFT)
                    {
                        print("Looking Left");
                        fireAnimationProjectile.transform.localPosition = new Vector2(-5.38f, fireAnimationProjectile.transform.position.y);
                        fireAnimationProjectile.transform.localEulerAngles = new Vector3(0, 180, 0);
                    }
                    if (m_orientation == PlayerOrientationConfig.RIGHT || m_orientation == PlayerOrientationConfig.UP_RIGHT || m_orientation == PlayerOrientationConfig.DOWN_RIGHT)
                    {
                        print("Looking Right");
                        fireAnimationProjectile.transform.localPosition = new Vector2(5.38f, fireAnimationProjectile.transform.position.y);
                        fireAnimationProjectile.transform.localEulerAngles = new Vector3(0, 0, 0);
                    }
                    fireAnimationProjectile.SetActive(true);
                    flameThrowerTimer += Time.deltaTime;
                    if (flameThrowerTimer > 2)
                    {
                        fireAnimationProjectile.SetActive(false);
                        enableFlameThrower = false;
                        if (audioSource.clip == fireProjectileAudio)
                        {
                            audioSource.Stop();
                        }
                        flameThrowerTimer = 0;
                    }

                    if (Physics.Raycast(transform.position, -raycastGO.forward, out RaycastHit hit2, m_maxDistanceForPushPullCheck, playerLayer, QueryTriggerInteraction.Ignore))
                    {
                        if (hit2.collider.gameObject.TryGetComponent<PlayerController>(out var l_player))
                        {
                            var l_hudController = l_player.m_hudController;
                            m_lastTouchedBy = l_player;
                            StartCoroutine(l_player.playerSkinController.DamageRed(0.5f));
                            l_player.StartCoroutine(nameof(Rumble), .5f);
                            l_hudController.m_data.TakeDamage(m_flameThrowerDamageRanConstantFor2Seconds);
                            l_hudController.UpdateMagicalOverload();
                        }
                    }
                }

                if (enableFlameThrower && !audioSource.isPlaying && audioSource.clip == fireProjectileAudio)
                {
                    audioSource.time = 1f;
                    audioSource.volume = audioSourceVolume;
                    audioSource.Play();
                }
            }


            if (cameraZoomer != null && m_hudController.m_data.m_currentLives == 0)
            {
                print("Remove Self");
                cameraZoomer.GetComponent<CameraZoomer>().playersAndAI.Remove(gameObject);
                cameraZoomer.GetComponent<CameraZoomer>().locationsOfPlayers.Remove(transform.position.x);
                cameraZoomer.GetComponent<CameraZoomer>().player.Remove(gameObject.GetComponent<PlayerController>());
            }

            if (m_movement != Vector2.zero && !stuckInIce && !stunned)
            {
                if (m_movement.y > 0 && m_rb.velocity.y < 0)
                {
                    m_movement.y = -1;
                }
                m_rb.AddForce(new Vector3(m_movement.x, 0, m_movement.y) * m_playerPhysicsData.m_movementSpeed * Time.fixedDeltaTime, ForceMode.Impulse);
            }

            if(m_hudController != null && m_hudController.m_data.m_currentLives > 0)
            {
                m_matchData.m_totalTimeAlive = Time.time;
            }


            if (m_matchData.m_isInMatch)
            {
                //Player has fallen off
                if (transform.position.y < -10.0f && !m_isOffPlatform)
                {
                    m_isOffPlatform = true;
                    m_lastOffPlatformTime = Time.time;

                    //Handle player falling off stat tracking
                    if (!m_lastTouchedByPlayer)
                    {
                        m_matchData.m_timesFallenOff++;
                    }
                    else
                    {
                        switch (m_lastAttackHit)
                        {
                            case AttackType.Push:
                                m_matchData.m_timesPushedOff++;
                                m_gameController = FindFirstObjectByType<NewGameController>();
                                if (!m_gameController.aiIsPlaying)
                                {
                                    m_lastTouchedBy.m_matchData.m_timesPushedOthersOff++;
                                }
                                break;
                            case AttackType.Pull:
                                m_matchData.m_timesFallenOff++;
                                m_gameController = FindFirstObjectByType<NewGameController>();
                                if (!m_gameController.aiIsPlaying)
                                {
                                    m_lastTouchedBy.m_matchData.m_timesPulledOthersOff++;
                                }
                                break;
                        }

                        m_lastAttackHit = 0;
                        m_lastTouchedBy = null;
                        m_lastTouchedByPlayer = false;
                    }

                    m_hudController.m_data.m_currentMagicalOverload = 0;
                    m_hudController.UpdateMagicalOverload();

                    //Handle losing lives
                    if (!m_hudController.LoseLife())
                    {
                        m_matchData.m_timeAliveLife1 = Time.time - m_lifeStart;
                        //Player knocked out!

                        m_matchData.m_matchPlace = (GameMatchPlace)m_hudController.m_gameController.m_matchController.OnPlayerKnockout();
                        m_rb.isKinematic = true;
                        m_matchData.m_isInMatch = false;
                    }
                    else
                    {
                        //Update lifetime tracker
                        switch (m_hudController.m_data.m_currentLives + 1)
                        {
                            case 2:
                                m_matchData.m_timeAliveLife2 = Time.time - m_lifeStart;
                                break;
                            case 3:
                                m_matchData.m_timeAliveLife3 = Time.time - m_lifeStart;
                                break;
                            default:
                                Debug.LogWarning("Player has more (or less!) lives than expected [" + m_hudController.m_data.m_currentLives + "] - this shouldn't happen!");
                                break;
                        }

                        m_lifeStart = Time.time;
                    }
                }
                //Respawn player
                if (m_isOffPlatform && Time.time > m_lastOffPlatformTime + m_offPlatformTime)
                {
                    m_isOffPlatform = false;
                    m_rb.velocity = new Vector3(0, -5.0f, 0);
                    transform.position = new Vector3(0, 10.0f, 0);
                }
            }
            //Debug.DrawRay(transform.position, transform.right * maxDistanceForPushCheck, Color.green);
        } else
        {
            transform.localScale *= m_selfDestroySpeed;

            if(transform.localScale.y <= 1 - m_selfDestroySpeed)
            {
                Destroy(gameObject);
            }
        }
        if (isStuck.playerTouchingPoisonPlatform)
        {
            timeStuckInPoisonPlatform += Time.deltaTime;
            if (timeStuckInPoisonPlatform > 3f)
            {
                transform.position = new Vector3(2f, 0);
                timeStuckInPoisonPlatform = 0;
                isStuck.playerTouchingPoisonPlatform = false;
            }
        }


        if (_anim.GetBool("Jump") && !m_isJumping)
        {
            if(_anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
            {
                _anim.SetBool("Jump", false);
            }
        }
        if(m_movement.x != 0)
        {
            _anim.SetBool("Run", true);
        }
        else
        {
            if(_anim.GetBool("Run")) _anim.SetBool("Run", false);
        }
        if (_anim.GetBool("PushStart"))
        {
            if (_anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
            {
                _anim.SetBool("PushStart", false);
            }
        }
        if (_anim.GetBool("PullStart"))
        {
            if (_anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
            {
                _anim.SetBool("PullStart", false);
            }
        }
        if (_anim.GetBool("BlockStart"))
        {
            if(isBlocking)
            {
                if(!Keyboard.current.bKey.isPressed || Gamepad.current != null && !Gamepad.current.leftShoulder.isPressed)//needed as block could get stuck on
                {
                    isBlocking = false;
                    _anim.SetBool("BlockHold", false);
                    _anim.SetBool("BlockStart", false);

                    blockingShield.SetActive(false);
                    print("Not blockign");
                    playerCanBlock = false;
                    blockDelay = 0;
                    StartCoroutine(nameof(DelayNextBlock), 1f);
                }
                if (_anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
                {
                    _anim.SetBool("BlockHold", true);
                }
            }
            else
            {
                _anim.SetBool("BlockHold", false);
                _anim.SetBool("BlockStart", false);
            }
        }
    }

    void OnBlock()//On press and release
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name != "LVL_LOBBY")
        {
            if (playerCanBlock)
            {
                print("Blocked");
                _anim.SetBool("BlockStart", true);
                isBlocking = !isBlocking;
                //Track amount of times blocked
                if (isBlocking)
                {
                    blockingShield.SetActive(true);
                    print("Still Blocking");
                    m_matchData.m_timesBlocked++;
                    if (!enableFlameThrower)
                    {
                        audioSource.Stop();
                        audioSource.clip = blockAudio;
                        audioSource.volume = audioSourceVolume;
                        audioSource.time = 0;
                        print("Play Audio");
                        audioSource.Play();
                    }
                }
                if (!isBlocking)
                {
                    blockingShield.SetActive(false);
                    print("Not blockign");
                    playerCanBlock = false;
                    blockDelay = 0;
                    StartCoroutine(nameof(DelayNextBlock), 1f);
                }
            }
            else
            {
                print("Dont RUn");
                playerCanBlock = false;
            }
        }
    }

    IEnumerator DelayNextBlock(float sec)
    {
        while (true)
        {
            print("Waiting before can block again");
            blockDelay += 1;
            yield return new WaitForSeconds(1);
            if (blockDelay >= sec)
            {
                print("Can block again");
                playerCanBlock = true;
                yield break;
            }
        }
    }

    void OnPlayerPush()//When player presses rt on controller or left click on mouse
    {
        if (!isBlocking && !stuckInIce && playerCanPush)
        { 
            m_lastAttackCast = AttackType.Push;
            m_matchData.m_timesPushed++;
            _anim.SetBool("PushStart", true);
            if (Physics.Raycast(transform.position, -raycastGO.forward, out RaycastHit hit, m_maxDistanceForPushPullCheck, playerLayer, QueryTriggerInteraction.Ignore))
            {
                print("Pushed");
                playerCanPush = false;
                StartCoroutine(nameof(PushAttackCooldown),2f);
                if (!enableFlameThrower)
                {
                    audioSource.Stop();
                    audioSource.volume = audioSourceVolume;
                    audioSource.clip = pushPullAudio;
                    audioSource.time = 0;
                    audioSource.Play();
                }
                try
                {
                    var l_player = hit.collider.gameObject.GetComponentInParent<PlayerController>();
                    //If hit player not blocking
                    if (l_player != null && !l_player.isBlocking)
                    {
                        var l_hudController = l_player.m_hudController;
                        m_lastTouchedBy = l_player;
                        l_hudController.m_data.TakeDamage(m_amountOfForceAppliedOnPush);
                        l_hudController.UpdateMagicalOverload();
                        StartCoroutine(l_player.playerSkinController.DamageRed(0.5f));
                        l_player.StartCoroutine(nameof(Rumble), .5f);
                        l_player.m_rb.AddForce(-raycastGO.forward * (1 + Mathf.Pow(l_hudController.m_data.m_currentMagicalOverload / 100.0f, 2)), ForceMode.Impulse);

                        l_player.m_lastTouchedBy = this;
                        l_player.m_lastTouchedByPlayer = true;

                        m_matchData.m_timesPushedHit++;
                        l_player.m_matchData.m_timesPushedByOthers++;
                        l_player.m_lastAttackHit = AttackType.Push;
                        //If hit player blocking
                    }
                    else if (l_player != null && l_player.isBlocking)
                    {
                        m_matchData.m_timesBlockedByOthers++;
                        l_player.m_matchData.m_timesBlockedHit++;
                    }
                    if(l_player == null)
                    {
                        if (hit.collider.gameObject.CompareTag("AI"))
                        {
                            var l_aiPlayer = hit.collider.gameObject.GetComponent<AIHandler>();
                            var l_hudController = l_aiPlayer.m_hudController;

                            l_hudController.m_data.TakeDamage(m_amountOfForceAppliedOnPush);
                            l_hudController.UpdateMagicalOverload();
                            StartCoroutine(l_aiPlayer.playerSkinController.DamageRed(0.5f));
                            l_aiPlayer.m_lastTouchedByPlayer = true;
                            l_aiPlayer.GetComponent<Rigidbody>().AddForce(-raycastGO.forward * (1 + Mathf.Pow(l_hudController.m_data.m_currentMagicalOverload / 100.0f, 2)), ForceMode.Impulse);
                            m_matchData.m_timesPulledHit++;
                            l_aiPlayer.m_matchData.m_timesPushedByOthers++;
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
        if(stuckInIce)
        {
            iceHealth -= 1;
            if(iceHealth <= 1)
            {
                stuckInIce = false;
                m_rb.isKinematic = false;
            }
        }
    }


    public void PlayPickupAudio()
    {
        if (!enableFlameThrower)
        {
            audioSource.Stop();
            audioSource.clip = pickupCollisionAudio;
            audioSource.volume = audioSourceVolume;
            audioSource.time = 0f;
            audioSource.Play();
        }
    }

    void OnPlayerPull()
    {
        if (!isBlocking && !stuckInIce && playerCanPull)
        {
            m_lastAttackCast = AttackType.Pull;
            m_matchData.m_timesPulled++;
            _anim.SetBool("PullStart", true);
            if (Physics.Raycast(transform.position, -raycastGO.forward, out RaycastHit hit, m_maxDistanceForPushPullCheck, playerLayer, QueryTriggerInteraction.Ignore))
            {
                print("Pulled");
                playerCanPull = false;
                StartCoroutine(nameof(PullAttackCooldown), 2f);
                if (!enableFlameThrower)
                {
                    audioSource.volume = audioSourceVolume;
                    audioSource.Stop();
                    audioSource.clip = pushPullAudio;
                    audioSource.time = 0;
                    audioSource.Play();
                }
                try
                {
                    var l_player = hit.collider.gameObject.GetComponentInParent<PlayerController>();
                    var l_player_skin_controller = hit.collider.gameObject.GetComponentInParent<PlayerSkinController>();
                    //If hit player not blocking
                    if (l_player != null && !l_player.isBlocking)
                    {
                        //This if statement should be removed when we have one cohesive model for the wizards
                        var l_hudController = l_player.m_hudController;

                        l_hudController.m_data.TakeDamage(m_amountOfForceAppliedOnPull);
                        l_hudController.UpdateMagicalOverload();
                        StartCoroutine(l_player.playerSkinController.DamageRed(0.5f));
                        l_player.StartCoroutine(nameof(Rumble), .5f);
                        l_player.m_rb.AddForce(-raycastGO.forward * (1 + Mathf.Pow(l_hudController.m_data.m_currentMagicalOverload / 100.0f, 2)), ForceMode.Impulse);

                        l_player.m_lastTouchedBy = this;
                        l_player.m_lastTouchedByPlayer = true;

                        m_matchData.m_timesPulledHit++;
                        l_player.m_matchData.m_timesPulledByOthers++;
                        l_player.m_lastAttackHit = AttackType.Pull;
                        //If hit player blocking
                    }
                    else if (l_player != null && l_player.isBlocking)
                    {
                        m_matchData.m_timesBlockedByOthers++;
                        l_player.m_matchData.m_timesBlockedHit++;
                    }
                    if (l_player == null)
                    {
                        if (hit.collider.gameObject.CompareTag("AI"))
                        {
                            var l_aiPlayer = hit.collider.gameObject.GetComponent<AIHandler>();
                            var l_hudController = l_aiPlayer.m_hudController;
                            l_aiPlayer.m_lastTouchedByPlayer = true;

                            l_hudController.m_data.TakeDamage(m_amountOfForceAppliedOnPull);
                            l_hudController.UpdateMagicalOverload();
                            StartCoroutine(l_aiPlayer.playerSkinController.DamageRed(0.5f));
                            l_aiPlayer.GetComponent<Rigidbody>().AddForce(-raycastGO.forward * (1 + Mathf.Pow(l_hudController.m_data.m_currentMagicalOverload / 100.0f, 2)), ForceMode.Impulse);
                            m_matchData.m_timesPulledHit++;
                            l_aiPlayer.m_matchData.m_timesPulledByOthers++;
                        }
                    }
                } 
                catch(System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
    }

    IEnumerator PushAttackCooldown(float sec)
    {
        while (true)
        {
            print("Waiting before can push again");
            coolDownTimePush += 1;
            yield return new WaitForSeconds(1);
            if (coolDownTimePush >= sec)
            {
                print("Can push again");
                playerCanPush = true;
                yield break;
            }
        }
    }
    IEnumerator PullAttackCooldown(float sec)
    {
        while (true)
        {
            print("Waiting before can pull again");
            coolDownTimePull += 1;
            yield return new WaitForSeconds(1);
            if (coolDownTimePull >= sec)
            {
                print("Can pull again");
                playerCanPull = true;
                yield break;
            }
        }
    }

    public IEnumerator Rumble(float dur)
    {
        while (true)
        {
            print("RUMBLE");
            if(connectedController != null)
            {
                connectedController.SetMotorSpeeds(0.2f, .4f);
                yield return new WaitForSeconds(dur);
                print("STOP RUMBLE");
                connectedController.ResetHaptics();
            }
            yield break;
        }
    }

    //Handle ground checking
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent(out PlayerController l_player))
        {
            m_lastTouchedBy = l_player;
        }

        if (!m_isGrounded || m_isJumping)
        {
            if (collision.collider.TryGetComponent(out GroundFlag l_flag))
            {
                if (l_flag != null && l_flag.Check())
                {
                    m_isGrounded = true;
                    m_isJumping = false;
                    m_movement.y = 0;
                    m_lastTouchedByPlayer = false;
                    m_lastTouchedBy = null;
                }
            }
        }
    }

    
}
