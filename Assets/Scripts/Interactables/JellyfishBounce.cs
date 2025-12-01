using UnityEngine;

public class JellyfishBounce : MonoBehaviour
{
    [SerializeField] private float bounceForce = 35f;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player")) return;

        PlayerController player = collision.collider.GetComponent<PlayerController>();

        if (player == null || player.IsDead) return;


        if (player.transform.position.y <= transform.position.y) return;

        Rigidbody2D playerRb = collision.collider.attachedRigidbody;

        if (playerRb == null) return;

        playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 0);
        playerRb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);

        player.playerState.Jumping = false;

        if (animator != null)
            animator.SetBool("Bounce", true);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player")) return;

        if (animator != null)
            animator.SetBool("Bounce", false);
    }
}
