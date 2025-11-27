using System.Collections;
using System.Data;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
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
    
    private bool canDash = true;
    private bool dashed;

    PlayerStateList playerState;
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
    }

    void Start()
    {
        playerState = GetComponent<PlayerStateList>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        gravity = rb.gravityScale;
    }

    void Update()
    {
        GetInputs();
        UpdateJumpVariables();

        if (playerState.Dashing) return;

        Flip();
        Move();
        Jump();
        StartDash();
        Attack();
    }

    void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");

        attack = Input.GetMouseButtonDown(0);
    }

    void Flip()
    {
        if (xAxis < 0)
            transform.localScale = new Vector2(-1, transform.localScale.y);
        else if (xAxis > 0)
            transform.localScale = new Vector2(1, transform.localScale.y);
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
        if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, groundLayer)
         || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, groundLayer)
         || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, groundLayer))
            return true;
        else
            return false;
    }

    void StartDash()
    {
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

        if (!playerState.Jumping)
        {
            if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce);

                playerState.Jumping = true;
            }
            else if (!Grounded() && airJumpCounter < maxAirJumps && Input.GetButtonDown("Jump"))
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

            if (yAxis == 0 || yAxis < 0 && Grounded())
                Hit(sideAttackTransform, sideAttackArea);
            else if (yAxis > 0)
                Hit(upAttackTransform, upAttackArea);
            else if (yAxis < 0 || !Grounded())
                Hit(downAttackTransform, downAttackArea);
        }
    }

    private void Hit(Transform attackTransform, Vector2 attackArea)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(attackTransform.position, attackArea, 0, attackableLayer);

        for (int i = 0; i < objectsToHit.Length; i++)
        {
            if (objectsToHit[i].GetComponent<BaseEnemyController>() != null)
            {
                objectsToHit[i].GetComponent<BaseEnemyController>().EnemyHit(damage);
            }
        }
    }
}