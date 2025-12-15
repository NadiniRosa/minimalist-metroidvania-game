using UnityEngine;

public class BossArenaTrigger : MonoBehaviour
{
    [SerializeField] private LionFish boss;

    private void Awake()
    {
        if (boss == null)
            boss = FindObjectOfType<LionFish>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        boss?.SetPlayerInArena(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        boss?.SetPlayerInArena(false);
    }
}
