using UnityEngine;

public class TutorialAreaTrigger : MonoBehaviour
{
    [Header("Tutorial")]
    [SerializeField] private Sprite tutorialSprite;
    [SerializeField] private bool oneShot = true;

    private bool hasShown = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (oneShot && hasShown) return;

        if (TutorialManager.Instance == null) return;

        if (tutorialSprite == null)
        {
            Debug.LogWarning($"TutorialAreaTrigger on {gameObject.name} has no sprite assigned.");
            return;
        }

        TutorialManager.Instance.ShowTutorial(tutorialSprite);
        hasShown = true;
    }
}
