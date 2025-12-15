using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Checkpoint Settings")]
    [SerializeField] private bool alreadyActivated = false;

    private Collider2D col;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    public void ActivateCheckpoint()
    {
        if (alreadyActivated)
            return;

        alreadyActivated = true;

        GameManager.Instance.SaveCheckpoint(PlayerController.Instance.transform);

        PlayerController.Instance.health = PlayerController.Instance.maxHealth;
        PlayerController.Instance.NotifyHealthChanged();

        UIManager.Instance.ShowCheckpointSaved();

        col.enabled = false;

        if (AudioService.Instance != null)
            AudioService.Instance.PlaySFX(SFXType.Checkpoint);
    }
}
