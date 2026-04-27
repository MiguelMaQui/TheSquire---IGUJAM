using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;

    void Awake()
    {
        // --- LÓGICA SINGLETON ---
        // Si ya existe una instancia de música (porque volvimos al menú), 
        // destruimos esta nueva para que no se solapen dos canciones.
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        // Esto es lo que hace la magia: el objeto sobrevive al cambio de escenas
        DontDestroyOnLoad(gameObject);
    }
}