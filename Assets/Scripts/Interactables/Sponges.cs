using UnityEngine;

public class Sponges : MonoBehaviour
{
    [Header("Sponge Health")]
    [SerializeField] private int health = 1;

    [Header("Health Drop Settings")]
    [SerializeField] private GameObject healthCollectablePrefab;
    [Range(0f, 1f)]
    [SerializeField] private float dropChance = 0.25f;

    private int hitsTaken = 0;
    private bool destroyed = false;

    private Animator animator;
    private Collider2D col;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }

    public void Hit()
    {
        if (destroyed) return;

        hitsTaken++;

        if (hitsTaken >= health)
        {
            destroyed = true;

            TrySpawnHealthDrop();

            if (animator != null)
                animator.SetBool("Destroyed", true);

            if (col != null)
                col.enabled = false;

            if (AudioService.Instance != null)
                AudioService.Instance.PlaySFX(SFXType.PropBreaking);
        }
    }

    private void TrySpawnHealthDrop()
    {
        if (healthCollectablePrefab == null) return;

        float roll = Random.value;

        if (roll <= dropChance)
            Instantiate(healthCollectablePrefab, transform.position, Quaternion.identity);
    }
}
