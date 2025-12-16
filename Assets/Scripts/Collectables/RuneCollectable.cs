using UnityEngine;

public class RuneCollectable : MonoBehaviour
{
    [SerializeField] private int amount = 1;
    [SerializeField] private bool destroyOnCollect = true;

    private RuneCounter runeCounter;

    private void Awake()
    {
        runeCounter = FindFirstObjectByType<RuneCounter>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (GameManager.Instance != null)
            GameManager.Instance.Runes = Mathf.Max(0, GameManager.Instance.Runes + amount);

        if (runeCounter != null)
            runeCounter.Refresh();

        if (AudioService.Instance != null)
            AudioService.Instance.PlaySFX(SFXType.PowerUp);

        if (destroyOnCollect)
            Destroy(gameObject);
    }
}
