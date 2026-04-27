using UnityEngine;

public class DoorControl : MonoBehaviour
{
    [Header("Configuración de la Puerta")]
    public string keyName = "Llave"; // Nombre exacto del prefab de la llave
    
    [Header("Sonidos")]
    public AudioClip openSound; // Arrastra aquí el sonido de la puerta abriéndose

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ItemManager inventory = collision.gameObject.GetComponent<ItemManager>();

            if (inventory != null)
            {
                // Intentamos usar la llave desde cualquier hueco del inventario
                if (inventory.TryUseItem(keyName))
                {
                    AbrirPuerta();
                }
                else
                {
                    Debug.Log("Necesitas la llave: " + keyName);
                }
            }
        }
    }

    void AbrirPuerta()
    {
        // Reproducimos el sonido en la posición de la puerta
        if (openSound != null)
        {
            // Usamos PlayClipAtPoint porque el objeto va a ser destruido inmediatamente.
            // Esto crea un objeto temporal que reproduce el sonido y luego se borra solo.
            AudioSource.PlayClipAtPoint(openSound, transform.position);
        }

        Debug.Log("¡Puerta abierta!");
        
        // Aquí podrías añadir un efecto de partículas o una animación antes de destruir
        Destroy(gameObject);
    }
}