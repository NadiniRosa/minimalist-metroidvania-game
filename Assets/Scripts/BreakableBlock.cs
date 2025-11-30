using UnityEngine;

public class BreakableBlock : Enemy
{
    [Header("Vida do bloco")]
    public float maxHealth = 50f;
    private float currentHealth;

    [Header("Debris (pedaços)")]
    public GameObject debrisPrefab;
    public int debrisAmount = 8;
    public float debrisForce = 6f;

    [Header("Opções")]
    public bool disableColliderOnBreak = true;
    public float destroyDelay = 0f;

    bool destroyed = false;
    Collider2D myCollider;
    SpriteRenderer sr;

    protected override void Start()
    {
        base.Start();

        currentHealth = maxHealth;

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        myCollider = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();

        Debug.Log($"[BreakableBlock] Iniciado com {maxHealth} de vida.");
    }

    protected override void Update()
    {
        // vazio — bloco não se move
    }

    public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        if (destroyed) return;

        currentHealth -= _damageDone;

        Debug.Log($"[BreakableBlock] Dano recebido: {_damageDone}. Vida restante: {currentHealth}");

        if (currentHealth <= 0f)
        {
            Debug.Log("[BreakableBlock] VIDA CHEGOU A 0 — QUEBRANDO O BLOCO!");
            StartBreakSequence();
        }
    }

    void StartBreakSequence()
    {
        destroyed = true;

        if (disableColliderOnBreak && myCollider != null) myCollider.enabled = false;
        if (sr != null) sr.enabled = false;

        Debug.Log("[BreakableBlock] Gerando pedaços...");

        for (int i = 0; i < debrisAmount; i++)
        {
            if (debrisPrefab == null) break;

            GameObject p = Instantiate(debrisPrefab, transform.position, Quaternion.identity);

            Rigidbody2D pieceRb = p.GetComponent<Rigidbody2D>();
            if (pieceRb != null)
            {
                Vector2 dir = Random.insideUnitCircle.normalized;
                pieceRb.AddForce(dir * debrisForce, ForceMode2D.Impulse);
                pieceRb.AddTorque(Random.Range(-360f, 360f));
            }
        }

        Debug.Log("[BreakableBlock] Bloco destruído!");

        if (destroyDelay <= 0f)
            Destroy(gameObject);
        else
            Destroy(gameObject, destroyDelay);
    }
}
