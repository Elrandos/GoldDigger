using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void NewGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void LoadGame()
    {
        Debug.Log("Load Game - jeszcze nie zaimplementowane!");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game - w edytorze nie działa, tylko w buildzie");
    }
}
