using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;

public class MenuControl : MonoBehaviour
{
    [SerializeField] GameObject Menu;
    

    public void StartGame()
    {
        SceneManager.LoadScene("Nivel1");
    }

    public void lvl2()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void goToMenu()
    {
        SceneManager.LoadScene("MainMenu");
        // PlayerControl.health = 3;
    }

    public void continueGame()
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            Menu.SetActive(false);
        }
        
    }


}
