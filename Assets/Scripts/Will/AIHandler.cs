using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AIHandler : MonoBehaviour
{
    private GameObject cameraZoom;
    public List<GameObject> players = new();
    [SerializeField]
    private float timeSpentChasing;
    [SerializeField]
    private float amountOfTimeUntilNewTarget;
    [SerializeField]
    private bool hasTarget;
    [SerializeField]
    private GameObject target;
    [SerializeField]
    private float distanceToAIAttackTarget, radius;
    private bool canAttack;
    [SerializeField]
    private float forceOfPush;
    [SerializeField]
    private float playerBeneathDistCheck;
    [SerializeField]
    private bool movingLeft, movingRight;
    public PlayerHUDController m_hudController;
    public PlayerMatchData m_matchData;
    private float m_lifeStart = 0;
    private bool onStart = true;
    [SerializeField]
    private bool hasBeenPushed = false;
    [SerializeField]
    private float attackDist;
    private bool hitByAi;
    [SerializeField]
    private bool isGrounded;
    [SerializeField]
    private float aiMovementSpeed;
    private Rigidbody _rb;
    private bool canJump;
    [SerializeField]
    private float jumpHeight;
    [SerializeField]
    private float m_offPlatformTime;
    [SerializeField]
    private float m_lastOffPlatformTime;
    [SerializeField]
    private bool m_isOffPlatform;
    public bool m_lastTouchedByPlayer;
    private AIHandler m_lastTouchedBy;
    [SerializeField]
    private AttackType m_lastAttackCast;
    [SerializeField]
    private AttackType m_lastAttackHit;
    public GameObject m_crown;
    public bool stunned;
    [Space]
    [Header("Player Indicator")]
    public int m_playerID;
    public TMPro.TextMeshProUGUI m_playerIndicatorText;
    private bool onTopOfPlayer;
    private bool targetOutOfBounds;
    private int layerMask;
    [SerializeField]
    private Collider[] hitColliders;
    [SerializeField]
    private Animator _anim;
    public PlayerSkinController playerSkinController;
    [SerializeField]
    private float timeStuckInPoisonPlatform;
    [SerializeField]
    private Stuck isStuck;
    public void AttachHUDController(PlayerHUDController p_hudController)
    {
        m_hudController = p_hudController;
    }
    private void Start()
    {
        stunned = false;
        canJump = true;
        _rb = GetComponent<Rigidbody>();
        hitByAi = false;
        onStart = true;
        canAttack = true;
        hasTarget = false;
        layerMask = 1 << 6;
    }

    public void SetPlayerID(int p_newID)
    {
        m_playerID = p_newID;
        m_playerIndicatorText.text = "PLAYER " + (m_playerID + 1);
    }

    public void StartMatchMechanics()
    {
        m_matchData = new PlayerMatchData();
        m_matchData.m_isInMatch = true;
        m_lifeStart = Time.time;
    }

    private void Update()
    {
        if (cameraZoom == null)
        {
            cameraZoom = GameObject.Find("CameraZoom");
        }
        else
        {

            if (_anim.GetBool("Jump"))
            {
                if (_anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
                {
                    _anim.SetBool("Jump", false);
                }
            }
            if (_anim.GetBool("PushStart"))
            {
                if (_anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
                {
                    _anim.SetBool("PushStart", false);
                }
            }
            if (_rb.velocity.magnitude > 0 && !_anim.GetBool("PushStart") && !_anim.GetBool("Jump"))
            {
                _anim.SetBool("Run", true);
            }
            else
            {
                _anim.SetBool("Run", false);
            }

            if (m_hudController.m_data.m_currentLives == 0)
            {
                cameraZoom.GetComponent<CameraZoomer>().playersAndAI.Remove(gameObject);
                cameraZoom.GetComponent<CameraZoomer>().locationsOfPlayers.Remove(transform.position.x);
            }
            if (onStart)
            {
                m_hudController.m_data.m_currentMagicalOverload = 0;
                m_hudController.UpdateMagicalOverload();
                onStart = false;
            }
            if (timeSpentChasing < amountOfTimeUntilNewTarget)
            {
                //if (!_agent.isOnNavMesh) return;
                timeSpentChasing += Time.deltaTime;
                if (!hasTarget && cameraZoom.GetComponent<CameraZoomer>().playersAndAI.Count > 1 && timeSpentChasing > 1f)
                {
                    print("Make it True");
                    do
                    {
                        target = cameraZoom.GetComponent<CameraZoomer>().playersAndAI[Random.Range(0, cameraZoom.GetComponent<CameraZoomer>().playersAndAI.Count)];
                    } while (target == gameObject);
                    hasTarget = true;
                }
                if(target != null && target.GetComponent<AIHandler>() != null)
                {
                    if(target.GetComponent<AIHandler>().m_isOffPlatform)
                    {
                        hasTarget = false;
                    }
                }
                if (target != null && target.GetComponent<PlayerController>() != null)
                {
                    if (target.GetComponent<PlayerController>().m_isOffPlatform)
                    {
                        hasTarget = false;
                    }
                }
                if (!onTopOfPlayer && !stunned && cameraZoom.GetComponent<CameraZoomer>().playersAndAI.Count >= 1 && !targetOutOfBounds && target!= null)
                {
                    ChaseTarget(target.transform.position);
                }
                if (onTopOfPlayer)
                {
                    _rb.AddForce(aiMovementSpeed * Time.deltaTime * transform.forward, ForceMode.Impulse);
                }
                RaycastHit[] multipleHits = Physics.RaycastAll(transform.position, -transform.forward, attackDist);
                if(multipleHits.Length > 0 )
                {
                    if(canAttack)
                    {
                        AiAttack(multipleHits[0].collider.gameObject);
                    }
                }
                if(multipleHits.Length > 1 )
                {
                    if(isGrounded)
                    {
                        AiJump();
                    }
                }
                hitColliders = Physics.OverlapSphere(transform.position - transform.up * playerBeneathDistCheck, radius, layerMask);
                if(hitColliders.Length > 1 ) 
                {
                    onTopOfPlayer = true;
                }
                if (hitColliders.Length <= 1)
                {
                    onTopOfPlayer = false;
                }
                if (target != null && target.transform.position.x < -9.53 || target != null && target.transform.position.x > 10.23)
                {
                    targetOutOfBounds = true;
                    hasTarget = false;
                }
                else
                {
                    targetOutOfBounds = false;
                }
            }
            else
            {
                hasTarget = false;
                timeSpentChasing = 0;
            }

            if (m_hudController != null && m_hudController.m_data.m_currentLives > 0)
            {
                m_matchData.m_totalTimeAlive = Time.time;
            }

            if (m_matchData.m_isInMatch)
            {
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
                                m_lastTouchedBy.m_matchData.m_timesPushedOthersOff++;
                                break;
                            case AttackType.Pull:
                                m_matchData.m_timesFallenOff++;
                                m_lastTouchedBy.m_matchData.m_timesPulledOthersOff++;
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
                        _rb.isKinematic = true;
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
                                Debug.LogWarning("AI has more (or less!) lives than expected [" + m_hudController.m_data.m_currentLives + "] - this shouldn't happen!");
                                break;
                        }

                        m_lifeStart = Time.time;
                    }
                }
                //Respawn player
                if (m_isOffPlatform && Time.time > m_lastOffPlatformTime + m_offPlatformTime)
                {
                    m_isOffPlatform = false;
                    _rb.velocity = new Vector3(0, -5.0f, 0);
                    transform.position = new Vector3(0, 10.0f, 0);
                    hasTarget = false;
                    target = null;
                    timeSpentChasing = 0;
                }
            }
        }
        Debug.DrawLine(transform.position, transform.position - transform.forward * attackDist, Color.red);
    }

    void AiJump()
    {
        if (isGrounded && canJump)
        {
            print("Jumped");
            _anim.SetBool("Run", false);
            _anim.SetBool("Jump", true);
            _rb.AddForce(new Vector3(0, jumpHeight, 0), ForceMode.Impulse);
            StartCoroutine(nameof(DelayNextJump), 1f);
        }
    }

    IEnumerator DelayNextJump(float delay)
    {
        canJump = false;
        yield return new WaitForSeconds(delay);
        canJump = true;
    }

    void AiAttack(GameObject playerOrAiToPush)
    {
        if(canAttack)
        {
            m_matchData.m_timesPushed++;
            if (playerOrAiToPush.GetComponent<PlayerController>() != null)
            {
                _anim.SetBool("Run", false);
                _anim.SetBool("PushStart", true);
                var l_player = playerOrAiToPush.GetComponent<PlayerController>();
                //If hit player not blocking
                if (l_player != null && !l_player.isBlocking)
                {
                    var l_hudController = l_player.m_hudController;
                    l_hudController.m_data.TakeDamage(forceOfPush);
                    l_hudController.UpdateMagicalOverload();
                    StartCoroutine(l_player.playerSkinController.DamageRed(0.5f));
                    StartCoroutine(l_player.Rumble(0.5f));
                    l_player.m_rb.AddForce(-transform.forward * (1 + Mathf.Pow(l_hudController.m_data.m_currentMagicalOverload / 100.0f, 2)), ForceMode.Impulse);
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
            }
            if (playerOrAiToPush.CompareTag("AI"))
            {
                _anim.SetBool("Run", false);
                _anim.SetBool("PushStart", true);
                var l_aiPlayer = playerOrAiToPush.GetComponent<AIHandler>();
                var l_hudController = l_aiPlayer.m_hudController;
                l_hudController.m_data.TakeDamage(forceOfPush);
                l_hudController.UpdateMagicalOverload();
                StartCoroutine(l_aiPlayer.playerSkinController.DamageRed(0.5f));
                l_aiPlayer._rb.AddForce(-transform.forward * (1 + Mathf.Pow(l_hudController.m_data.m_currentMagicalOverload / 100.0f, 2)), ForceMode.Impulse);
                //l_aiPlayer.GetComponent<Rigidbody>().AddForce(-transform.forward * (1 + Mathf.Pow(l_hudController.m_data.m_currentMagicalOverload / 100.0f, 2)), ForceMode.Impulse);
                //l_aiPlayer.GetComponent<NavMeshAgent>().velocity = l_aiPlayer.GetComponent<Rigidbody>().velocity;
                m_matchData.m_timesPushedHit++;
                l_aiPlayer.m_matchData.m_timesPushedByOthers++;
            }
            StartCoroutine(nameof(DelayNextAttack), 1f);
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position - transform.up * playerBeneathDistCheck, radius);
    }
    IEnumerator DelayNextAttack(float dur)
    {
        canAttack = false;
        yield return new WaitForSeconds(dur);
        canAttack = true;
    }

    void ChaseTarget(Vector3 targPos)
    {
        if(targPos.x < transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0, 90, 0);
        }
        else if(targPos.x > transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0, -90, 0);
        }
        _rb.AddForce(aiMovementSpeed * Time.deltaTime * -transform.forward, ForceMode.Impulse);
        //transform.rotation = Quaternion.Slerp(transform.rotation, target.transform.rotation, 5);
        //transform.position = Vector3.MoveTowards(transform.position, targPos, aiMovementSpeed * Time.deltaTime);
       // gameObject.Vec.SetDestination(targPos);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.gameObject.CompareTag("Player") || collision.collider.gameObject.CompareTag("AI"))
        {
            print("Touching Player Or AI");
            _rb.AddForce(force: aiMovementSpeed * Time.deltaTime * transform.forward, mode: ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isGrounded)
        {
            if (collision.collider.TryGetComponent(out GroundFlag l_flag))
            {
                if (l_flag != null && l_flag.Check())
                {
                    isGrounded = true;
                    m_lastTouchedByPlayer = false;
                }
            }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (isGrounded)
        {
            if (collision.collider.TryGetComponent(out GroundFlag l_flag))
            {
                if (l_flag != null && l_flag.Check())
                {
                    isGrounded = false;
                }
            }
        }

    }

}