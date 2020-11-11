using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
  private Enemy[] _enemies;
  private static int _nextLevelIndex = 1;

  private void OnEnable()
  {
    _enemies = FindObjectsOfType<Enemy>();

  }

  // Update is called once per frame
  void Update()
  {
    foreach (Enemy enemy in _enemies)
    {
      if (enemy)
      {
        return;
      }
    }

    print("Level finished");
    string nextLevelName = "Level" + ++_nextLevelIndex;
    SceneManager.LoadScene(nextLevelName);
  }
}
