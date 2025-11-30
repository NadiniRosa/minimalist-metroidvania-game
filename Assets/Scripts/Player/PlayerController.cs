using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Health Settings")]
    public int health;
    public int maxHealth;

    private bool isDead = false;
    public bool IsDead => isDead;
    [Space(5)]

    [Header("Horizontal Movement Settings")]
    [SerializeField] private float walkSpeed = 1;
    [Space(5)]

    [Header("Vertical Movement Settings")]
    [SerializeField] private float jumpForce = 45;
    [SerializeField] private float jumpBufferFrames;
    [SerializeField] private float coyoteTime;
    [SerializeField] private int maxAirJumps;

    private float jumpBufferCounter = 0;
    private float coyoteTimeCounter = 0;
    private int airJumpCounter = 0;
    
    private bool canDash = true;
    private bool dashed;
    [Space(5)]

    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private float groundCheckX = 0.5f;
    [SerializeField] private LayerMask groundLayer;
    [Space(5)]

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldown;

    [Header("Attack Settings")]
    private bool attack = false;
    private float timeBetweenAttack, timeSinceAttack;

    [SerializeField] private float damage;
    [SerializeField] private Transform sideAttackTransform, upAttackTransform, downAttackTransform;
    [SerializeField] private Vector2 sideAttackArea, upAttackArea, downAttackArea;
    [SerializeField] private LayerMask attackableLayer;

    [Header("Recoil Settings")]
    [SerializeField] private int recoilXSteps = 5;
    [SerializeField] private int recoilYSteps = 5;
    [Space(3)]
    [SerializeField] private float recoilXSpeed = 100;
    [SerializeField] private float recoilYSpeed = 100;
    [Space(3)]
    private int stepsXRecoiled, stepsYRecoiled;
    [Space(5)]

    [Header("Platform Settings")]
    [SerializeField] private LayerMask platformLayer;
    
    private Collider2D playerCollider;

    private int playerLayer;
    private bool droppingFromPlatform = false;


    [HideInInspector] public PlayerStateList playerState;

    Animator animator;

    private Rigidbody2D rb;
    private float gravity;
    private float xAxis, yAxis;


    public static PlayerController Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        health = maxHealth;
    }

    void Start()
    {
        playerState = GetComponent<PlayerStateList>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        gravity = rb.gravityScale;

        playerLayer = gameObject.layer;
        playerCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (!IsAlive()) return;

        GetInputs();
        UpdateJumpVariables();

        if (playerState.Dashing) return;

        Flip();
        Move();
        Jump();
        StartDash();
        Attack();
        Recoil();
    }

    void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");

        attack = Input.GetMouseButtonDown(0);
    }

    bool IsAlive()
    {
        if (isDead) return false;

        if (health <= 0)
        {
            isDead = true;
            playerState.Invincible = true;

            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            rb.gravityScale = gravity;

            animator.SetTrigger("Death");
            GameManager.Instance.PlayerDied();

            return false;
        }

        return true;
    }

    void Flip()
    {
        if (xAxis < 0)
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
            playerState.LookingRight = false;
        }

        else if (xAxis > 0)
        {
            transform.localScale = new Vector2(1, transform.localScale.y);
            playerState.LookingRight = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(sideAttackTransform.position, sideAttackArea);
        Gizmos.DrawWireCube(upAttackTransform.position, upAttackArea);
        Gizmos.DrawWireCube(downAttackTransform.position, downAttackArea);
    }

    private void Move()
    {
        rb.linearVelocity = new Vector2(walkSpeed * xAxis, rb.linearVelocity.y);
        animator.SetBool("Walking", rb.linearVelocity.x != 0 && Grounded());
    }

    public bool Grounded()
    {
        if (droppingFromPlatform)
            return false;

        if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, groundLayer)
         || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, groundLayer)
         || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, groundLayer))
            return true;
        else
            return false;
    }

    void StartDash()
    {
        if (!GameManager.Instance.DashUnlocked) return;

        if (Input.GetButtonDown("Dash") && canDash && !dashed)
        {
            StartCoroutine(Dash());
            dashed = true;
        }

        if (Grounded())
            dashed = false;
    }

    IEnumerator Dash()
    {
        canDash = false;
        playerState.Dashing = true;

        animator.SetTrigger("Dashing");

        rb.gravityScale = 0;
        rb.linearVelocity = new Vector2(transform.localScale.x * dashSpeed, 0);

        yield return new WaitForSeconds(dashTime);

        rb.gravityScale = gravity;
        playerState.Dashing = false;

        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }

    void Jump()
    {
        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            playerState.Jumping = false;
        }

        if (Grounded() && yAxis < 0 && Input.GetButtonDown("Jump"))
        {
            StartCoroutine(DropDownPlatform());
            return;
        }

        if (!playerState.Jumping)
        {
            if (!Grounded() && !GameManager.Instance.DoubleJumpUnlocked)
                coyoteTimeCounter = 0;

            if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce);
                playerState.Jumping = true;
                return;
            }

            if (!Grounded()
                && GameManager.Instance.DoubleJumpUnlocked
                && airJumpCounter < maxAirJumps
                && Input.GetButtonDown("Jump"))
            {
                playerState.Jumping = true;
                airJumpCounter++;
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce);
            }
        }

        animator.SetBool("Jumping", !Grounded());
    }

    void UpdateJumpVariables()
    {
        if (Grounded())
        {
            playerState.Jumping = false;
            coyoteTimeCounter = coyoteTime;
            airJumpCounter = 0;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferFrames;
        }
        else
        {
            jumpBufferCounter--;
        }
    }

    void Attack()
    {
        timeSinceAttack += Time.deltaTime;

        if (attack && timeSinceAttack >= timeBetweenAttack)
        {
            timeSinceAttack = 0;

            animator.SetTrigger("Attacking");

            if (yAxis == 0 || yAxis < 0 && Grounded())
                Hit(sideAttackTransform, sideAttackArea, ref playerState.RecoilingX, recoilXSpeed);
            else if (yAxis > 0)
                Hit(upAttackTransform, upAttackArea, ref playerState.RecoilingY, recoilYSpeed);
            else if (yAxis < 0 || !Grounded())
                Hit(downAttackTransform, downAttackArea, ref playerState.RecoilingY, recoilYSpeed);
        }
    }

    void Recoil()
    {
        if (playerState.RecoilingX)
        {
            if (playerState.LookingRight)
            {
                rb.linearVelocity = new Vector2(-recoilXSpeed, 0);
            }
            else
            {
                rb.linearVelocity = new Vector2(recoilXSpeed, 0);
            }
        }

        if (playerState.RecoilingY)
        {
            rb.gravityScale = 0;
            if (yAxis < 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, recoilYSpeed);
            }
            else
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, -recoilYSpeed);
            }
            airJumpCounter = 0;
        }
        else
        {
            rb.gravityScale = gravity;
        }

        if (playerState.RecoilingX && stepsXRecoiled < recoilXSteps)
        {
            stepsXRecoiled++;
        }
        else
        {
            StopRecoilX();
        }
        if (playerState.RecoilingY && stepsYRecoiled < recoilYSteps)
        {
            stepsYRecoiled++;
        }
        else
        {
            StopRecoilY();
        }

        if (Grounded())
        {
            StopRecoilY();
        }
    }
    void StopRecoilX()
    {
        stepsXRecoiled = 0;
        playerState.RecoilingX = false;
    }
    void StopRecoilY()
    {
        stepsYRecoiled = 0;
        playerState.RecoilingY = false;
    }

    void Hit(Transform _attackTransform, Vector2 _attackArea, ref bool _recoilDir, float _recoilStrength)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);
        List<Enemy> hitEnemies = new List<Enemy>();

        if (objectsToHit.Length > 0)
        {
            _recoilDir = true;
        }
        for (int i = 0; i < objectsToHit.Length; i++)
        {
            Enemy e = objectsToHit[i].GetComponent<Enemy>();
            if (e && !hitEnemies.Contains(e))
            {
                e.EnemyHit(damage, (transform.position - objectsToHit[i].transform.position).normalized, _recoilStrength);
                hitEnemies.Add(e);
            }
        }
    }

    public void TakeDamage(float _damage)
    {
        if (isDead) return;

        health -= Mathf.RoundToInt(_damage);

        StartCoroutine(StopTakingDamage());
    }

    IEnumerator StopTakingDamage()
    {
        playerState.Invincible = true;
        animator.SetTrigger("TakeDamage");
        ClampHealth();
        yield return new WaitForSeconds(1f);
        playerState.Invincible = false;
    }

    void ClampHealth()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
    }

    IEnumerator DropDownPlatform()
    {
        droppingFromPlatform = true;

        Vector2 boxSize = new Vector2(groundCheckX * 2f, groundCheckY * 2f);
        Collider2D[] platforms = Physics2D.OverlapBoxAll(groundCheckPoint.position, boxSize, 0f, platformLayer);

        foreach (Collider2D platform in platforms)
        {
            if (platform != null)
            {
                Physics2D.IgnoreCollision(playerCollider, platform, true);
            }
        }

        yield return new WaitForSeconds(0.3f);

        foreach (Collider2D platform in platforms)
        {
            if (platform != null)
                Physics2D.IgnoreCollision(playerCollider, platform, false);
        }

        droppingFromPlatform = false;
    }
}