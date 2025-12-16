using UnityEngine;

public class ExtraLifeCollectable : MonoBehaviour
{
    [SerializeField] private bool destroyOnCollect = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();

        if (player == null && PlayerController.Instance != null)
            player = PlayerController.Instance;

        if (player == null) return;

        player.UnlockExtraLife();

        if (AudioService.Instance != null)
            AudioService.Instance.PlaySFX(SFXType.HealthCollected);

        if (destroyOnCollect)
            Destroy(gameObject);
    }
}
