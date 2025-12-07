using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float health;

    [Header("Recoil")]
    [SerializeField] protected float recoilLength;
    [SerializeField] protected float recoilFactor;

    protected float recoilTimer;

    [SerializeField] protected bool isRecoiling = false;

    [SerializeField] protected float speed;
    [SerializeField] protected float damage;

    [SerializeField] protected PlayerController player;
    protected Rigidbody2D rb;

    protected bool isDead = false;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = PlayerController.Instance;
    }

    protected virtual void Update()
    {
        if (isDead) return;

        if (health <= 0)
        {
            Die();
            return;
        }

        if (isRecoiling)
        {
            if (recoilTimer < recoilLength)
            {
                recoilTimer += Time.deltaTime;
            }
            else
            {
                isRecoiling = false;
                recoilTimer = 0;
            }
        }
    }

    public virtual void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        if (isDead) return;

        health -= _damageDone;

        if (!isRecoiling)
        {
            rb.AddForce(-_hitForce * recoilFactor * _hitDirection);
            isRecoiling = true;
        }
    }

    protected void OnTriggerStay2D(Collider2D _other)
    {
        if (isDead) return;

        if (_other.CompareTag("Player") &&
            !PlayerController.Instance.playerState.Invincible &&
            !PlayerController.Instance.IsDead)
        {
            Attack();
        }
    }

    protected virtual void Attack()
    {
        PlayerController.Instance.TakeDamage(damage);
    }

    protected virtual void Die()
    {
        isDead = true;
        Destroy(gameObject);
    }
}
