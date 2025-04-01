using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour

{
    public string nextScene;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") )
        {
            SceneManager.LoadScene(nextScene);
        }
    }
    public void quitGame()
    {
        Application.Quit();
    }

}