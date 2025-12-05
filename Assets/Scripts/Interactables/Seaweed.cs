using UnityEngine;

public class Seaweed : MonoBehaviour
{
    [SerializeField] private int health = 3;

    private int hitsTaken = 0;
    private bool destroyed = false;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
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
        }
    }
}
