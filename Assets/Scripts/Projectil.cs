using UnityEngine;

public class Projectil : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 2f; // Se destruye tras 2 segundos para no llenar la memoria

    void Start()
    {
        // Se destruye solo después de un tiempo si no choca con nada
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyControl>().TakeDamage(1);
            Destroy(gameObject); // La bala desaparece al impactar
        }
        else if (other.CompareTag("Walls"))
        {
            Destroy(gameObject);
        }
    }
}