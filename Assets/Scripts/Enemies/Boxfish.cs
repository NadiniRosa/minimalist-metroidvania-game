using System.Collections;
using UnityEngine;

public class Boxfish : Enemy
{
    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 3f;

    [Header("Bubble Settings")]
    [SerializeField] private GameObject bubblePrefab;
    [SerializeField] private Transform mouthPoint;

    private Animator animator;
    private bool playerInside = false;
    private Coroutine attackRoutine;
    private Transform playerTransform;

    private BubbleProjectile currentBubble;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Attack()
    {
        // boxfish doesnt do damage
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (isDead) return;

        playerInside = true;
        playerTransform = other.transform;

        if (attackRoutine == null)
            attackRoutine = StartCoroutine(AttackLoop());
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (isDead) return;

        playerInside = false;

        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
        }
    }

    private IEnumerator AttackLoop()
    {
        while (playerInside && !isDead)
        {
            if (currentBubble == null)
            {
                animator.SetTrigger("Attack");
                yield return new WaitForSeconds(attackCooldown);
            }
            else
                yield return null;
        }
    }

    public void SpawnBubble()
    {
        if (bubblePrefab == null || mouthPoint == null) return;
        if (isDead) return;

        GameObject bubbleObj = Instantiate(bubblePrefab, mouthPoint.position, Quaternion.identity);
        BubbleProjectile bp = bubbleObj.GetComponent<BubbleProjectile>();

        if (bp != null && playerInside && playerTransform != null)
        {
            currentBubble = bp;
            bp.Initialize(playerTransform, this);
        }
    }

    public void OnBubblePopped(BubbleProjectile bubble)
    {
        if (currentBubble == bubble)
            currentBubble = null;
    }

    protected override void Die()
    {
        if (isDead) return;
        isDead = true;

        playerInside = false;
        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
        }

        if (currentBubble != null)
        {
            currentBubble.Pop();
            currentBubble = null;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        foreach (var col in GetComponents<Collider2D>())
            col.enabled = false;

        if (animator != null)
            animator.SetTrigger("Death");
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
