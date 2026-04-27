using UnityEngine;
using UnityEngine.SceneManagement;

public class CaballeroControl : MonoBehaviour
{
    [Header("Configuración de Misión")]
    [Tooltip("¿Cuántos objetos necesita para pasar de nivel?")]
    public int itemsRequired = 3; 
    
    private int itemsDelivered = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si lo que toca al caballero tiene la tag "Item"
        if (other.CompareTag("Item"))
        {
            itemsDelivered++;
            Destroy(other.gameObject); // El objeto desaparece
            
            Debug.Log("Objeto entregado. Total: " + itemsDelivered + "/" + itemsRequired);

            // Comprobamos si ya hemos terminado
            if (itemsDelivered >= itemsRequired)
            {
                PasarAlSiguienteNivel();
            }
        }
    }

    void PasarAlSiguienteNivel()
    {
        // Obtenemos el índice de la escena actual y le sumamos 1
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        // Comprobamos si existe una siguiente escena en el Build Settings
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("¡Felicidades! Has completado todos los niveles.");
        }
    }
}
