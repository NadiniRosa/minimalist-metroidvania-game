using UnityEngine;

public class BouncePad : MonoBehaviour
{
    [Header("Força do salto")]
    public float bounceForce = 25f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica se o player colidiu
        PlayerController player = collision.collider.GetComponent<PlayerController>();
        if (player != null)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

            // zera velocidade vertical e aplica impulso para cima
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);

            Debug.Log("BOUNCE!");
        }
    }
}
