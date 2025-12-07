using UnityEngine;

public class DoubleJumpCollectable : MonoBehaviour
{
    [SerializeField] private bool destroyOnCollect = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (GameManager.Instance != null)
            GameManager.Instance.DoubleJumpUnlocked = true;

        if (destroyOnCollect)
            Destroy(gameObject);
    }
}
