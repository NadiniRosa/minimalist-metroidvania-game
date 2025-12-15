using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RuneDoor : MonoBehaviour
{
    [Header("Requirement")]
    [SerializeField] private RuneCounter runeCounter;
    [SerializeField] private int runesRequired = 3;
    [SerializeField] private bool consumeRunes = true;

    [Header("Door Visuals")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] placeRuneSprites; 
    [SerializeField] private float timePerSprite = 1f;

    [Header("Colliders")]
    [SerializeField] private Collider2D triggerCollider;
    [SerializeField] private Collider2D solidCollider; 

    [Header("Lights To Enable After Unlock")]
    [SerializeField] private Transform lightsRoot;

    private bool unlocking = false;
    private bool unlocked = false;

    private void Reset()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (unlocked || unlocking) return;
        if (!other.CompareTag("Player")) return;
        if (runeCounter == null) return;

        if (runeCounter.Get() >= runesRequired)
            StartCoroutine(UnlockSequence());
    }

    private IEnumerator UnlockSequence()
    {
        unlocking = true;

        if (consumeRunes && runeCounter != null)
            runeCounter.Add(-runesRequired);

        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        if (placeRuneSprites != null && placeRuneSprites.Length > 0 && spriteRenderer != null)
        {
            int count = Mathf.Min(3, placeRuneSprites.Length);
            for (int i = 0; i < count; i++)
            {
                spriteRenderer.sprite = placeRuneSprites[i];
                yield return new WaitForSeconds(timePerSprite);
            }
        }
        else
        {
            yield return new WaitForSeconds(timePerSprite);
        }

        if (triggerCollider != null) triggerCollider.enabled = false;
        if (solidCollider != null) solidCollider.enabled = false;

        unlocked = true;
        unlocking = false;

        EnableLightsUnderRoot();
    }

    private void EnableLightsUnderRoot()
    {
        if (lightsRoot == null) return;

        Light2D[] lights = lightsRoot.GetComponentsInChildren<Light2D>(true);
        for (int i = 0; i < lights.Length; i++)
        {
            if (lights[i] != null)
                lights[i].enabled = true;
        }
    }
}
