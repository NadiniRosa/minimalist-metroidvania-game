using UnityEngine;

public class LightCollectable : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private bool destroyOnCollect = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (GameManager.Instance != null)
            GameManager.Instance.LightUnlocked = true;

        if (destroyOnCollect)
            Destroy(gameObject);
    }
}
