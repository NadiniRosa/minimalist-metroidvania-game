using UnityEngine;

public class HealthCollectable : MonoBehaviour
{
    [SerializeField] private int healthAmount = 1;
    [SerializeField] private bool destroyOnCollect = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();

        if (player == null && PlayerController.Instance != null)
            player = PlayerController.Instance;

        if (player == null) return;

        player.AddHealth(healthAmount);

        if (destroyOnCollect)
            Destroy(gameObject);
    }
}
