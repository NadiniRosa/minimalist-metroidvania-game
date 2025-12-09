using UnityEngine;

public class FossilWall : MonoBehaviour
{
    [SerializeField] private int health = 5;

    private int hitsTaken = 0;
    private bool destroyed = false;

    private Animator animator;
    private Collider2D[] colliders;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        colliders = GetComponents<Collider2D>();
    }

    public void Hit()
    {
        if (destroyed) return;

        hitsTaken++;

        if (hitsTaken >= health)
        {
            destroyed = true;

            if (animator != null)
                animator.SetBool("Destroyed", true);

            foreach (var col in colliders)
                col.enabled = false;
        }
    }
}
