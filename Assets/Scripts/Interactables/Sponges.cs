using UnityEngine;

public class Sponges : MonoBehaviour
{
    [SerializeField] private int health = 3;

    private int hitsTaken = 0;
    private bool destroyed = false;

    private Animator animator;
    private Collider2D col;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
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

            col.enabled = false;
        }
    }
}
