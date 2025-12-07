using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthHUD : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private List<Image> barImages = new List<Image>();

    [Header("Sprites")]
    [SerializeField] private Sprite lifeSprite;
    [SerializeField] private Sprite lostLifeSprite;
    [SerializeField] private Sprite noLifeSprite;

    private void Awake()
    {
        if (player == null && PlayerController.Instance != null)
            player = PlayerController.Instance;

        if (player != null)
        {
            player.OnHealthChanged += UpdateHealthBars;
            UpdateHealthBars();
        }
    }

    private void UpdateHealthBars()
    {
        if (player == null) return;

        int currentHealth = player.health;
        int maxHealth = player.maxHealth;

        maxHealth = Mathf.Clamp(maxHealth, 0, barImages.Count);
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        for (int i = 0; i < barImages.Count; i++)
        {
            Image img = barImages[i];
            if (img == null) continue;

            if (i < maxHealth)
                img.sprite = (i < currentHealth) ? lifeSprite : lostLifeSprite;
            else
                img.sprite = noLifeSprite;
        }
    }
}
