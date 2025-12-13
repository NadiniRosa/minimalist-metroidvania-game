using UnityEngine;

public class TutorialAreaTriggerRune : MonoBehaviour
{
    [Header("Tutorial")]
    [SerializeField] private Sprite tutorialSprite;
    [SerializeField] private bool oneShot = true;

    [Header("Unlock UI Counter")]
    [SerializeField] private RuneCounter counterToUnlock;
    [SerializeField] private bool unlockAlsoOneShot = true;

    private bool hasShown = false;
    private bool hasUnlocked = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (!(oneShot && hasShown))
        {
            if (TutorialManager.Instance == null) return;

            if (tutorialSprite == null)
            {
                Debug.LogWarning($"TutorialAreaTriggerUnlock on {gameObject.name} has no sprite assigned.");
                return;
            }

            TutorialManager.Instance.ShowTutorial(tutorialSprite);
            hasShown = true;
        }

        if (counterToUnlock != null && !(unlockAlsoOneShot && hasUnlocked))
        {
            counterToUnlock.Refresh();
            hasUnlocked = true;
        }
    }
}
