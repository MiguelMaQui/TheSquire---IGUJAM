using UnityEngine;
using System.Collections;

public class EnemyControl : MonoBehaviour
{
    [Header("Ajustes de Movimiento")]
    public float moveSpeed = 3f;
    public float detectionRange = 5f;
    public int health = 3;
    private bool isAggro = false; 

    [Header("Visuales")]
    public SpriteRenderer spriteRenderer;
    public Sprite rightSprite; // Obligatorio
    public Sprite leftSprite;  // Opcional (si es null, hará flip al de la derecha)
    public Sprite upSprite;    // Opcional
    public Sprite downSprite;  // Opcional

    [Header("Sonidos")]
    public AudioClip deathSound;

    private Transform player;
    private Rigidbody2D rb;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
        rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.freezeRotation = true;
    }

    void Update()
    {
        if (player == null) return;
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance < detectionRange || isAggro) isAggro = true;
    }

    void FixedUpdate()
    {
        if (isAggro && player != null)
        {
            Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;
            ActualizarVisuales(direction);
        }
        else rb.linearVelocity = Vector2.zero;
    }

    void ActualizarVisuales(Vector2 dir)
    {
        // PRIORIDAD HORIZONTAL
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            if (dir.x > 0) // Derecha
            {
                spriteRenderer.sprite = rightSprite;
                spriteRenderer.flipX = false;
            }
            else // Izquierda
            {
                if (leftSprite != null)
                {
                    spriteRenderer.sprite = leftSprite;
                    spriteRenderer.flipX = false;
                }
                else
                {
                    spriteRenderer.sprite = rightSprite;
                    spriteRenderer.flipX = true; // Hacemos espejo si no hay sprite de izquierda
                }
            }
        }
        // PRIORIDAD VERTICAL
        else
        {
            if (dir.y > 0 && upSprite != null) // Arriba
            {
                spriteRenderer.sprite = upSprite;
                spriteRenderer.flipX = false;
            }
            else if (dir.y < 0 && downSprite != null) // Abajo
            {
                spriteRenderer.sprite = downSprite;
                spriteRenderer.flipX = false;
            }
            else // Fallback a lateral si no hay sprites verticales
            {
                if (dir.x >= 0)
                {
                    spriteRenderer.sprite = rightSprite;
                    spriteRenderer.flipX = false;
                }
                else
                {
                    if (leftSprite != null)
                    {
                        spriteRenderer.sprite = leftSprite;
                        spriteRenderer.flipX = false;
                    }
                    else
                    {
                        spriteRenderer.sprite = rightSprite;
                        spriteRenderer.flipX = true;
                    }
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        isAggro = true; 
        StartCoroutine(FlashRed());
        if (health <= 0) Morir();
    }

    void Morir()
    {
        if (deathSound != null) AudioSource.PlayClipAtPoint(deathSound, transform.position);
        Destroy(gameObject);
    }

    IEnumerator FlashRed()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerControl pc = collision.gameObject.GetComponent<PlayerControl>();
            if (pc != null) pc.TakeDamage(1);
        }
    }
}