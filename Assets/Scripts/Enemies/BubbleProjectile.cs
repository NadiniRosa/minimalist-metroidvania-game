using UnityEngine;

public class BubbleProjectile : MonoBehaviour
{
    [Header("Trajectory Settings")]
    [SerializeField] private float timeToTarget = 1.2f;
    [SerializeField] private float damage = 1f;

    private Rigidbody2D rb;
    private Animator anim;
    private Boxfish owner;

    private bool popped = false;

    public void Initialize(Transform target, Boxfish shooter)
    {
        owner = shooter;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (rb == null) return;

        Vector2 start = transform.position;
        Vector2 end = target.position;

        float dx = end.x - start.x;
        float dy = end.y - start.y;

        float g = -Physics2D.gravity.y * rb.gravityScale;

        if (timeToTarget <= 0.01f || g <= 0.01f)
        {
            Vector2 dir = (end - start).normalized;
            rb.linearVelocity = dir * 5f;
            return;
        }

        float T = timeToTarget;
        float vx = dx / T;
        float vy = (dy + 0.5f * g * T * T) / T;

        rb.linearVelocity = new Vector2(vx, vy);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (popped) return;

        bool hitPlayer = col.CompareTag("Player");
        bool hitGround = col.gameObject.layer == LayerMask.NameToLayer("Ground");
        bool hitPlatform = col.gameObject.layer == LayerMask.NameToLayer("Platform");

        if (hitPlayer)
            if (!PlayerController.Instance.playerState.Invincible && !PlayerController.Instance.IsDead)
                PlayerController.Instance.TakeDamage(damage);

        if (hitPlayer || hitGround || hitPlatform)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;

            if (anim != null)
                anim.SetTrigger("Pop");
            else
                Pop();
        }
    }

    public void Pop()
    {
        if (popped) return;
        popped = true;

        if (owner != null)
            owner.OnBubblePopped(this);

        Destroy(gameObject);
    }
}
