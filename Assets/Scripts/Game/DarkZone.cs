using UnityEngine;

public class DarkZone : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private int damagePerTick = 1;
    [SerializeField] private float damageInterval = 1f;

    private float lastDamageTime = -999f;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (GameManager.Instance != null && GameManager.Instance.LightUnlocked) return;

        PlayerController player = other.GetComponent<PlayerController>();

        if (player == null && PlayerController.Instance != null)
            player = PlayerController.Instance;

        if (player == null)
            return;

        if (Time.time >= lastDamageTime + damageInterval)
        {
            player.TakeDamage(damagePerTick);
            lastDamageTime = Time.time;
        }
    }
}
