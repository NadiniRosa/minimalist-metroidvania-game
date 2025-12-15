using UnityEngine;

public class BouncingBubbleProjectile : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 18f;

    [Header("Bounces")]
    [SerializeField] private int maxBounces = 3;

    [Tooltip("Multiplier applied to speed after each bounce. Try 1.15 - 1.5")]
    [SerializeField] private float bounceMultiplier = 1.35f;

    [Tooltip("Prevents super-flat bounces. Adds a little upward kick when bouncing on flat ground.")]
    [SerializeField] private float minUpwardAfterBounce = 2.5f;

    [Header("Damage")]
    [SerializeField] private float damage = 1f;

    private Rigidbody2D rb;
    private Animator anim;

    private int bouncesDone = 0;
    private float currentSpeed;
    private bool popped = false;

    public void Initialize(Transform target)
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        currentSpeed = speed;
        bouncesDone = 0;

        if (rb == null || target == null) return;

        Vector2 dir = ((Vector2)target.position - (Vector2)transform.position).normalized;
        rb.linearVelocity = dir * currentSpeed;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (popped) return;

        bool hitPlayer = col.CompareTag("Player");
        bool hitGround = col.gameObject.layer == LayerMask.NameToLayer("Ground");
        bool hitPlatform = col.gameObject.layer == LayerMask.NameToLayer("Platform");

        if (hitPlayer)
        {
            if (PlayerController.Instance != null &&
                !PlayerController.Instance.playerState.Invincible &&
                !PlayerController.Instance.IsDead)
            {
                PlayerController.Instance.TakeDamage(damage);
            }

            PopNow();
            return;
        }

        if (hitGround || hitPlatform)
        {
            Bounce(col);
        }
    }

    private void Bounce(Collider2D col)
    {
        if (rb == null)
        {
            PopNow();
            return;
        }

        if (bouncesDone >= maxBounces)
        {
            PopNow();
            return;
        }

        Vector2 v = rb.linearVelocity;
        if (v.sqrMagnitude < 0.0001f)
        {
            PopNow();
            return;
        }

        Vector2 contactPoint = col.ClosestPoint(transform.position);
        Vector2 normal = ((Vector2)transform.position - contactPoint).normalized;

        if (normal.sqrMagnitude < 0.001f)
        {
            PopNow();
            return;
        }

        bouncesDone++;
        currentSpeed *= bounceMultiplier;

        Vector2 reflectedDir = Vector2.Reflect(v.normalized, normal);

        Vector2 newVel = reflectedDir * currentSpeed;

        if (Mathf.Abs(normal.y) > 0.6f && newVel.y < minUpwardAfterBounce)
            newVel.y = minUpwardAfterBounce;

        rb.linearVelocity = newVel;
    }

    private void PopNow()
    {
        if (popped) return;
        popped = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        if (anim != null)
            anim.SetTrigger("Pop");
        else
            Destroy(gameObject);
    }
   
    public void Pop()
    {
        Destroy(gameObject);
    }
}
