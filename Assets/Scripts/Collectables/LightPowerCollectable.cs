using UnityEngine;

public class LightCollectable : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private bool destroyOnCollect = true;

    [Header("Tutorial")]
    [SerializeField] private Sprite tutorialSprite;
    [SerializeField] private bool showTutorial = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (GameManager.Instance != null)
            GameManager.Instance.LightUnlocked = true;

        if (showTutorial && TutorialManager.Instance != null && tutorialSprite != null)
            TutorialManager.Instance.ShowTutorial(tutorialSprite);

        if (AudioService.Instance != null)
            AudioService.Instance.PlaySFX(SFXType.PowerUp);

        if (destroyOnCollect)
            Destroy(gameObject);
    }
}
