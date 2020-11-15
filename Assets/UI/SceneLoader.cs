using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
  public void LoadGame(string name)
  {
    SceneManager.LoadScene(name);
  }
  public void QuitGame()
  {
    Application.Quit();
  }
}
