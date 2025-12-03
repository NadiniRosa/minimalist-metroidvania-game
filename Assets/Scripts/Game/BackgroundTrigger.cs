using UnityEngine;

public class BackgroundTrigger : MonoBehaviour
{
    [SerializeField] private AreaID areaToSet;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!collider.CompareTag("Player")) return;
        if (BackgroundManager.Instance == null) return;

        BackgroundManager.Instance.SetBackground(areaToSet);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.3f, 0.7f, 1f, 0.3f);
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
}
